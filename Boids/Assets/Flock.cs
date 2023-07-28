using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

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

    [SerializeField]
    List<GameObject> boids = new();

    [SerializeField]
    public Transform[] m_Transforms;
    TransformAccessArray m_AccessArray;


    NativeArray<float2> localFlockPositions;
    NativeArray<float2> localFlockVelocities;
    NativeArray<float2> newVelocities;
    NativeArray<JobHandle> jobHandles;

    // Start is called before the first frame update
    void Start()
    {
        m_AccessArray = new();

        height = cam.orthographicSize;
        width = height * cam.aspect;

        for (int i = 0; i < flockSize; i++) 
        {
            float x = UnityEngine.Random.Range(-width, width);
            float y = UnityEngine.Random.Range(-height, height);

            Vector3 randomPos = new Vector3(x, y, 0);
            GameObject boid = Instantiate(boidPrefab, randomPos, Quaternion.identity);
            boid.transform.parent = transform;
            boids.Add(boid);
            m_AccessArray.Add(boid.transform);
        }
    }       

    // Update is called once per frame
    void Update()
    {
        jobHandles = new NativeArray<JobHandle>();         
        newVelocities = new(flockSize, Allocator.TempJob);    

        for (int i = 0; i < flockSize; i++)
        {
            int index = i;
            Boid Boid = boids[i].GetComponent<Boid>();            
            float2 position = Boid.position;
            float2 velocity = Boid.velocity;
            List<GameObject> localBoids = Boid.localBoids;

            if (localBoids != null)
            {
                localFlockPositions = new(localBoids.Count, Allocator.TempJob);
                localFlockVelocities = new(localBoids.Count, Allocator.TempJob);

                for (int j = 0; j < localBoids.Count; j++)
                {
                    Boid localBoid = localBoids[j].GetComponent<Boid>();
                    localFlockPositions[j] = localBoid.position;
                    localFlockVelocities[j] = localBoid.velocity;
                }

                LocalFlockJob localFlock = new()
                {
                    index = index,
                    position = position,
                    velocity = velocity,
                    minDistance = minDistance,
                    avoidFactor = avoidFactor,
                    alignFactor = alignFactor,
                    cohereFactor = cohereFactor,
                    localFlockPositions = localFlockPositions,
                    localFlockVelocities = localFlockVelocities,
                    newVelocities = newVelocities
                };

                JobHandle localJobHandle = localFlock.Schedule();
                jobHandles[i] = localJobHandle;
            }
        }

        JobHandle.CompleteAll(jobHandles);        
        localFlockPositions.Dispose();
        localFlockVelocities.Dispose();
        jobHandles.Dispose();

        FlockJob flock = new()
        {
            speed = speed,
            deltaTime = Time.deltaTime,
            newVelocities = newVelocities,
        };

        JobHandle jobHandle = flock.Schedule(m_AccessArray);
        jobHandle.Complete();

        for (int i = 0; i < flockSize; i++)
        {
            Boid Boid = boids[i].GetComponent<Boid>();
            Boid.velocity = newVelocities[i];
        }
        newVelocities.Dispose();
    }

    private void OnDestroy()
    {
        localFlockPositions.Dispose();
        localFlockVelocities.Dispose();
        m_AccessArray.Dispose();
        newVelocities.Dispose();        
        jobHandles.Dispose();
    }
}
