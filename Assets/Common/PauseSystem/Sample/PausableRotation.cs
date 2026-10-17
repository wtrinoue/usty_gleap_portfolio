using UnityEngine;

public class PausableRotation : MonoBehaviour, IPausable
{
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 90f);

    // ここで書かれた処理は、PauseRegisterによりPauseManagerに登録され、実行される。
    public void OnCustomUpdate()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
