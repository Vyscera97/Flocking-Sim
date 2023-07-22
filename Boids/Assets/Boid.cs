using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boid : MonoBehaviour
{
    [SerializeField] float minDistance;    
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] Rigidbody2D rb;

    [SerializeField]
    List<GameObject> boids = new List<GameObject>();

    Vector2 newVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.up * Random.Range(minSpeed, maxSpeed); 
    }

    // Update is called once per frame
    void Update()
    {
        newVelocity = Vector2.zero;
        AvoidBoids();
        AlignBoids();
        CohereBoids();
        rb.velocity = Vector2.ClampMagnitude(rb.velocity + newVelocity, maxSpeed);
        Quaternion rotation = Quaternion.LookRotation(transform.forward, rb.velocity);
        transform.rotation = rotation;
    }

    void AvoidBoids()
    {
        Vector2 avoidVelocity = Vector2.zero;
        if (boids.Count > 0)
        {
            foreach (GameObject boid in boids)
            {
                Vector2 distance = (boid.transform.position - transform.position);
                if (distance.magnitude < minDistance) 
                {
                    distance /= distance.sqrMagnitude;
                    avoidVelocity -= distance;
                }
            }
        }
        newVelocity += avoidVelocity;
    }

    void AlignBoids()
    {

    }

    void CohereBoids()
    {

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
                Gizmos.DrawLine(transform.position, boid.transform.position);
            }
        }
    }
}
