using UnityEngine;
using UnityEngine.Rendering;
[RequireComponent(typeof(StatusContainer))]
[RequireComponent(typeof(HpBarGenerator))]
[RequireComponent(typeof(EffectGenerator))]

public class CharacterBaseComponent : MonoBehaviour
{ }

/*
※以下のクラス構成をデフォルトで書くと扱いやすい。

public class Unknown : MonoBehaviour
{
    public StatusContainer statusContainer;
    public EffectGenerator effectGenerator;

    // オブジェクト生成時に呼ばれる関数
    void Start() { }
    // 毎フレーム呼ばれる関数
    void Update() {}
}
*/
