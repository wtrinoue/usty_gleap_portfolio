using UnityEngine;

[RequireComponent(typeof(StatusContainer))]
public class SpiritConsumer : MonoBehaviour, WeaponInterface
{
    public ModifierDefinition modifierDefinition;
    private StatusContainer statusContainer;
    private Modifier modifier;
    private StatusToken magicToken;

    void Start()
    {
        Debug();
    }
    private void Debug()
    {
        Init();
        InsertToken(new MagicToken());
        Fire();
    }

    public void Init()
    {
        statusContainer = GetComponent<StatusContainer>();
        modifier = new Modifier(modifierDefinition, gameObject);
    }

    public void InsertToken(StatusToken token)
    {
        magicToken = token;
    }

    public void Fire()
    {
        // Magicの数値が0になったときの検知用
        CustomToken customToken = new CustomToken((StatusVector statusVector) =>
        {
            float currentMagic = statusVector.Get(StatusCategory.Magic, StatusMethod.Base);
            if (currentMagic <= 0f) Destroy(gameObject);
        });
        statusContainer.ApplyOneTimeToken(magicToken);// Magicの補給
        statusContainer.ApplyEternalToken(customToken);// Magicの検知
        statusContainer.AddModifier(modifier);// Magicの消費
    }
}
