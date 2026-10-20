using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private StatusContainer _statusContainer;
    [SerializeField] private Slider _statusSlider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private StatusCategory statusCategory;
    [SerializeField] private Color slideColor;
    private bool IsDestroyed = false;
    private float maxParam = 0f;


    private void Update()
    {
        // Debug.Log("まだ生きています");
        if (_statusContainer == null || _statusSlider == null)
        {
            DestroyBar();
            return;
        }
        if (IsDestroyed) return;

        float currentParam = _statusContainer.GetStatus().Get(statusCategory, StatusMethod.Base);

        // MaxHPが0の場合は0を設定
        _statusSlider.value = maxParam > 0 ? currentParam / maxParam : 0;

        // HPバーの向きを固定
        // transform.LookAt(Camera.main.transform);
        transform.rotation = Camera.main.transform.rotation;
        transform.position = target.position + offset;
        // transform.Rotate(0, 180, 0);
    }
    public void SetStatusCategory(StatusCategory sc)
    {
        statusCategory = sc;
    }
    public void SetColor(Color color)
    {
        slideColor = color;
    }
    public void SetTarget(Transform t)
    {
        target = t;
    }

    public void SetOffset(float height)
    {
        offset = new Vector3(0f, height, -3f);
    }

    public void Initialize()
    {
        _statusContainer = target.GetComponentInParent<StatusContainer>(); //親コンポーネントのStatusContainerを取得
        // 変化すゲージの色を調整
        Image fillImage = _statusSlider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            fillImage.color = slideColor;
        }
        maxParam = _statusContainer.statusVector.Get(StatusCategory.Magic, StatusMethod.Base);
        Debug.Log(maxParam);
    }

    private void DestroyBar()
    {
        Destroy(gameObject);
        IsDestroyed = true;
    }
}
