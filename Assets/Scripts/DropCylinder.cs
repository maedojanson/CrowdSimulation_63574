using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropCylinder : MonoBehaviour 
{
    public GameObject obstacle;
    private GameObject[] agents;

    void Start() 
    {
        agents = GameObject.FindGameObjectsWithTag("agent");
    }

    void Update() 
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame) 
        {
            RaycastHit hitInfo;
            Vector2 mousePos = Mouse.current.position.ReadValue();

            Camera cam = Camera.main;
            if (cam == null)
            {
                cam = GetComponentInChildren<Camera>();
            }

            if (cam == null) return;

            Ray ray = cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out hitInfo)) 
            {
                if (obstacle != null)
                {
                    Instantiate(obstacle, hitInfo.point, obstacle.transform.rotation);
                }

                if (agents != null && agents.Length > 0)
                {
                    foreach (GameObject a in agents) 
                    {
                        if (a != null)
                        {
                            AIControl ai = a.GetComponent<AIControl>();
                            if (ai != null)
                            {
                                ai.DetectNewObstacle(hitInfo.point);
                            }
                        }
                    }
                }
            }
        }
    }
}
