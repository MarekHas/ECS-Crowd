using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;

public class HumanCollisionSystem : SystemBase
{
    private BuildPhysicsWorld _buildPhysicsWorld;
    private StepPhysicsWorld _stepPhysicsWorld;

    protected override void OnCreate()
    {
        _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    struct HumanCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<HumanTag> HumansGroup;
        public ComponentDataFromEntity<URPMaterialPropertyBaseColor> ColorGroup;
        public float RandomSeed;

        public void Execute(TriggerEvent triggerEvent)
        {
            bool isEntityAHuman = HumansGroup.HasComponent(triggerEvent.EntityA);
            bool isEntityBHuman = HumansGroup.HasComponent(triggerEvent.EntityB);

            if (!isEntityAHuman || !isEntityBHuman) { return; }

            var random = new Random((uint)((1 + RandomSeed) + (triggerEvent.BodyIndexA * triggerEvent.BodyIndexB)));

            random = ChangeMaterialColor(random, triggerEvent.EntityA);

            ChangeMaterialColor(random, triggerEvent.EntityB);
        }

        private Random ChangeMaterialColor(Random random, Entity entity)
        {
            if (ColorGroup.HasComponent(entity))
            {
                var colorComponent = ColorGroup[entity];

                colorComponent.Value.x = random.NextFloat(0, 1);
                colorComponent.Value.y = random.NextFloat(0, 1);
                colorComponent.Value.z = random.NextFloat(0, 1);

                ColorGroup[entity] = colorComponent;
            }

            return random;
        }
    }

    protected override void OnUpdate()
    {
        Dependency = new HumanCollisionJob
        {
            HumansGroup = GetComponentDataFromEntity<HumanTag>(true),
            ColorGroup = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>(),
            RandomSeed = System.DateTimeOffset.Now.Millisecond
        }.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, Dependency);
    }
}
