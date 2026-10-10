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
        packToken = new();
        packToken.AddStatusToken(new DamageToken()); // ここでTokenをいくらでも追加できる。
        packToken.ExtractStatus(statusContainer.GetStatus());// WeaponCore自体のStatusをここで吸収させる。
    }

    public void ExtractPlayerStatus(in StatusVector s)
    {
        packToken.ExtractStatus(s);// Player側で発火してもらうようにする。
    }

    public void Fire()
    {
        Spawn();
    }
}
*/
