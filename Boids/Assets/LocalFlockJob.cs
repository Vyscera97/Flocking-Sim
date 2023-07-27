using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct LocalFlockJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float3> localFlockPositions;
    [ReadOnly]
    public NativeArray<float2> localFlockVelocities;
    public NativeArray<float3> newPos;
    public NativeArray<float2> newVel;
    public float minDistance;
    //public float2 newVelocity;
    public float2 avoidVelocity;
    public float2 averageVelocity;
    //public float2 avgVector;
    public float3 avgPos;
    public float3 pos;

    public void Execute(int index)
    {       
        float3 difference = (localFlockPositions[index] - pos);
        float distance = math.length(difference);
        if (distance < minDistance)
        {
            avgPos += localFlockPositions[index];
            averageVelocity += localFlockVelocities[index];
            difference /= (distance * distance);
            avoidVelocity -= distance;
        }
        newPos[0] = avgPos;
        newVel[0] = avoidVelocity;
        newVel[1] = averageVelocity;
    }
}