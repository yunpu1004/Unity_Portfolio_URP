using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 이 스크립트는 대화 UI를 표시합니다. (Dialogue의 OnDialogueDataChanged 이벤트에 연결되어 있습니다.)
public class DialogueUI : MonoBehaviour
{
    public bool onDialogue = false;
    public PlayerInput input;
    public TextMeshProUGUI npcName, dialogueText;
    public Button continueButton;
    public Button[] options;
    public TextMeshProUGUI[] optionTexts;
    public GameObject content;
    public Dialogue dialogue;

    private void Awake() 
    {
        dialogue.AddOnDialogueDataChangedEvent(UpdateDialogueUI);
    }

    // 대화 데이터를 확인하고, UI를 업데이트합니다.
    private void UpdateDialogueUI(DialogueData dialogueData)
    {
        if(dialogueData == null)
        {
            input.SetCursorLock(true);
            onDialogue = false;
            content.gameObject.SetActive(false);
            return;
        }
        else
        {
            input.SetCursorLock(false);
            onDialogue = true;
            content.gameObject.SetActive(true);

            npcName.text = dialogueData.npcName;
            dialogueText.text = dialogueData.dialogueText;
            
            int optionCount = dialogueData.GetOptionCount();
            for(int i = 0; i < 4; i++)
            {
                options[i].gameObject.SetActive(i < optionCount);
                if(i < optionCount)
                {
                    optionTexts[i].text = dialogueData.GetOptionText(i);
                }
            }
        }
    }
}
