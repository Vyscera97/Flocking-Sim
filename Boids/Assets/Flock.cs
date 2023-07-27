using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField]
    int flockSize;
    [SerializeField]
    GameObject boidPrefab;
    [SerializeField]
    Camera cam;

    public float height;
    public float width;

    //Everything under here is for using the job system. remove all of this when you break everything

    [SerializeField] float minDistance;    
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float avoidFactor;
    [SerializeField] float alignFactor;
    [SerializeField] float cohereFactor;

    // Start is called before the first frame update
    void Start()
    {
        height = cam.orthographicSize;
        width = height * cam.aspect;

        for (int i = 0; i < flockSize; i++) 
        {
            float x = UnityEngine.Random.Range(-width, width);
            float y = UnityEngine.Random.Range(-height, height);

            Vector3 randomPos = new Vector3(x, y, 0);
            GameObject boid = Instantiate(boidPrefab, randomPos, Quaternion.identity);
            boid.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        NativeArray<float2> flockVelocities = new(flockSize, Allocator.TempJob);
        NativeArray<float3> flockPositions = new(flockSize, Allocator.TempJob);
        NativeArray<Quaternion> flockRotations = new(flockSize, Allocator.TempJob);
        NativeArray<bool> updateChecks = new(flockSize, Allocator.TempJob);

        FlockJob job = new()
        {

        };
        JobHandle jobHandler = job.Schedule(flockSize, 1);

    }
}
