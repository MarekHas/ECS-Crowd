using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class DestinationSystem : SystemBase
{
    private RandomNumbersSystem randomSystem;

    protected override void OnCreate()
    {
        randomSystem = World.GetExistingSystem<RandomNumbersSystem>();
    }

    protected override void OnUpdate()
    {
        var randomArray = randomSystem.RandomNumbers;

        Entities
            .WithNativeDisableParallelForRestriction(randomArray)
            .ForEach((int nativeThreadIndex, ref Destination destination, in Translation translation) =>
            {
                float distance = math.abs(math.length(destination.Coordinates - translation.Value));

                if (distance < 0.1f)
                {
                    var random = randomArray[nativeThreadIndex];

                    destination.Coordinates.x = random.NextFloat(0, 250);
                    destination.Coordinates.z = random.NextFloat(0, 250);

                    randomArray[nativeThreadIndex] = random;
                }
            }).ScheduleParallel();
    }
}
