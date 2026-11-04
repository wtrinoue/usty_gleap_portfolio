# PauseSystem 改善点まとめ

## 概要

PauseSystem は、各オブジェクトが個別に `Update()` を持つ代わりに、`PauseManager` が登録されたオブジェクトの `OnCustomUpdate()` を一括で呼び出す仕組みです。
ポーズ中は `PauseManager` が呼び出しを止めるため、登録済みのオブジェクトがまとめて停止します。

| ファイル | 役割 |
| --- | --- |
| `Interface/IPausable.cs` | ポーズ対象であることを示すインターフェース（`OnCustomUpdate()`） |
| `MonoBehaviour/PauseManager.cs` | シングルトン。登録されたオブジェクトの `OnCustomUpdate()` を毎フレーム呼ぶ |
| `MonoBehaviour/PauseRegister.cs` | インスペクターで指定したコンポーネントを `PauseManager` に登録・解除する |
| `Sample/PausableRotation.cs` | 使い方のサンプル（回転するだけ） |

---

## 対応済みの改善

### 1. 再有効化したオブジェクトが動かなくなる問題

- **問題**: 登録が `Start`（最初の1回のみ）、解除が `OnDisable`（無効化のたび）で対になっておらず、`SetActive(false)` → `SetActive(true)` すると二度と登録されなかった。オブジェクトプールを使う弾などで問題になる。
- **対応**:
  - `PauseRegister` で `OnEnable` で登録、`OnDisable` で解除するように変更。
  - シーン開始時に `PauseManager` がまだ無い場合の保険として `Start` でも登録（`HashSet` なので二重登録はされない）。
  - `PauseManager` に `[DefaultExecutionOrder(-1000)]` を付け、他スクリプトより先に `Instance` が用意されるようにした。

### 2. 更新ループ中のコレクション変更による例外

- **問題**: `OnCustomUpdate()` の中でオブジェクトの破棄・生成・無効化が起きると、`foreach` 中に `HashSet` が変更され `InvalidOperationException` が発生する可能性があった。
- **対応**:
  - ループ中の `Register` / `Unregister` は保留リスト（`_pendingAdd` / `_pendingRemove`）に溜め、ループ終了後に `ApplyPendingChanges()` でまとめて反映。
  - ループ中に解除されたオブジェクトは、そのフレームの残りでは `OnCustomUpdate()` を呼ばない。
  - 例外発生時も状態が壊れないよう `try / finally` で保護。

### 3. シーン終了時の NullReferenceException

- **問題**: シーン切り替え時に `PauseManager` が先に破棄されると、`PauseRegister.OnDisable` で `PauseManager.Instance` が null になりエラーになる可能性があった。
- **対応**:
  - `PauseManager.OnDestroy` で `Instance` を null に戻す。
  - `PauseRegister` の登録・解除前に `Instance` の null チェックを追加。

### その他

- `Instance` を `{ get; private set; }` のプロパティにし、外部から書き換えられないようにした。
- ポーズ状態を確認できる `IsPaused` プロパティを追加。
- 登録のたびに出ていたデバッグログを削除。
- 未使用の `using System.Security.Cryptography;` を削除。

---

## 未対応の課題・今後の検討事項

### 4. `OnCustomUpdate()` 以外の処理は止まらない

PauseSystem で止まるのは `OnCustomUpdate()` の中身だけです。以下はポーズ中も動き続けます。

- `Rigidbody` / `Rigidbody2D` の物理演算（速度・重力）
- `Animator` によるアニメーション
- コルーチン、`Invoke`
- パーティクル（`ParticleSystem`）
- オーディオ

**対応案**

- `IPausable` に `OnPause()` / `OnResume()` を追加し、各コンポーネントが自分で `Rigidbody` の速度保存・`Animator.speed = 0` などを行えるようにする。
- もしくは、ポーズ時は `Time.timeScale = 0` と併用する（ただし UI アニメーションなどへの影響に注意）。

### 5. `FixedUpdate` / `LateUpdate` に相当する仕組みがない

物理処理（`FixedUpdate`）やカメラ追従（`LateUpdate`）をポーズ対象にしたい場合に対応できない。
必要に応じて `OnCustomFixedUpdate()` / `OnCustomLateUpdate()` を追加する、またはインターフェースを分けることを検討。

### 6. 既存スクリプトの移行

プロジェクト内には、まだ `MonoBehaviour.Update()` を直接使っているスクリプトが多数あります（敵・弾・武器・墓など）。
ポーズで止めたいものは、以下の手順で移行が必要です。

1. クラスに `IPausable` を実装する。
2. `Update()` の中身を `OnCustomUpdate()` に移す。
3. 同じ GameObject（または親）に `PauseRegister` を付け、`targetComponents` に対象コンポーネントを設定する。

※ UI やポーズメニューなど、ポーズ中も動いてほしいものは移行しないこと。

### 7. ポーズの切り替え操作

現状は `PauseManager.SetPause(bool)` を呼ぶ側（入力処理やポーズメニュー）が未整備。
キー入力でトグルする処理や、ポーズ開始・解除を通知するイベント（`event Action<bool> OnPauseChanged` など）があると、UI 表示との連携がしやすくなる。

### 8. `PauseRegister` の設定忘れ対策

`IPausable` を実装しても `PauseRegister` への登録を忘れると、そのオブジェクトは一切動かなくなる。
対応案として、`IPausable` を実装したコンポーネント自身が `OnEnable` / `OnDisable` で登録・解除する基底クラス（例: `PausableBehaviour`）を用意すると、設定ミスを減らせる。
