using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Translation translation, ref Rotation rotation, in Destination destination, in Movement movement) => 
        {    
            if (math.all(destination.Coordinates == translation.Value)) 
            { 
                return; 
            }

            float3 targetDestination = destination.Coordinates - translation.Value;
            rotation.Value = quaternion.LookRotation(targetDestination, new float3(0, 1, 0));
            float3 moveValue = math.normalize(targetDestination) * movement.Speed * deltaTime;
            
            if (math.length(moveValue) >= math.length(targetDestination))
            {
                translation.Value = destination.Coordinates;
            }
            else
            {
                translation.Value += moveValue;
            }
        }).ScheduleParallel();
    }
}
