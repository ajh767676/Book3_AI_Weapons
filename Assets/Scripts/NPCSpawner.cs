using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    public SoldierSmart npcPrefab;
    public SoldierSmart sceneSoldier;
    public float spawnInterval = 5f;
    public int maximumSpawns = 5;

    private float timer;
    private int spawnedCount;

    void Update()
    {
        if (Time.timeScale == 0f ||
            npcPrefab == null ||
            sceneSoldier == null ||
            spawnedCount >= maximumSpawns)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer < spawnInterval)
            return;

        timer = 0f;

        if (!NavMesh.SamplePosition(
            transform.position,
            out NavMeshHit hit,
            5f,
            NavMesh.AllAreas))
        {
            return;
        }

        SoldierSmart soldier = Instantiate(
            npcPrefab,
            hit.position,
            transform.rotation);

        soldier.player = sceneSoldier.player;
        soldier.waypoints = sceneSoldier.waypoints;
        soldier.healthPickup = sceneSoldier.healthPickup;
        soldier.ammoPickup = sceneSoldier.ammoPickup;
        soldier.ambushSpot = sceneSoldier.ambushSpot;
        soldier.ambushArea = sceneSoldier.ambushArea;

        spawnedCount++;
    }
}
