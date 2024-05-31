using UnityEngine;

// 이 스크립트는 플레이어가 몬스터의 시야에 들어왔는지를 판단합니다.
public class ChaseTrigger : MonoBehaviour
{
    private bool inSight = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inSight = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inSight = false;
        }
    }

    public bool IsInSight()
    {
        return inSight;
    }
}
