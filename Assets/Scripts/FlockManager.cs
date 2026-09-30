using UnityEngine;

public class FlockManager : MonoBehaviour {

    public static FlockManager FM;
    public GameObject[] fishPrefab;
    public int numFish = 20;
    public GameObject[] allFish;
    public Vector3 swimLimits = new Vector3(5.0f, 5.0f, 5.0f);
    public Vector3 goalPos = Vector3.zero;

    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Range(1.0f, 10.0f)]
    public float neighbourDistance;
    [Range(1.0f, 5.0f)]
    public float rotationSpeed;

    void Start() {

        allFish = new GameObject[numFish];

        for (int i = 0; i < numFish; ++i) {

            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-swimLimits.x, swimLimits.x), 
                Random.Range(-swimLimits.x, swimLimits.x), 
                Random.Range(-swimLimits.x, swimLimits.x));

            allFish[i] = Instantiate(fishPrefab[Random.Range(0, fishPrefab.Length)], pos, Quaternion.identity);
        }
        FM = this;
        goalPos = this.transform.position;
    }


    void Update() {

        if(Random.Range(0, 100) < 10) {

            goalPos = this.transform.position + new Vector3(
                Random.Range(-swimLimits.x, swimLimits.x),
                Random.Range(-swimLimits.x, swimLimits.x),
                Random.Range(-swimLimits.x, swimLimits.x));
        }
    }
}
