using UnityEngine;
using UnityEngine.AI;

public class NavAgentTEST : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private NavMeshAgent navMeshAgent;
    
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                navMeshAgent.SetDestination(hit.point);
            }
        }
    }
}
