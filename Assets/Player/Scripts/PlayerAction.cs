using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    private WeaponCoreHolder weaponCoreHolder;
    private List<GameObject> weaponCoreList;
    private GraveHolder graveHolder;
    private StatusContainer statusContainer;
    private PlayerMove playerMove;
    private List<GameObject> graves;
    private int currentWeaponCoreIndex = 0;
    private InputAction fireAction;
    private InputAction rightClickAction;
    private InputAction scrollAction;
    private InputAction spaceAction;
    private bool isDestroyed = false;
    private bool isInitialized = false;
    void Start()
    {
        weaponCoreHolder = gameObject.GetComponent<WeaponCoreHolder>();
        weaponCoreList = weaponCoreHolder.GetWeaponCoreList();
        graveHolder = gameObject.GetComponent<GraveHolder>();
        statusContainer = GetComponent<StatusContainer>();
        playerMove = GetComponent<PlayerMove>();
        graves = graveHolder.GetGraves();

        // 攻撃用 InputAction(Button)
        fireAction = new InputAction(
            name: "Fire",
            type: InputActionType.Button,
            binding: "<Mouse>/leftButton" // マウス左クリック
        );

        fireAction.performed += OnFire;

        // 右クリック用 InputAction(Button)
        rightClickAction = new InputAction(
            name: "RightClick",
            type: InputActionType.Button,
            binding: "<Mouse>/rightButton"
        );

        rightClickAction.performed += OnRightClick;

        // スクロール用 InputAction(Value)
        scrollAction = new InputAction(
            name: "Scroll",
            type: InputActionType.Value,
            binding: "<Mouse>/scroll"
        );

        scrollAction.performed += OnScroll;

        // スペースキー用 InputAction(Button)
        spaceAction = new InputAction(
            name: "Space",
            type: InputActionType.Button,
            binding: "<Keyboard>/space"
        );

        spaceAction.performed += OnSpace;

        fireAction.Enable();
        rightClickAction.Enable();
        scrollAction.Enable();
        spaceAction.Enable();
        isInitialized = true;

        // MagicをHP以下に抑える
        LimitMagicByHPToken lmbht = new();
        lmbht.ExtractStatus(statusContainer.GetStatus());
        statusContainer.ApplyEternalToken(lmbht);

        // Magicを初めは0にする。
        ResetToken rt = new(StatusCategory.Magic);
        statusContainer.ApplyOneTimeToken(rt);

    }
    void OnDisable()
    {
        fireAction.performed -= OnFire;
        rightClickAction.performed -= OnRightClick;
        scrollAction.performed -= OnScroll;
        spaceAction.performed -= OnSpace;

        fireAction.Disable();
        rightClickAction.Disable();
        scrollAction.Disable();
        spaceAction.Disable();
    }


    public void OnFire(InputAction.CallbackContext context)
    {
        if (!isInitialized) return;
        if (isDestroyed)
        {
            Debug.Log("破壊されています");
            return;
        }
        // クリックが押されたときだけ処理
        if (!context.performed)
        {
            Debug.Log("クリックではありません");
            return;
        }
        // 武器がなければ処理しない
        weaponCoreList ??= weaponCoreHolder.GetWeaponCoreList();
        if (weaponCoreList.Count == 0) return;
        Debug.Log("クリックが動作します");

        // 自分→マウス方向のベクトル
        Vector3 direction = GetMouseDirection();

        GameObject clone = Instantiate(weaponCoreList[currentWeaponCoreIndex], transform.position, Quaternion.identity);
        clone.transform.SetParent(transform);
        clone.transform.up = direction; // 2Dでは上方向をベクトルに合わせる

        // WeaponCoreのセットアップを生成側でする。
        WeaponCoreInterface wci = clone.GetComponent<WeaponCoreInterface>();
        wci.Init();
        wci.ExtractPlayerStatus(statusContainer.GetStatus());
        wci.Fire();
    }
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!isInitialized) return;
        Debug.Log($"graveHolder: {graveHolder}, graves: {graves}");
        if (isDestroyed)
        {
            Debug.Log("破壊されています");
            return;
        }
        if (!context.performed)
        {
            Debug.Log("スペースキーではありません");
            return;
        }
        // graves と graveHolder が初期化されているか確認
        if (graves == null)
        {
            graves = graveHolder.GetGraves();
        }
        // MagicがHPを上回っているかチェック
        if (EnableGenerateGrave())
        {
            // お墓を生成
            GenerateGrave();

            // ゴーストエスケープ
            playerMove.GhostEscape(GetMouseDirection());
        }

    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        if (!isInitialized) return;
        if (isDestroyed)
        {
            Debug.Log("破壊されています");
            return;
        }
        // 武器がなければ処理しない
        if (weaponCoreList == null || weaponCoreList.Count == 0)
        {
            weaponCoreList = weaponCoreHolder.GetWeaponCoreList();
        }
        Debug.Log("スクロールが動作します");
        Vector2 scroll = context.ReadValue<Vector2>();

        if (scroll.y > 0)
        {
            currentWeaponCoreIndex++;
        }
        else if (scroll.y < 0)
        {
            currentWeaponCoreIndex--;
        }

        // 範囲制限 or ループ
        if (currentWeaponCoreIndex >= weaponCoreList.Count)
            currentWeaponCoreIndex = 0;
        if (currentWeaponCoreIndex < 0)
            currentWeaponCoreIndex = weaponCoreList.Count - 1;
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (!isInitialized) return;
        Debug.Log($"graveHolder: {graveHolder}, graves: {graves}");
        if (isDestroyed)
        {
            Debug.Log("破壊されています");
            return;
        }
        if (!context.performed)
        {
            Debug.Log("スペースキーではありません");
            return;
        }
        // graves と graveHolder が初期化されているか確認
        if (graves == null)
        {
            graves = graveHolder.GetGraves();
        }
        Debug.Log("スペースが動作します");
        if (graves.Count == 0)
        {
            DestroyMe();
            return;
        }
        // お墓を生成
        GenerateGrave();

        // 自分を破壊
        DestroyMe();
    }

    public void DestroyMe()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        StartCoroutine(DestroyNextFrame());
    }

    IEnumerator DestroyNextFrame()
    {
        yield return null; // 1フレーム待つ
        Destroy(gameObject);
    }
    private void GenerateGrave()
    {
        // 自分の位置に生成
        GameObject grave = Instantiate(graves[0], transform.position, Quaternion.identity);
        // HpToMagicTokenを生成する。
        HpToMagicToken htmt = new();
        htmt.ExtractStatus(statusContainer.GetStatus());
        // MagicTokenをGraveに流す
        WeaponInterface wi = grave.GetComponent<WeaponInterface>();
        wi.Init();
        wi.InsertToken(htmt);
        wi.Fire();
        // MagicをHPにする
        CopyToken copyToken = new(StatusCategory.Magic, StatusCategory.HP);
        statusContainer.ApplyOneTimeToken(copyToken);
        // 自分のMagicをリセットする。
        ResetToken resetToken = new(StatusCategory.Magic);
        statusContainer.ApplyOneTimeToken(resetToken);
        // お墓を削除する。
        graveHolder.RemoveFirstGrave();
    }

    private Vector3 GetMouseDirection()
    {
        // マウス座標をスクリーン空間で取得
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // スクリーン座標 → ワールド座標
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // Z座標は2Dなら0に固定
        mouseWorldPos.z = 0f;

        // 自分（このオブジェクト）の位置
        Vector3 myPos = transform.position;

        // 自分→マウス方向のベクトル
        Vector3 direction = -(mouseWorldPos - myPos).normalized;

        return direction;
    }

    private bool EnableGenerateGrave()
    {
        float currentHP = statusContainer.statusVector.Get(StatusCategory.HP, StatusMethod.Base);
        float currentMagic = statusContainer.statusVector.Get(StatusCategory.Magic, StatusMethod.Base);
        bool enable = currentMagic >= currentHP && graves.Count > 0;
        return enable;
    }
}
