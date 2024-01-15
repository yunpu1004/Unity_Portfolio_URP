using UnityEngine;
using System;

// 이 스크립트는 플레이어의 행동을 기록하고, 플레이어의 행동이 변경될 때마다 이벤트를 발생시킵니다.
// 퀘스트의 진행 상황을 기록하거나, 플레이어의 행동을 기록하는 등의 기능을 구현할때 사용할 수 있습니다.
public class PlayerActivity : MonoBehaviour
{
    private PlayerActivityData recentActivity;
    private Action<PlayerActivityData> onPlayerActivityChanged;

    public PlayerActivityData GetRecentActivity() => recentActivity;

    // 플레이어의 행동을 기록하고 이벤트를 발생시킵니다.
    public void SetRecentActivity(PlayerActivityData playerActivityData)
    {
        recentActivity = playerActivityData;
        onPlayerActivityChanged?.Invoke(recentActivity);
    }

    public void AddOnPlayerActivityChangedEvent(Action<PlayerActivityData> action)
    {
        onPlayerActivityChanged += action;
    }
}

// 플레이어의 행동을 기록하는 구조체
public struct PlayerActivityData
{
    public ActivityType activityType;
    public string activityTarget;
    
    public enum ActivityType
    {
        Kill,
        Item,
        LevelUp,
        Skill,
        Craft,
        Trade,
        Other
    }
}
