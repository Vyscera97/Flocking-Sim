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
        Flock();
        //AvoidBoids();
        //AlignBoids();
        //CohereBoids();
        // Test if this is necessary, or if it's better to simply limit to max speed, as opposed to always going max speed
        //rb.velocity = (rb.velocity + newVelocity).normalized * maxSpeed;
        /*
        speed = (rb.velocity + newVelocity).magnitude;  
        if (speed > maxSpeed)
        {
            rb.velocity = (rb.velocity + newVelocity).normalized * maxSpeed;
        }
        else if (speed < minSpeed) 
        {
            rb.velocity = (rb.velocity + newVelocity).normalized * minSpeed;
        }
        else
        {
            rb.velocity+= newVelocity;
        }
        */
        Quaternion rotation = Quaternion.LookRotation(transform.forward, rb.velocity);
        transform.rotation = rotation;

        CheckEdge();
    }

    void Flock()
    {
        newVelocity = Vector2.zero;
        Vector3 pos = transform.position;
        Vector2 velocity = rb.velocity;
        Vector2 avoidVelocity = Vector2.zero;
        Vector2 averageVelocity = Vector2.zero;
        Vector2 avgVector = Vector2.zero;
        Vector3 avgPos = Vector3.zero;
        if (boids.Count > 0)
        {
            foreach (GameObject boid in boids)
            {
                distance = (boid.transform.position - pos);
                if (distance.magnitude < minDistance)
                {                    
                    Rigidbody2D otherRb = boid.GetComponent<Rigidbody2D>();
                    averageVelocity += otherRb.velocity;
                    avgPos += boid.transform.position;
                    distance /= distance.sqrMagnitude;
                    avoidVelocity -= distance;
                }
            }
            avoidVelocity = (avoidVelocity - velocity) * avoidFactor;
            averageVelocity = ((averageVelocity / boids.Count) - velocity) * alignFactor;
            avgVector = avgPos / boids.Count - pos;
            avgVector = (avgVector - velocity) * cohereFactor;
            newVelocity += (avoidVelocity + averageVelocity + avgVector);
        }
        rb.velocity = ((rb.velocity + newVelocity).normalized * maxSpeed);
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
            newVelocity += ((avoidVelocity - rb.velocity) * avoidFactor) * Time.deltaTime;
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
            newVelocity += ((averageVelocity - rb.velocity) * alignFactor) * Time.deltaTime;
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
            newVelocity += ((avgVector - rb.velocity) * cohereFactor) * Time.deltaTime;
        }        
    }

    void CheckEdge()
    {
        //get a world space coord and transfom it to viewport space.
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        //everything from here on is in viewport space where 0,0 is the bottom 
        //left of your screen and 1,1 the top right.
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

        //and here it gets transformed back to world space.
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
