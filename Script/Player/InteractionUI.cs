using TMPro;
using UnityEngine;

// 이 스크립트는 상호작용 UI를 표시합니다. (Interaction의 OnNearestInteractionChanged 이벤트에 연결되어 있습니다.)
public class InteractionUI : MonoBehaviour
{
    public bool interactable = false;
    public TextMeshProUGUI interactionText;
    public Interaction nearInteraction;
    public GameObject content;

    private void Awake() 
    {
        Interaction.AddOnNearestInteractionChangedEvent(UpdateInteractionUI);
    }

    // 플레이어와 가장 가까운 Interaction 객체를 확인하고, UI를 업데이트합니다.
    private void UpdateInteractionUI(Interaction nearestInteraction)
    {
        // 주변에 상호 작용이 없으면 UI를 숨깁니다.
        if(nearestInteraction == null)
        {
            interactable = false;
            content.gameObject.SetActive(false);
            interactionText.text = "";
            nearInteraction = null;
        }

        // 주변에 상호 작용이 있으면 UI를 표시합니다.
        else
        {
            nearInteraction = nearestInteraction;
            interactable = true;
            content.gameObject.SetActive(true);
            interactionText.text = nearInteraction.GetInteractionText();
        }
    }
}