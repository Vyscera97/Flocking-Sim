using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boid : MonoBehaviour
{
    [SerializeField] float minDistance;
    [SerializeField] float maxSpeed;
    [SerializeField] float avoidFactor;
    [SerializeField] float alignFactor;
    [SerializeField] float cohereFactor;

    Rigidbody2D rb;

    [SerializeField]
    List<GameObject> boids = new List<GameObject>();

    Vector2 difference;
    Vector2 velocity;
    Vector2 newVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector3 startingVector = Random.insideUnitCircle.normalized;
        newVelocity = startingVector * maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = Quaternion.LookRotation(transform.forward, rb.velocity);
        transform.rotation = rotation;
        CheckEdge();
    }

    private void FixedUpdate()
    {
        rb.velocity = newVelocity;
        Flock();
    }

    void Flock()
    {
        velocity = newVelocity;
        newVelocity = Vector2.zero;
        Vector3 pos = transform.position;        
        Vector2 avoidVelocity = Vector2.zero;
        Vector2 averageVelocity = Vector2.zero;
        Vector2 avgVector = Vector2.zero;
        Vector3 avgPos = Vector3.zero;
        if (boids.Count > 0)
        {            
            foreach (GameObject boid in boids)
            {
                Boid Boid = boid.GetComponent<Boid>();
                difference = (boid.transform.position - pos);
                if (difference.magnitude < minDistance)
                {
                    Rigidbody2D otherRb = boid.GetComponent<Rigidbody2D>();
                    averageVelocity += Boid.velocity;
                    avgPos += boid.transform.position;
                    difference /= difference.sqrMagnitude;
                    avoidVelocity -= difference;
                }
            }
            avoidVelocity = (avoidVelocity) * avoidFactor;
            averageVelocity = ((averageVelocity / boids.Count) - velocity) * alignFactor;
            avgVector = avgPos / boids.Count - pos;
            avgVector = (avgVector - velocity) * cohereFactor;
            newVelocity = (avoidVelocity + averageVelocity + avgVector);
        }  
        newVelocity = (velocity + newVelocity).normalized * maxSpeed;
    }

    void CheckEdge()
    {        
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        
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