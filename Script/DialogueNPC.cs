using UnityEngine;

// 이 스트립트는 DialogueNPC의 Interaction에 대화 이벤트를 등록합니다.
public class DialogueNPC : MonoBehaviour
{
    public string firstDialogueId;
    public Interaction interaction;
    public Dialogue dialogue;

    private void Awake() 
    {
        var dialogueData = DataLoader.instance.GetDialogueData(firstDialogueId);
        interaction.SetInteraction(name, () => dialogue.SetDialogue(dialogueData));
    }
}