using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[BurstCompile]
public struct LocalFlockJob : IJob
{
    public NativeArray<float2> newVelocity;

    [ReadOnly]
    public NativeArray<float2> localFlockPositions;
    [ReadOnly]
    public NativeArray<float2> localFlockVelocities;

    [ReadOnly]
    public float2 position;
    [ReadOnly]
    public float2 velocity;
    [ReadOnly]
    public float minDistance;    
    [ReadOnly]
    public float avoidFactor;
    [ReadOnly]
    public float alignFactor;
    [ReadOnly]
    public float cohereFactor;

    public void Execute()
    {
        float localFlockCount = localFlockPositions.Length;

        float2 avoidVelocity = float2.zero;
        float2 averageVelocity = float2.zero;
        float2 avgVector = float2.zero;
        float2 avgPos = float2.zero;

        for (int i = 0; i < localFlockPositions.Length; i++)
        {
            float2 difference = (localFlockPositions[i] - position); ;
            if (math.length(difference) < minDistance)
            {
                avgPos += localFlockPositions[i];
                averageVelocity += localFlockVelocities[i];
                difference /= math.lengthsq(difference);
                avoidVelocity -= new float2(difference.x, difference.y);
            }
        }

        avoidVelocity = (avoidVelocity - velocity) * avoidFactor;
        averageVelocity = ((averageVelocity / localFlockCount) - velocity) * alignFactor;
        avgVector = (avgPos / localFlockCount) - position;
        avgVector = (avgVector - velocity) * cohereFactor;
        newVelocity[0] = (avoidVelocity + averageVelocity + avgVector);

    }
}