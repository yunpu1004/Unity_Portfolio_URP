using System.Collections;
using Cinemachine;
using UnityEngine;

// 이 스크립트는 게임이 끝날 때 나오는 카메라 연출과 UI 이벤트를 관리합니다.
public class EndingEffectManager : MonoBehaviour
{
    public static EndingEffectManager instance;

    public float fadeDuration = 1.0f;
    public float cameraDuration = 5.0f;

    public Player player;
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera virtualCamera1;
    public CinemachineVirtualCamera virtualCamera2;
    public CinemachineVirtualCamera virtualCamera3;


    private void Awake()
    {
        instance = this;
    }

    public void StartEndingEffect()
    {
        StartCoroutine(StartEndingEffectCoroutine());
    }

    // 게임을 클리어했을 때 시작되는 연출입니다.
    // 미리 배치된 가상 카메라를 페이드 효과를 사용하여 전환하며 장면을 만듭니다.
    IEnumerator StartEndingEffectCoroutine()
    {
        player.enabled = false;
        var canvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
        {
            canvas.enabled = false;
        }
        canvas.enabled = true;

        yield return FadeOut();

        playerCamera.gameObject.SetActive(false);
        virtualCamera1.gameObject.SetActive(true);

        yield return FadeIn();
        yield return new WaitForSeconds(cameraDuration);
        yield return FadeOut();

        virtualCamera1.gameObject.SetActive(false);
        virtualCamera2.gameObject.SetActive(true);
        
        yield return FadeIn();
        yield return new WaitForSeconds(cameraDuration);
        yield return FadeOut();

        virtualCamera2.gameObject.SetActive(false);
        virtualCamera3.gameObject.SetActive(true);

        yield return FadeIn();
        yield return new WaitForSeconds(cameraDuration);
        
        foreach (Transform child in canvas.transform)
        {
            child.gameObject.SetActive(true);
        }

        yield return FadeOut();

        Cursor.lockState = CursorLockMode.None;
    }

    // 화면을 서서히 어둡게 합니다.
    IEnumerator FadeOut()
    {
        float timer = 0.0f;

        while (timer < fadeDuration / 2)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / (fadeDuration / 2));
            yield return null;
        }
    }

    // 화면을 서서히 밝게 합니다.
    IEnumerator FadeIn()
    {
        float timer = 0.0f;
        
        while (timer < fadeDuration / 2)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / (fadeDuration / 2));
            yield return null;
        }
    }

    // 게임을 종료합니다.
    public void QuitGame()
    {
        Application.Quit();
    }
}