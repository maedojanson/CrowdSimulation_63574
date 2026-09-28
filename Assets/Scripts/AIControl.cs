using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class AIControl : MonoBehaviour 
{
    GameObject[] goalLocations;
    NavMeshAgent agent;
    Animator anim;
    float speedMult;
    float detectionRadius = 20.0f;
    float fleeRadius = 10.0f;

    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        goalLocations = GameObject.FindGameObjectsWithTag("goal");

        if (goalLocations != null && goalLocations.Length > 0 && agent != null)
        {
            int i = Random.Range(0, goalLocations.Length);
            agent.SetDestination(goalLocations[i].transform.position);
        }

        if (anim != null)
        {
            anim.SetFloat("wOffset", Random.Range(0.0f, 1.0f));
        }

        ResetAgent();
    }

    void ResetAgent() 
    {
        speedMult = Random.Range(0.1f, 1.5f);

        if (anim != null)
        {
            anim.SetFloat("speedMult", speedMult);
            anim.SetTrigger("isWalking");
        }

        if (agent != null)
        {
            agent.speed = 2.0f * speedMult;
            agent.angularSpeed = 120.0f;
            agent.ResetPath();
        }
    }

    public void DetectNewObstacle(Vector3 position) 
    {
        if (agent == null) return;

        if (Vector3.Distance(position, transform.position) < detectionRadius) 
        {
            Vector3 fleeDirection = (transform.position - position).normalized;
            Vector3 newGoal = transform.position + fleeDirection * fleeRadius;

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newGoal, path);

            if (path.status != NavMeshPathStatus.PathInvalid) 
            {
                agent.SetDestination(path.corners[path.corners.Length - 1]);

                if (anim != null)
                {
                    anim.SetTrigger("isRunning");
                }

                agent.speed = 10.0f;
                agent.angularSpeed = 500.0f;
            }
        }
    }

    void Update() 
    {
        if (agent == null || goalLocations == null || goalLocations.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 1.0f) 
        {
            ResetAgent();
            int i = Random.Range(0, goalLocations.Length);
            agent.SetDestination(goalLocations[i].transform.position);
        }
    }
}
