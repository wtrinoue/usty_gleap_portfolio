using System.Collections.Generic;
using UnityEngine;

public class CurlingGraveCoreBehaviour : MonoBehaviour
{
    public GameObject curlingGravePrefab;

    public int graveCount;
    private float offsetAngle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float angleInterval = 360f / graveCount;
        for (int i = 0; i < graveCount; i++)
        {
            GenerateCurlingGrave(offsetAngle + angleInterval * i);
        }
        Destroy(gameObject);
    }

    private void GenerateCurlingGrave(float angle)
    {
        GameObject curlingGrave = Instantiate(
            curlingGravePrefab,
            transform.position,
            transform.rotation
        );

        CurlingGraveBehaviour cgb = curlingGrave.GetComponent<CurlingGraveBehaviour>();

        cgb.SetAngle(angle);
    }
}
