using UnityEngine;

public class StatusBarGenerator : MonoBehaviour
{
    public GameObject statusBarPrefab;
    public float height = 1f;
    public StatusCategory statusCategory;
    public Color color;
    private StatusBar statusBar;

    void Start()
    {
        GameObject statusBarObject = Instantiate(statusBarPrefab);
        statusBar = statusBarObject.GetComponent<StatusBar>();
        statusBar.SetColor(color);
        statusBar.SetStatusCategory(statusCategory);
        statusBar.SetTarget(transform);
        statusBar.SetOffset(height);
        statusBar.Initialize();
        statusBarObject.transform.SetParent(transform);
    }
}
