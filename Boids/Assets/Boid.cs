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
    public float2 difference;
    public float2 pos;

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
        pos = new float2(transform.position.x, transform.position.y);
        Flock();
        Quaternion rotation = Quaternion.LookRotation(transform.forward, rb.velocity);
        transform.rotation = rotation;
    }

    void Flock()
    {
        Boid Boid;
        float2 avoidVelocity = float2.zero;
        float2 averageVelocity = float2.zero;
        float2 avgVector = float2.zero;
        float2 avgPos = float2.zero;

        if (boids.Count > 0)
        {
            foreach (GameObject boid in boids)
            {
                float2 otherPos = new float2(boid.transform.position.x, boid.transform.position.y);
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
                    difference /= (distance * distance);
                    avoidVelocity -= difference;
                }
            }
            
            avoidVelocity = (avoidVelocity - velocity) * avoidFactor;
            averageVelocity = ((averageVelocity / boids.Count) - velocity) * alignFactor;
            avgVector = avgPos / boids.Count - pos;            
            avgVector = (avgVector - velocity) * cohereFactor;
            newVelocity = (avoidVelocity + averageVelocity + avgVector);
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