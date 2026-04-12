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

    public void TeleportEnemy(GameObject enemy)
    {
        if (destination != null)
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                agent.enabled = false;

                enemy.transform.position = destination.position;

                agent.enabled = true;

                var controller = enemy.GetComponent<EnemyFSMController>();
                if (controller != null && controller.target != null)
                {
                    agent.SetDestination(controller.target.position);
                }
            }
            else
            {
                enemy.transform.position = destination.position;
            }

            Debug.Log($"{enemy.name}가 실제로 {destination.position}으로 이동했습니다.");
        }
    }
}