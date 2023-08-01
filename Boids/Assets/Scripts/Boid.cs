using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile(CompileSynchronously = true)]
public class Boid : MonoBehaviour
{
    [SerializeField]
    List<GameObject> boids = new List<GameObject>();    

    Rigidbody2D rb;
    Flock flockManager;
    Camera cam;

    float minDistance;
    float maxSpeed;
    float avoidFactor;
    float alignFactor;
    float cohereFactor;

    float2 difference;
    float2 pos;
    float2 velocity;
    float2 newVelocity;

    // Start is called before the first frame update
    void Start()
    {
        flockManager = GameObject.Find("Flock").GetComponent<Flock>();
        cam = flockManager.cam;        
        minDistance = flockManager.minDistance;
        maxSpeed = flockManager.flockSpeed;
        avoidFactor = flockManager.avoidFactor / 1.5f;
        alignFactor = flockManager.alignFactor / 10f;
        cohereFactor = flockManager.cohereFactor / 5f;

        rb = GetComponent<Rigidbody2D>();
        float2 startingVector = UnityEngine.Random.insideUnitCircle.normalized;
        newVelocity = startingVector * maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        minDistance = flockManager.minDistance;
        maxSpeed = flockManager.flockSpeed;
        avoidFactor = flockManager.avoidFactor / 1.5f;
        alignFactor = flockManager.alignFactor / 10f;
        cohereFactor = flockManager.cohereFactor / 5f;                
    }

    private void FixedUpdate()
    {
        rb.velocity = newVelocity;
        Quaternion rotation = Quaternion.LookRotation(transform.forward, rb.velocity);
        transform.rotation = rotation;
        Flock();
        CheckEdge();
    }

    void Flock()
    {
        velocity = newVelocity;
        newVelocity = Vector2.zero;
        pos = new(transform.position.x, transform.position.y);
        float2 avoidVelocity = Vector2.zero;
        float2 averageVelocity = Vector2.zero;
        float2 avgVector = Vector2.zero;
        float2 avgPos = Vector2.zero;
        if (boids.Count > 0)
        {
            foreach (GameObject boid in boids)
            {
                Boid Boid = boid.GetComponent<Boid>();
                float2 otherPos = new(boid.transform.position.x, boid.transform.position.y);
                difference = (otherPos - pos);
                if (math.length(difference) < minDistance)
                {
                    averageVelocity += Boid.velocity;
                    avgPos += otherPos;
                    difference /= math.lengthsq(difference);
                    avoidVelocity -= difference;
                }
            }
            avoidVelocity = (avoidVelocity) * avoidFactor;
            averageVelocity = ((averageVelocity / boids.Count) - velocity) * alignFactor;
            avgVector = avgPos / boids.Count - pos;
            avgVector = (avgVector - velocity) * cohereFactor;
            newVelocity = (avoidVelocity + averageVelocity + avgVector);
        }
        newVelocity = math.normalize(velocity + newVelocity) * maxSpeed;
    }

    void CheckEdge()
    {
        Vector3 pos = cam.WorldToViewportPoint(transform.position);

        if (pos.x <= 0.0f)
        {
            pos = new Vector3(1.0f, pos.y, pos.z);
        }
        else if (pos.x >= 1.0f)
        {
            pos = new Vector3(0.0f, pos.y, pos.z);
        }
        if (pos.y <= 0.0f)
        {
            pos = new Vector3(pos.x, 1.0f, pos.z);
        }
        else if (pos.y >= 1.0f)
        {
            pos = new Vector3(pos.x, 0.0f, pos.z);
        }

        transform.position = cam.ViewportToWorldPoint(pos);
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