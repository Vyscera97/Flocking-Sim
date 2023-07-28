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

    // Shouldn't be necesary if using transform.position rather than rigidbody.velocity
    //Rigidbody2D rb;

    [SerializeField]
    public List<GameObject> localBoids = new();

    public bool isUpdated = false;
    public bool useJobs = false;

    public float distance;
    public float2 velocity;
    public float2 newVelocity;
    public float2 difference;
    public float2 position;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        float2 startingVector = UnityEngine.Random.insideUnitCircle.normalized;
        velocity = startingVector;
    }

    // Update is called once per frame
    void Update()
    {  
        //isUpdated = false;
        CheckEdge();
        //velocity = rb.velocity;
        position = new float2(transform.position.x, transform.position.y);       
        //Quaternion rotation = Quaternion.LookRotation(transform.forward, velocity);
       // transform.rotation = rotation;
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
        localBoids.Add(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        localBoids.Remove(collision.gameObject);
    }
   
    private void OnDrawGizmosSelected()
    {
        if (localBoids.Count > 0)
        {
            foreach (GameObject boid in localBoids)
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