using UnityEngine;
using UnityEngine.SceneManagement;

// 이 스크립트는 메인 메뉴 씬을 관리합니다.
public class MenuSceneManager : MonoBehaviour
{
    public static MenuSceneManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // 메인 씬을 불러옵니다.
    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    // 게임을 종료합니다.
    public void QuitGame()
    {
        Application.Quit();
    }
}