using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트는 플레이어가 상호작용을 요청할 시, 등록된 interactionAction을 실행합니다.
// 또한 옵저버 패턴을 사용하여 가장 가까운 상호작용을 찾을 때마다 이벤트를 발생시킵니다.
public class Interaction : MonoBehaviour
{
    private string interactionText;
    private Action interactionAction;
    private SphereCollider sphereCollider;

    private static List<Interaction> nearInteractions = new List<Interaction>();
    private static Interaction nearestInteraction = null;
    private static Transform player;
    private static Action<Interaction> OnNearestInteractionChanged;
    public string GetInteractionText() => interactionText;
    public Action GetInteractionAction() => interactionAction;

    private void Awake() 
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    // 상호작용 객체의 텍스트와 액션을 설정합니다.
    public void SetInteraction(string text, Action action)
    {
        interactionText = text;
        interactionAction = action;
    }

    // 상호작용을 실행합니다.
    public void ExecuteInteraction()
    {
        interactionAction();
    }

    // 플레이어가 상호작용 범위에 들어왔을 때, 가장 가까운 상호작용을 찾습니다.
    // other가 아니라 가장 가까운 상호작용 객체를 찾는 이유는, 플레이어가 두개 이상의 상호작용 객체에 동시에 접근할 수 있기 때문입니다.
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StartCoroutine(CheckPlayerTrigger());
            nearInteractions.Add(this);
            GetNearestInteraction();
        }
    }

    // 플레이어가 상호작용 범위에서 나갔을 때, 가장 가까운 상호작용을 찾아 갱신합니다.
    private void OnDestroy() 
    {
        nearInteractions.Remove(this);
        GetNearestInteraction();
    }


    // 플레이어 주변에 있는 상호작용 객체 중 가장 가까운 객체를 찾습니다.
    private static Interaction GetNearestInteraction()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        Vector3 playerPos = player.position;

        // 상호작용 객체가 파괴되는 경우가 있으므로, null인 객체를 제거합니다. (ex. 필드 아이템)
        for(int i = 0; i < nearInteractions.Count; i++)
        {
            if(nearInteractions[i] == null)
            {
                nearInteractions.RemoveAt(i);
                i--;
            }
        }

        // 주변에 상호작용이 없으면 가장 가까운 상호작용을 null로 설정합니다.
        if(nearInteractions.Count == 0)
        {
            nearestInteraction = null;
            OnNearestInteractionChanged?.Invoke(nearestInteraction);
            return null;
        }

        // 주변에 상호작용이 한 개 이상 있으면 가장 가까운 상호작용을 찾아서 설정합니다.
        else
        {
            nearestInteraction = nearInteractions[0];
            float nearestDistance = Vector3.Distance(playerPos, nearestInteraction.transform.position);
            for(int i = 1; i < nearInteractions.Count; i++)
            {
                float distance = Vector3.Distance(playerPos, nearInteractions[i].transform.position);
                if(distance < nearestDistance)
                {
                    nearestInteraction = nearInteractions[i];
                    nearestDistance = distance;
                }
            }

            OnNearestInteractionChanged?.Invoke(nearestInteraction);
            return nearestInteraction;
        }
    }

    // 가장 가까운 상호작용을 실행합니다.
    public static void ExecuteNearestInteraction()
    {
        if(nearestInteraction == null) return;
        nearestInteraction.ExecuteInteraction();

        // 상호작용 결과 해당 객체가 파괴되는 경우가 있으므로, 다시 가장 가까운 상호작용을 찾습니다. (ex. 필드 아이템)
        GetNearestInteraction();
    }

    public static void AddOnNearestInteractionChangedEvent(Action<Interaction> listener)
    {
        OnNearestInteractionChanged += listener;
    }

    private IEnumerator CheckPlayerTrigger()
    {
        yield return new WaitForSeconds(0.1f);

        while(Physics.OverlapSphere(transform.position, sphereCollider.radius * 1.1f, 1 << LayerMask.NameToLayer("Player")).Length > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        nearInteractions.Remove(this);
        GetNearestInteraction();     
    }
}