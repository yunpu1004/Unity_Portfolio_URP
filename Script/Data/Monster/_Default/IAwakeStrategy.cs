using UnityEngine;

// 이 인터페이스는 Awake 시점의 초기화 전략을 나타냅니다.
public interface IAwakeStrategy
{
    void OnAwake(GameObject gameObject);
}