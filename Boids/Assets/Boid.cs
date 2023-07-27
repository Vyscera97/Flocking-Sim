using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField] float minDistance;    
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float avoidFactor;
    [SerializeField] float alignFactor;
    [SerializeField] float cohereFactor;

    Rigidbody2D rb;

    [SerializeField]
    List<GameObject> boids = new();

    public bool isUpdated = false;
    public bool useJobs = false;

    public float distance;
    public float2 velocity;
    public float2 newVelocity;
    public float3 difference;
    public float3 pos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector3 startingVector = UnityEngine.Random.insideUnitCircle.normalized;
        rb.velocity = startingVector * maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {  
        isUpdated = false;
        CheckEdge();
        velocity = rb.velocity;
        pos = transform.position;
        Flock();        
        Quaternion rotation = Quaternion.LookRotation(transform.forward, rb.velocity);
        transform.rotation = rotation;
    }

    void Flock()
    {
        Boid Boid;        
        float2 avoidVelocity = Vector2.zero;
        float2 averageVelocity = Vector2.zero;
        float3 avgVector = Vector3.zero;
        float3 avgPos = Vector3.zero;
        if (boids.Count > 0)
        {
            if (useJobs)
            {
                NativeArray<float3> localFlockPositions = new NativeArray<float3>(boids.Count, Allocator.TempJob);
                NativeArray<float2> localFlockVelocities = new NativeArray<float2>(boids.Count, Allocator.TempJob);
                NativeArray<float3> newPos = new NativeArray<float3>(1, Allocator.TempJob);
                NativeArray<float2> newVel = new NativeArray<float2>(2, Allocator.TempJob);
                //NativeArray<float2> localFlockNewVelocities = new NativeArray<float2>(boids.Count, Allocator.Temp);

                for (int i = 0; i < boids.Count; i++)
                {
                    Boid = boids[i].GetComponent<Boid>();
                    localFlockPositions[i] = Boid.transform.position;
                    if (Boid.isUpdated == true)
                    {
                        localFlockVelocities[i] = Boid.velocity;
                    }
                    else
                    {
                        localFlockVelocities[i] = Boid.newVelocity;
                    }
                }
                
                LocalFlockJob flockJob = new LocalFlockJob()
                {
                    localFlockPositions = localFlockPositions,
                    localFlockVelocities = localFlockVelocities,
                    newPos = newPos,
                    newVel = newVel,
                    minDistance = minDistance,
                    //newVelocity = newVelocity,
                    avoidVelocity = avoidVelocity,
                    averageVelocity = averageVelocity,
                    //avgVector = avgVector,
                    avgPos = avgPos,
                    pos = pos
                };
                JobHandle jobHandle = new JobHandle();
                jobHandle = flockJob.Schedule(boids.Count, 1);
                jobHandle.Complete();
                localFlockPositions.Dispose();
                localFlockVelocities.Dispose();
                avoidVelocity = newVel[0];
                averageVelocity = newVel[1];
                avgPos = newPos[0];
                newPos.Dispose();
                newVel.Dispose();
            }
            else
            {
                foreach (GameObject boid in boids)
                {
                    float3 otherPos = boid.transform.position;
                    difference = (otherPos - pos);
                    distance = math.length(difference);
                    if (distance < minDistance)
                    {
                        Boid = boid.GetComponent<Boid>();
                        if (Boid.isUpdated)
                        {
                            averageVelocity += Boid.velocity;
                        }
                        else
                        {
                            averageVelocity += Boid.newVelocity;
                        }
                        avgPos += Boid.pos;
                        distance /= (distance * distance);
                        avoidVelocity -= distance;
                    }
                }

            }
            avoidVelocity = (avoidVelocity - velocity) * avoidFactor;
            averageVelocity = ((averageVelocity / boids.Count) - velocity) * alignFactor;
            avgVector = avgPos / boids.Count - pos;
            // I HATE THIS WORKAROUND
            Vector3 renameAvgVector = avgVector;
            Vector2 alsoRenameThisVector = renameAvgVector;
            float2 whyDoesThisWork = alsoRenameThisVector;
            whyDoesThisWork = (whyDoesThisWork - velocity) * cohereFactor;
            newVelocity = (avoidVelocity + averageVelocity + whyDoesThisWork);
        }
        newVelocity = (math.normalize(velocity + newVelocity) * maxSpeed);        
        rb.velocity = newVelocity;
        isUpdated = true;
    }

    void CheckEdge()
    {
        float3 pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x <= 0.0f)
        {
            pos = new float3(1.0f, pos.y, pos.z);
        }
        else if (pos.x >= 1.0f)
        {
            pos = new float3(0.0f, pos.y, pos.z);
        }
        if (pos.y <= 0.0f)
        {
            pos = new float3(pos.x, 1.0f, pos.z);
        }
        else if (pos.y >= 1.0f)
        {
            pos = new float3(pos.x, 0.0f, pos.z);
        }
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        boids.Add(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        boids.Remove(collision.gameObject);
    }
   
    private void OnDrawGizmosSelected()
    {
        if (boids.Count > 0)
        {
            foreach (GameObject boid in boids)
            {
                
                if ((boid.transform.position - transform.position).magnitude < minDistance)
                {
                    Gizmos.DrawLine(transform.position, boid.transform.position);
                }                
                //Gizmos.DrawLine(transform.position, boid.transform.position);
            }
        }
    }    
}