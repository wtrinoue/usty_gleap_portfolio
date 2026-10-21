using UnityEngine;
[RequireComponent(typeof(StatusContainer))]
[RequireComponent(typeof(Collider2D))]
public class SpiritGenerator : MonoBehaviour
{
    public GameObject spiritOrbPrefab;
    private StatusContainer sc;
    private Collider2D col;
    private bool isGenerated = false;
    void Start()
    {
        SetToken();
    }

    private void SetToken()
    {
        sc = GetComponent<StatusContainer>();
        col = GetComponent<Collider2D>();
        DeadToken deadToken = new();
        deadToken.SetAction(
            () => { GenerateSpiritOrb(); }
        );
        sc.ApplyEternalToken(deadToken);
    }

    private void GenerateSpiritOrb()
    {
        if (isGenerated) return;
        GameObject spiritOrb = Instantiate(spiritOrbPrefab, transform.position, transform.rotation);
        WeaponCoreInterface wci = spiritOrb.GetComponent<WeaponCoreInterface>();
        wci.Init();
        wci.ExtractPlayerStatus(sc.GetStatus());
        wci.Fire();
        col.isTrigger = true;
        isGenerated = true;
    }
}
