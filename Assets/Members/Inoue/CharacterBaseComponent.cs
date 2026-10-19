using UnityEngine;
using UnityEngine.Rendering;
[RequireComponent(typeof(StatusContainer))]
[RequireComponent(typeof(HpBarGenerator))]
[RequireComponent(typeof(EffectGenerator))]
[RequireComponent(typeof(SpiritGenerator))]

public class CharacterBaseComponent : MonoBehaviour
{ }

/*
※以下のクラス構成をデフォルトで書くと扱いやすい。
HPBarGeneratorとEffectGeneratorは外部から操作されることが多いので、自分ではあまり触れない。

using UnityEngine;

public class Unknown : MonoBehaviour
{
    public StatusContainer statusContainer;

    // オブジェクト生成時に呼ばれる関数
    void Start() { }
    // 毎フレーム呼ばれる関数
    void Update() {}
}
*/
