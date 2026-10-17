using UnityEngine;

public class PausableRotation : MonoBehaviour, IPausable
{
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 90f);

    public void OnCustomUpdate()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
