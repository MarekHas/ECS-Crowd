using Unity.Entities;
using Unity.Jobs;

public class LifeSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var entityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Life life) =>
        {
            life.Time -= deltaTime;

            if (life.Time <= 0f)
            {
                entityCommandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }
        }).ScheduleParallel();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

}