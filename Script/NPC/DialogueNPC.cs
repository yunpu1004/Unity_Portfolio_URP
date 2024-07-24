using UnityEngine;

// 이 스트립트는 DialogueNPC의 Interaction에 대화 이벤트를 등록합니다.
public class DialogueNPC : MonoBehaviour
{
    public string firstDialogueId;
    public string[] questIdArray { get; private set; }
    private Interaction interaction;

    private void Awake() 
    {
        interaction = GetComponent<Interaction>();
        var dialogueData = DataManager.instance.GetDialogueData(firstDialogueId);
        interaction.SetInteraction(name, () => Dialogue.instance.SetDialogue(dialogueData));
    }

    private void Start() 
    {
        questIdArray = DataManager.instance.GetNpcQuestIds(name); 
    }
}