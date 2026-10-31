using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public GameObject[] pickupPrefabs;
    public Transform[] spawnPoints;

    void Start()
    {
        int amountToSpawn = Mathf.Min(
            pickupPrefabs.Length,
            spawnPoints.Length
        );

        for (int i = 0; i < amountToSpawn; i++)
        {
            Instantiate(
                pickupPrefabs[i],
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );
        }
    }
}