using System;
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
using UnityEngine.UIElements;


[BurstCompile (CompileSynchronously = true)]
public struct FlockJob : IJobParallelForTransform
{
    public float speed;
    public float deltaTime;
    public float avoidFactor;
    public float alignFactor;
    public float cohereFactor;

    public NativeArray<bool> hasLocalFlock;

    public NativeArray<float2> velocities;
    public NativeArray<float2> avoidVelocities;
    public NativeArray<float2> avgVelocities;    
    public NativeArray<float2> avgPositions;
    public NativeArray<float2> localFlockCounts;
    public NativeArray<float2> newVelocities;


    public void Execute(int i, TransformAccess transform)
    {
        float2 position = new float2(transform.position.x, transform.position.y);

        if (hasLocalFlock[i])
        {
            float2 avoidVelocity = (avoidVelocities[i] - velocities[i]) * avoidFactor;
            float2 avgVelocity = ((avgVelocities[i] / localFlockCounts[i]) - velocities[i]) * alignFactor;
            float2 avgVector = (avgPositions[i] / localFlockCounts[i]) - position;
            avgVector = (avgVector - velocities[i]) * cohereFactor;
            newVelocities[i] = velocities[i] + (avoidVelocity + avgVelocity + avgVector);            
        }
        else
        {
            newVelocities[i] = velocities[i];
        }

        Vector3 newPos = new Vector3(newVelocities[i].x, newVelocities[i].y, 0);        
        transform.position += (newPos * speed);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, newPos);
    }
}


