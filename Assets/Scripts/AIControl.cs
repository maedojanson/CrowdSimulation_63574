using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour 
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject[] goalLocations;
    private GameObject currentGoal;
    private bool isWaiting = false;

    [Header("Configurações")]
    public float waitTime = 2.0f;          // 2 segundos em repouso no cubo
    public float arriveDistance = 1.5f;    // Distância para iniciar a paragem

    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (agent != null)
        {
            agent.stoppingDistance = 0.5f;
            agent.autoBraking = true; // Desaceleração suave ao aproximar-se
        }

        // Recolhe os cubos pelo nome ou pela tag "goal"
        List<GameObject> cubesList = new List<GameObject>();
        for (int i = 0; i < 20; i++)
        {
            string nameToSearch = (i == 0) ? "Cube" : "Cube (" + i + ")";
            GameObject c = GameObject.Find(nameToSearch);
            if (c != null)
            {
                cubesList.Add(c);
            }
        }

        if (cubesList.Count == 0)
        {
            cubesList.AddRange(GameObject.FindGameObjectsWithTag("goal"));
        }

        goalLocations = cubesList.ToArray();

        // Inicia o movimento
        PickRandomGoal();
    }

    void Update() 
    {
        if (agent == null || isWaiting) return;

        // Se está perto do cubo e tem caminho ativo, inicia a pausa com idle
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= arriveDistance)
        {
            StartCoroutine(IdleAtCubeRoutine());
        }
    }

    IEnumerator IdleAtCubeRoutine()
    {
        isWaiting = true;

        // 1. Pára o NavMeshAgent suavemente
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // 2. Transita suavemente para a animação de repouso (Idle / Respiração)
        if (anim != null)
        {
            anim.ResetTrigger("isWalking");
            anim.SetTrigger("isIdle");
        }

        // 3. Fica a respirar no sítio durante 2 segundos
        yield return new WaitForSeconds(waitTime);

        // 4. Volta a ativar a animação de caminhar
        if (anim != null)
        {
            anim.ResetTrigger("isIdle");
            anim.SetTrigger("isWalking");
        }

        // 5. Retoma a navegação e escolhe o próximo destino
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        PickRandomGoal();
        isWaiting = false;
    }

    void PickRandomGoal()
    {
        if (agent == null || goalLocations == null || goalLocations.Length == 0) return;

        // Escolhe um cubo diferente do anterior
        GameObject newGoal = currentGoal;
        int attempts = 0;
        while (newGoal == currentGoal && attempts < 10)
        {
            int randomIndex = Random.Range(0, goalLocations.Length);
            newGoal = goalLocations[randomIndex];
            attempts++;
        }

        currentGoal = newGoal;

        // Variação suave à volta do ponto
        Vector3 offset = Random.insideUnitSphere * 1.0f;
        offset.y = 0;

        agent.SetDestination(currentGoal.transform.position + offset);

        // Garante que o trigger de andar está ativo ao partir
        if (anim != null)
        {
            anim.SetTrigger("isWalking");
        }
    }
}