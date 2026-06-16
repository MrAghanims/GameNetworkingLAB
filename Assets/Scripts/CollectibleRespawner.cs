using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public static CollectibleSpawner Instance;

    public NetworkRunner runner;
    public NetworkPrefabRef collectiblePrefab;

    public Vector3 minBounds;
    public Vector3 maxBounds;



    private void Awake()
    {
        Instance = this;
    }

    public void SpawnCollectible()
    {
        Vector3 pos = GetGroundedPosition();

        runner.Spawn(collectiblePrefab, pos, Quaternion.identity);
    }

    private Vector3 GetGroundedPosition()
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            50f,
            Random.Range(minBounds.z, maxBounds.z)
        );

        if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 100f))
        {
            return hit.point + Vector3.up * 0.3f;
        }

        return new Vector3(randomPoint.x, 1f, randomPoint.z);
    }
}