using UnityEngine;

// 이 스크립트는 활성화 된 오브젝트의 CanvasGroup 컴포넌트의 알파 값을 조정하여 페이드 효과를 줍니다.
[RequireComponent(typeof(CanvasGroup))]
public class AutoFade : MonoBehaviour
{
    public float fadeInTime = 1f;
    public float delayBeforeFadeOut = 1f;
    public float fadeOutTime = 1f;

    private CanvasGroup canvasGroup;

    private float alpha_Original;
    private float alpha_Current;
    private float timeElapsed;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        alpha_Original = canvasGroup.alpha;
    }

    private void OnEnable()
    {
        timeElapsed = 0f;
        alpha_Current = 0f;
        canvasGroup.alpha = alpha_Current;
    }

    // 페이드 효과를 줍니다.
    private void Update()
    {
        timeElapsed += Time.deltaTime;

        // 페이드 인 효과
        if(timeElapsed < fadeInTime)
        {
            float progress = timeElapsed / fadeInTime;
            float easedAlpha = Mathf.Sin(progress * Mathf.PI * 0.5f);
            alpha_Current = easedAlpha;

        }

        // 페이드 인과 페이드 아웃 사이의 딜레이는 아무것도 하지 않으므로 빈 칸으로 둡니다.
        else if(timeElapsed < fadeInTime + delayBeforeFadeOut)
        {
            
        }

        // 페이드 아웃 효과
        else if(timeElapsed < fadeInTime + delayBeforeFadeOut + fadeOutTime)
        {
            float progress = (timeElapsed - fadeInTime - delayBeforeFadeOut) / fadeOutTime;
            float easedAlpha = Mathf.Sin(progress * Mathf.PI * 0.5f);
            alpha_Current = 1f - easedAlpha;
        }

        // 페이드 아웃이 끝나면 오브젝트를 비활성화 시킵니다.
        else
        {
            alpha_Current = 0f;
            gameObject.SetActive(false);
        }

        canvasGroup.alpha = alpha_Original * alpha_Current;
    }
}