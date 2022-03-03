using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RandomNumbersSystem : SystemBase
{
    public NativeArray<Random> RandomNumbers { get; private set; }

    protected override void OnUpdate() { }

    protected override void OnCreate()
    {
        var randomNumbers = new Random[JobsUtility.MaxJobThreadCount];
        var randomSystem = new System.Random();

        for (int i = 0; i < JobsUtility.MaxJobThreadCount; i++)
        {
            randomNumbers[i] = new Random((uint)randomSystem.Next());
        }

        RandomNumbers = new NativeArray<Random>(randomNumbers, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        RandomNumbers.Dispose();
    }
}
