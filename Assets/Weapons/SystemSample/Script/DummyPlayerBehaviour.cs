using UnityEngine;
using System.Collections;

public class DummyPlayerBehaviour : MonoBehaviour
{
    public GameObject sampleWeaponCorePrefab;
    private StatusContainer statusContainer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statusContainer = GetComponent<StatusContainer>();
        StartCoroutine(UseWeaponInterval(2.0f));// 一定間隔で使用
    }

    // 武器使用時に呼ばれる関数
    private void UseWeapon()
    {
        // WeaponCoreを生成する。
        GameObject sampleWeaponCore = Instantiate(
            sampleWeaponCorePrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, 90f)
        );

        // WeaponCoreInterfaceの機能を起動する。
        WeaponCoreInterface wci = sampleWeaponCore.GetComponent<WeaponCoreInterface>();
        wci.Init();
        wci.ExtractPlayerStatus(statusContainer.GetStatus());// ここでPlayerのStatusを入れ込む。
        wci.Fire();
    }

    // 一定間隔で発射
    private IEnumerator UseWeaponInterval(float interval)
    {
        while (true)
        {
            UseWeapon();
            yield return new WaitForSeconds(interval);
        }
    }
}

/*
「WeaponCoreの動作原理について」

1. Playerにて
WeaponCoreの生成
WeaponCoreからWeaponCoreInterfaceの取得
WeaponCoreInterface.Init()の発動
WeaponCoreInterface.ExtractPlayer(in StatusVector s)でPlayerのStatusを入れる。
WeaponCoreInterface.Fire()の発動

2. WeaponCoreにて
Weaponの生成
WeaponからweaponInterfaceの取得
WeaponInterface.Init()の発動
WeaponInterface.InsertToken(StatusToken token)で自身のStatusTokenを入れ込む。
WeaponInterface.Fire()の発動

3. Weaponにて
ぶつかるなどの敵との接触
敵のStatusContainerの取得
敵のStatusContainerにStatusTokenを登録。

n. Weaponにて（WeaponInterfaceを付ければいくらでもStatusTokenの譲渡が可能）
Weaponの生成
WeaponからweaponInterfaceの取得
WeaponInterface.Init()の発動
WeaponInterface.InsertToken(StatusToken token)で自身のStatusTokenを入れ込む。
WeaponInterface.Fire()の発動

まとめると、インターフェースを用意し、生成側で初期化や後処理のタイミングを決めることが目的。
WeaponCoreでStatusTokenを生成し、武器の細部までいきわたらせる。（例：弓だと矢にStatusTokenが入っている。）
StatusTokenをパケットとみなすと、パケット通信に近いかも。

このStatusTokenの譲渡を自由にすることが、多様な武器を作ることを可能にしている。

・武器の例
爆弾：Player -> WeaponCore -> Bomb -> BombFragment
弓：Player -> WeaponCore -> Bow -> Arrow
剣：Player -> WeaponCore -> Blade
*/

/*
「コードの見方」
DummyPlayerBehaviour -> SampleWeaponCore -> SampleWeapon
この流れで見ると追いやすく、応用も思いつきやすいと思う。


「理解度チェック」
Hierarchy -> DummyPlayer -> StatusContainer(Inspector) に移動
DummyPlayerStatusMatrixのAttckのBaseの値を変えてみよう。

Prefab -> SampleWeaponCore -> StatusContainer(Inspector) に移動
SampleWeaponCoreStatusMatrixのAttackのBaseの値を変えてみよう。

これら二つの操作をすると敵のダメージの入り方が変わるはず。
ダメージはそれら二つのAttackの値の和になる。
*/
