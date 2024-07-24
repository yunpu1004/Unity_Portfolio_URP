using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 메뉴 UI를 표시합니다.
// 레벨과 경험치를 표시하기 위해서 PlayerStat의 이벤트에 연결되어 있습니다.
public class MenuUI : MonoBehaviour
{
    private PlayerInput input;
    private PlayerStat playerStat;

    public bool onTapMenu { get; private set;} = false; // 메뉴가 열려있는지 확인합니다.
    public bool onTapMenuProgress { get; private set;} = false; // 메뉴가 열리거나 닫히는 중인지 확인합니다.


    [Header("탭 메뉴")]
    public CanvasGroup tapMenu;
    public RectTransform tapMenuNav;
    public RectTransform tapMenuPanel;
    public TextMeshProUGUI tapMenuLevelText;
    public TextMeshProUGUI tapMenuExpText;
    public Image tapMenuExpBar;
    private float tapMenuProgress = 0;


    [Header("콘텍스트 메뉴 (인벤토리 & 장비)")]
    public GameObject background;
    public InventoryUI inventoryUI;
    public PlayerStatUI equipmentUI;    

    public Button testButton; // 기능 테스트용 버튼


    private void Awake() {
        input = FindObjectOfType<PlayerInput>();
        playerStat = FindObjectOfType<PlayerStat>();

        playerStat.OnLevelUpEvent += SetLevelText;
        playerStat.OnExpChangedEvent += SetExpText;
    }

    // 메뉴의 레벨 텍스트를 수정합니다.
    private void SetLevelText(int level)
    {
        tapMenuLevelText.text = "Lv." + level;
    }

    // 메뉴의 경험치 텍스트를 수정합니다.
    private void SetExpText(int exp, int maxExp)
    {
        tapMenuExpText.text = "Exp : " + exp + " / " + maxExp;
        tapMenuExpBar.fillAmount = (float)exp / maxExp;
    }

    // 메뉴를 엽니다.
    public void ShowTapMenu()
    {
        if(onTapMenuProgress) return;
        tapMenu.gameObject.SetActive(true);
        onTapMenu = true;
        onTapMenuProgress = true;
        tapMenu.interactable = false;
        input.SetCursorLock(false);
    }

    // 메뉴를 닫습니다.
    public void HideTapMenu()
    {
        if(onTapMenuProgress) return;
        onTapMenu = false;
        onTapMenuProgress = true;
        tapMenu.interactable = false;
        input.SetCursorLock(true);
    }

    // 메뉴를 열거나 닫는 애니메이션을 재생합니다.
    public void ProgressTapMenu()
    {
        if(!onTapMenuProgress) return;

        if(onTapMenu)
        {
            tapMenuProgress += Time.deltaTime * 20;
            if(tapMenuProgress >= 1)
            {
                tapMenuProgress = 1;
                onTapMenuProgress = false;
            }
        }
        else
        {
            tapMenuProgress -= Time.deltaTime * 20;
            if(tapMenuProgress <= 0)
            {
                tapMenuProgress = 0;
                onTapMenuProgress = false;
            }
        }

        var panelPos = tapMenuPanel.anchoredPosition;
        panelPos.x = Mathf.Lerp(-250, 350, tapMenuProgress);
        tapMenuPanel.anchoredPosition = panelPos;

        var navPos = tapMenuNav.anchoredPosition;
        navPos.x = Mathf.Lerp(-50, 50, tapMenuProgress);
        tapMenuNav.anchoredPosition = navPos;

        if(!onTapMenuProgress) tapMenu.interactable = true;
        if(!onTapMenu && !onTapMenuProgress) tapMenu.gameObject.SetActive(false);
    }

    // 콘텍스트 메뉴를 엽니다.
    public void ShowContext()
    {
        background.SetActive(true);
        inventoryUI.OpenInventory();
        equipmentUI.OpenPlayerStat();
    }

    // 콘텍스트 메뉴를 닫습니다.
    public void HideContext()
    {
        background.SetActive(false);
        inventoryUI.CloseInventory();
        equipmentUI.ClosePlayerStat();
    }

    // 게임을 종료합니다.
    public void ExitApplication()
    {
        Application.Quit();
    }
}
