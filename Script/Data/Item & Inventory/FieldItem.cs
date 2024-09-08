using UnityEngine;

// 이 스크립트는 필드 아이템의 동작과 상호작용을 담당합니다.
public class FieldItem : MonoBehaviour
{
    // 아이템의 id를 키로 사용하여 프리팹을 저장합니다.
    private static GameObject fieldItems;
    private Interaction interaction;
    private Inventory inventory;

    public Item item;    
    float timeElapsed;

    private void Awake() 
    {
        fieldItems = GameObject.Find("FieldItems");
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
        interaction = GetComponent<Interaction>();

        timeElapsed = 0;
        name = transform.name.Replace("(Clone)", "");
        interaction.SetInteraction(name + " 획득", Pickup);
    }

    // 필드 아이템의 움직임을 실행합니다.
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > 300) 
        {
            Destroy(gameObject);
        }
        else
        {
            /// 0.2 ~ 0.4 사이의 값을 갖도록 사인 함수를 사용 (주기는 4초)
            float y = 0.1f * Mathf.Sin(2 * Mathf.PI * timeElapsed / 4) + 0.3f;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);

            /// 6초 주기로 y축을 기준으로 회전
            transform.Rotate(Vector3.up, 360 * Time.deltaTime / 6);
        }
    }

    // 지정된 위치에 아이템을 생성합니다.
    public static GameObject CreateFieldItem(Item item, Vector3 position)
    {
        var itemPrefabDict = DataManager.instance.GetItemPrefab(item.id);
        if(itemPrefabDict != null) 
        {
            var go = Instantiate(itemPrefabDict, position, Quaternion.identity);
            go.name = go.name.Replace("(Clone)", "");
            go.transform.SetParent(fieldItems.transform);
            go.SetActive(true);
            go.GetComponent<FieldItem>().item = item;
            return go;
        }
        else 
        {
            Debug.LogError($"아이템 프리팹을 찾을 수 없습니다. {item}");
            return null;
        }
    }

    // 아이템을 인벤토리에 추가하고, 필드 아이템을 파괴합니다.
    public void Pickup()
    {
        inventory.AddItem(item);
        Destroy(gameObject);
    }
}
