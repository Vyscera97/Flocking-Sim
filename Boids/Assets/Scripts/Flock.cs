using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flock : MonoBehaviour
{
    [SerializeField] 
    float flockSize;
    [SerializeField] 
    public float flockSpeed;
    [SerializeField] 
    public float minDistance;

    [SerializeField]
    public Camera cam;
    [SerializeField]
    GameObject boidPrefab;    

    [SerializeField]
    Slider AvoidSlider;
    [SerializeField]
    Slider AlignSlider;
    [SerializeField]
    Slider CohereSlider;

    public float height;
    public float width;
    public float avoidFactor;
    public float alignFactor;
    public float cohereFactor;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;

        height = cam.orthographicSize;
        width = height * cam.aspect;

        for (int i = 0; i < flockSize; i++) 
        {
            float x = Random.Range(-width, width);
            float y = Random.Range(-height, height);

            Vector3 randomPos = new Vector3(x, y, 0);
            GameObject boid = Instantiate(boidPrefab, randomPos, Quaternion.identity);
            boid.transform.parent = transform;
        }

        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {      
        avoidFactor = AvoidSlider.value;
        alignFactor = AlignSlider.value;
        cohereFactor = CohereSlider.value;        
    }
}
