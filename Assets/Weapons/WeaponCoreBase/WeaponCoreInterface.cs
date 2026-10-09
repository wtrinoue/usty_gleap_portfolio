public interface WeaponCoreInterface
{
    public void Init();
    public void ExtractPlayerStatus(in StatusVector s);
    public void Fire();
}

/*
WeaponCoreInterfaceはPlayerActionにおいて、Inite, ExtractPlayerStatus, Fireの順で実行される。
※おそらく実装すると以下のようになる。

public class UnknownBehaviour : MonoBehaviour, WeaponCoreInterface
{
    public StatusContainer statusContainer;
    private PackToken packToken;

    public void Init()
    {
        centerObject = gameObject;
        if (centerObject != null)
        {
            transform.position = centerObject.transform.position;
            transform.SetParent(centerObject.transform);
        }

        packToken = new();
        packToken.AddStatusToken(new DamageToken()); // ここでTokenをいくらでも追加できる。
        packToken.ExtractStatus(statusContainer.GetStatus());
    }

    public void ExtractPlayerStatus(in StatusVector s)
    {
        packToken.ExtractStatus(s);
    }

    public void Fire()
    {
        Spawn();
    }
}
*/
