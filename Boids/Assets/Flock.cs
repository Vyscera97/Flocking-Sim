using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    float height;
    float width;

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
    [SerializeField]
    TransformAccessArray m_AccessArray;

    NativeArray<bool> hasLocalFlock;
    NativeArray<float2> velocities;
    NativeArray<float2> avoidVelocities;
    NativeArray<float2> avgVelocities;
    NativeArray<float2> avgPositions;
    NativeArray<float2> localFlockCounts;
    NativeArray<float2> newVelocities;

    // Start is called before the first frame update
    void Awake()
    {        
        m_Transforms = new Transform[flockSize];

        height = cam.orthographicSize;
        width = height * cam.aspect;

        for (int i = 0; i < flockSize; i++) 
        {
            float x = UnityEngine.Random.Range(-width, width);
            float y = UnityEngine.Random.Range(-height, height);

            Vector3 randomPos = new Vector3(x, y, 0);
            GameObject boid = Instantiate(boidPrefab, randomPos, Quaternion.identity);            
            //boid.transform.parent = transform;
            m_Transforms.Append(boid.transform);
            boids.Add(boid);                       
        }
    }       

    // Update is called once per frame
    void FixedUpdate()
    {
        hasLocalFlock = new(flockSize, Allocator.Persistent);
        velocities = new(flockSize, Allocator.Persistent);
        avoidVelocities = new(flockSize, Allocator.Persistent);
        avgVelocities = new(flockSize, Allocator.Persistent);
        avgPositions = new(flockSize, Allocator.Persistent);
        localFlockCounts = new(flockSize, Allocator.Persistent);          
        newVelocities = new(flockSize, Allocator.Persistent);          
        

        for (int i = 0; i < flockSize; i++)
        {
            float2 avoidVelocity = float2.zero;
            float2 averageVelocity = float2.zero;
            float2 avgVector = float2.zero;
            float2 avgPos = float2.zero;

            Boid Boid = boids[i].GetComponent<Boid>();            
            float2 position = Boid.position;
            float2 velocity = Boid.velocity;
            velocities[i] = velocity;


            List<GameObject> localBoids = Boid.localBoids;

            if (localBoids.Count > 0)
            {
                hasLocalFlock[i] = true;

                foreach (GameObject localBoid in localBoids)
                {
                    Boid boid = localBoid.GetComponent<Boid>();
                    float2 otherPos = boid.position;
                    float2 otherVelocity = boid.velocity;
                    float2 difference = (otherPos - position); 
                    if (math.length(difference) < minDistance)
                    {
                        avgPos += otherPos;
                        averageVelocity += otherVelocity;
                        difference /= math.lengthsq(difference);
                        avoidVelocity -= new float2(difference.x, difference.y);
                    }
                }
            }
            else
            {
                hasLocalFlock[i] = false;
            }

            avoidVelocities[i] = avoidVelocity;
            avgVelocities[i] = averageVelocity;
            avgPositions[i] = avgPos;
        }      

        FlockJob flock = new()
        {
            speed = speed,
            deltaTime = Time.deltaTime,
            avoidFactor = avoidFactor,
            alignFactor = alignFactor,
            cohereFactor = cohereFactor,
            hasLocalFlock = hasLocalFlock,
            velocities = velocities,
            avgVelocities = avgVelocities,
            avgPositions = avgPositions,
            localFlockCounts = localFlockCounts,
            newVelocities = newVelocities,
            
        };

        JobHandle jobHandle = flock.Schedule(m_AccessArray);
        jobHandle.Complete();

        hasLocalFlock.Dispose();
        velocities.Dispose();
        avoidVelocities.Dispose();
        avgVelocities.Dispose();
        avgPositions.Dispose();
        localFlockCounts.Dispose();

        for (int i = 0; i < flockSize; i++)
        {
            Boid Boid = boids[i].GetComponent<Boid>();
            Boid.velocity = newVelocities[i];
        }
        newVelocities.Dispose();
    }

    private void OnDestroy()
    {        
        m_AccessArray.Dispose();
    }
}
