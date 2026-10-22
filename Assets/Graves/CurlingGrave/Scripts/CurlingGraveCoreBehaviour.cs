using System.Collections.Generic;
using UnityEngine;

public class CurlingGraveCoreBehaviour : MonoBehaviour, WeaponInterface
{
    public GameObject curlingGravePrefab;

    public int graveCount;
    private float offsetAngle;
    private StatusToken magicToken;

    public void Init() { }

    public void InsertToken(StatusToken token)
    {
        magicToken = token;
    }

    public void Fire()
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

        WeaponInterface wi = curlingGrave.GetComponent<WeaponInterface>();

        wi.Init();
        wi.InsertToken(magicToken);
        wi.Fire();
    }
}
