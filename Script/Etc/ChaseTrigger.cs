using System.Collections;
using UnityEngine;

// 이 스크립트는 플레이어가 몬스터의 시야에 들어왔는지를 판단합니다.
public class ChaseTrigger : MonoBehaviour
{
    public bool inSight {get; private set;} = false;
    private BoxCollider boxCollider;

    private void Awake() 
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // 플레이어가 몬스터의 시야에 들어왔을 때, 몬스터의 시야에 들어왔다는 플래그를 설정합니다.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inSight = true;
            StartCoroutine(CheckPlayerTrigger());
        }
    }

    // 0.1초마다 플레이어가 몬스터의 시야에 있는지 체크합니다.
    IEnumerator CheckPlayerTrigger()
    {
        yield return new WaitForSeconds(0.1f);
        while (Physics.CheckBox(boxCollider.bounds.center, boxCollider.bounds.extents, transform.rotation, LayerMask.GetMask("Player")))
        {
            yield return new WaitForSeconds(0.1f);
        }
        inSight = false;
    }
}
