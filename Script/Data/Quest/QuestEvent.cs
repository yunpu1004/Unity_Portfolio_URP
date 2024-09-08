using UnityEngine;

// 이 클래스는 퀘스트 수락, 완성시 발생하는 이벤트를 관리합니다.
// QuestCSV 파일에 작성된 "QuestEvent.GameClear"와 같은 문자열을 읽고 리플렉션을 통해 해당 메서드를 호출하는 방식으로 사용합니다.
public static class QuestEvent
{
    private static GameObject _monsters;
    private static GameObject monsters 
    {
        get
        {
            if(_monsters == null)
            {
                _monsters = GameObject.Find("Monsters");
            }
            return _monsters;
        }
    }

    // 몬스터를 소환합니다.
    public static void SpawnEnemy(string enemyName)
    {
        GameObject enemy = monsters.transform.Find(enemyName).gameObject;
        enemy.SetActive(true);
    }

    // 게임 클리어 이벤트를 발생시킵니다.
    public static void GameClear()
    {
        EndingEffectManager.instance.StartEndingEffect();
    }
}
