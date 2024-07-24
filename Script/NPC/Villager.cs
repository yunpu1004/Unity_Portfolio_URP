using UnityEngine;
using UnityEngine.AI;

// 이 스크립트는 마을 사람들의 움직임을 제어합니다.
public class Villager : MonoBehaviour
{
    public Transform[] waypoints;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private float idleTime = 0f;
    private const float maxIdleTime = 5f;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        navMeshAgent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].position);
        navMeshAgent.speed = 0.9f;
    }   

    // 지정된 경로를 따라 마을 사람들을 이동시킵니다.
    private void FixedUpdate() 
    {
        if (navMeshAgent.remainingDistance < 0.5f)
        {
            idleTime += Time.deltaTime;
            if (idleTime > maxIdleTime)
            {
                idleTime = 0f;
                navMeshAgent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].position);
            }
        }
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }
}