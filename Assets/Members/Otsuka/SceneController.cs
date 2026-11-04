using UnityEngine;
using UnityEngine.SceneManagement; // これが必要！

public class TitleScreen : MonoBehaviour
    
{
    [SerializeField] private string nextSceneName;
    public void OnStartButtonClick()
    {
        // ビルド設定に追加したシーンのファイル名を文字列で指定
        SceneManager.LoadScene(nextSceneName);
    }
}