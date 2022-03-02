using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class HumanSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _humanPrefab;
    [SerializeField] private int _gridSize;
    [SerializeField] private int _distanceBetween;
    [SerializeField] private Vector2 _speedRange = new Vector2(2, 6);

    private BlobAssetStore blob;

    private void Start()
    {
        blob = new BlobAssetStore();

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blob);
        var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_humanPrefab, settings);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                var instance = entityManager.Instantiate(entity);

                float3 position = new float3(x * _distanceBetween, 0, z * _distanceBetween);
                float speed = UnityEngine.Random.Range(_speedRange.x, _speedRange.y);
                float random = UnityEngine.Random.Range(-_gridSize, _gridSize);
                float3 destination = new float3(0, 0,random);
                entityManager.SetComponentData(instance, new Translation { Value = position });
                entityManager.SetComponentData(instance, new Destination { Coordinates = destination });
                entityManager.SetComponentData(instance, new Movement { Speed = speed });
            }
        }
    }

    private void OnDestroy()
    {
        blob.Dispose();
    }

}
