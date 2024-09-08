using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 이 스크립트는 플레이어 스테이터스 슬롯을 표시합니다. (무기와 방패를 변경할때 사용됩니다.)
public class PlayerStatSlot : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    private event System.Action onDoubleClick;

    // 슬롯을 업데이트합니다.
    public void UpdateSlot(Item item)
    {
        if(!item.IsEmpty())
        {
            gameObject.SetActive(true);
            image.sprite = item.sprite;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void AddOnDoubleClick(System.Action action)
    {
        onDoubleClick += action;
    }

    // 슬롯을 더블 클릭하면 이벤트를 발생시킵니다.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) // 더블 클릭 감지
        {
            onDoubleClick?.Invoke();
        }
    }
}
