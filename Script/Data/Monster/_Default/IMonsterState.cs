// 상태에 따른 몬스터의 행동을 구현하기 인터페이스 입니다.
// 이 인터페이스를 상속받아서 WolfState.cs 등을 구현합니다.
public interface IMonsterState
{
    void OnFixedUpdate();
    void OnEnterState();
    void OnExitState();
}
