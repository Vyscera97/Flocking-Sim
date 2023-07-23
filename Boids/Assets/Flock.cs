using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField]
    float flockSize;
    [SerializeField]
    GameObject boidPrefab;
    [SerializeField]
    Camera cam;

    public float height;
    public float width;
    // Start is called before the first frame update
    void Start()
    {

        height = cam.orthographicSize;
        width = height * cam.aspect;

        for (int i = 0; i < flockSize; i++) 
        {
            float x = Random.Range(-width, width);
            float y = Random.Range(-height, height);

            Vector3 randomPos = new Vector3(x, y, 0);
            GameObject boid = Instantiate(boidPrefab, randomPos, Quaternion.identity);
            boid.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
