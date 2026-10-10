using UnityEngine;


// WeaponCoreにはWeaponCoreInterfaceを付けて、PlayerのStatusを入れてもらう。
public class SampleWeaponCoreBehaviour : MonoBehaviour, WeaponCoreInterface
{
    public GameObject sampleWeaponPrefab;
    private StatusContainer statusContainer;
    private PackToken packToken;
    public void Init()
    {
        statusContainer = GetComponent<StatusContainer>();// WeaponCoreのStatusContainerを取得。これで武器ごとに攻撃力が分けられる。
        packToken = new();
        packToken.AddStatusToken(new DamageToken());// ここでいろいろなTokenを入れる。※PackTokenの特性
        packToken.ExtractStatus(statusContainer.GetStatus());
    }
    public void ExtractPlayerStatus(in StatusVector s)
    {
        packToken.ExtractStatus(s);// PlayerからStatusを入れてもらう。
    }
    public void Fire()
    {
        // 今回は弾丸の生成
        GameObject sampleWeapon = Instantiate(
            sampleWeaponPrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, 90f)
        );

        // WeaponInterfaceからStatusTokenをセットする。
        WeaponInterface wi = sampleWeapon.GetComponent<WeaponInterface>();
        wi.Init();
        wi.InsertToken(packToken);
        wi.Fire();

        // 生成したらもういらないので自己破壊
        Destroy(gameObject);
    }
}
