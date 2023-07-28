using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;


[BurstCompile (CompileSynchronously = true)]
public struct FlockJob : IJobParallelForTransform
{
    public float speed;
    public float deltaTime;
    public NativeArray<float2> newVelocities;


    public void Execute(int i, TransformAccess transform)
    {
        Vector3 newPos = new Vector3(newVelocities[i].x, newVelocities[i].y, 0);
        newPos *= speed * deltaTime;
        transform.position += newPos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, newPos);
    }
}


