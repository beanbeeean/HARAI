using UnityEngine;
using UnityEngine.AI;

public class TeleportPortal : InteractableBase
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform destination;

    public override void Interact()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && destination != null)
        {
            player.transform.position = destination.position;
            HideUI();
        }
    }

    // 몬스터가 '지능적'으로 판단한 후 호출하는 함수
    public void TeleportEnemy(GameObject enemy)
    {
        if (destination != null)
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                // 1. 에이전트 비활성화 (순간이동을 위해 중요!)
                agent.enabled = false;

                // 2. 위치 이동
                enemy.transform.position = destination.position;

                // 3. 에이전트 재활성화
                agent.enabled = true;

                // 4. [중요] 새로운 위치에서 즉시 목적지 재설정
                // Chase 상태인 경우 타겟 방향으로 다시 길을 찾게 합니다.
                var controller = enemy.GetComponent<EnemyFSMController>();
                if (controller != null && controller.target != null)
                {
                    agent.SetDestination(controller.target.position);
                }
            }
            else
            {
                // 에이전트가 없는 일반 객체라면 그냥 이동
                enemy.transform.position = destination.position;
            }

            Debug.Log($"{enemy.name}가 실제로 {destination.position}으로 이동했습니다.");
        }
    }
}