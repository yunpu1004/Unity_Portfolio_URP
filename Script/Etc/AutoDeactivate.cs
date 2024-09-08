using UnityEngine;

// 이 스크립트는 활성화 된 오브젝트를 일정 시간이 지나면 비활성화 시킵니다.
public class AutoDeactivate : MonoBehaviour
{
    public float lifeTime = 1f;
    private float timer = 0f;

    private void OnEnable() 
    {
        timer = 0f;
    }

    private void Update() 
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }
}
