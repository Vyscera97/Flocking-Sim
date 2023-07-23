using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    List<GameObject> boids = new List<GameObject>();

    Vector2 distance;
    Vector2 newVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector3 startingVector = Random.insideUnitCircle.normalized;
        rb.velocity = startingVector * maxSpeed; 
    }

    // Update is called once per frame
    void Update()
    {        
        AvoidBoids();
        AlignBoids();
        CohereBoids();
        rb.velocity = (rb.velocity + newVelocity).normalized * maxSpeed;

        speed = rb.velocity.magnitude;
        //rb.velocity = Vector2.ClampMagnitude(rb.velocity + newVelocity, maxSpeed);
        /*
        if (speed < minSpeed)
        {
            rb.velocity = newVelocity.normalized * minSpeed;            
        }
        else if (speed > maxSpeed)
        {
            rb.velocity = newVelocity.normalized * maxSpeed;
        }
        */
        Quaternion rotation = Quaternion.LookRotation(transform.forward, rb.velocity);
        transform.rotation = rotation;
    }

    void AvoidBoids()
    {
        newVelocity = Vector2.zero;
        Vector2 avoidVelocity = Vector2.zero;
        if (boids.Count > 0)
        {
            foreach (GameObject boid in boids)
            {
                distance = (boid.transform.position - transform.position);
                if (distance.magnitude < minDistance) 
                {
                    distance /= distance.sqrMagnitude;
                    avoidVelocity -= distance;
                }
            }
            //avoidVelocity /= boids.Count;
            newVelocity += (avoidVelocity * avoidFactor) * Time.deltaTime;
        }        
    }

    void AlignBoids()
    {
        Vector2 averageVelocity = Vector2.zero;
        if (boids.Count > 0)
        {
            foreach (GameObject boid in boids)
            {
                distance = (boid.transform.position - transform.position);
                if (distance.magnitude < minDistance)
                {
                    Rigidbody2D otherRb = boid.GetComponent<Rigidbody2D>();
                    averageVelocity += otherRb.velocity;
                }
            }
            averageVelocity /= boids.Count;
            newVelocity += (averageVelocity * alignFactor) * Time.deltaTime;
        }        
    }

    void CohereBoids()
    {
        Vector3 avgPos = Vector3.zero;
        Vector2 avgVector = Vector2.zero;
        if (boids.Count > 0)
        {
            foreach (GameObject boid in boids)
            {
                distance = (boid.transform.position - transform.position);
                if (distance.magnitude < minDistance)
                {
                    avgPos += boid.transform.position;
                }
            }
            avgPos /= boids.Count;
            avgVector = avgPos - transform.position;
            newVelocity += (avgVector * cohereFactor) * Time.deltaTime;
        }        
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
