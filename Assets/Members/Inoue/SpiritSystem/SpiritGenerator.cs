using UnityEngine;
[RequireComponent(typeof(StatusContainer))]
public class SpiritGenerator : MonoBehaviour
{
    public GameObject spiritOrbPrefab;
    private StatusContainer sc;
    private bool isGenerated = false;
    void Start()
    {
        SetToken();
    }

    private void SetToken()
    {
        sc = GetComponent<StatusContainer>();
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
        isGenerated = true;
    }
}
