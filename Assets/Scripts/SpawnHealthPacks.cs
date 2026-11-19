using UnityEngine;

public class SpawnHealthPacks : MonoBehaviour
{
    public GameObject healthPackPrefab;
    public Transform[] spawnPoints;
    public float checkInterval = 10f;

    private float timer;

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        timer += Time.deltaTime;

        if (timer < checkInterval)
            return;

        timer = 0f;

        GameObject[] packs =
            GameObject.FindGameObjectsWithTag("NPCHealthPickup");

        if (packs.Length > 0 ||
            healthPackPrefab == null ||
            spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        Transform point = spawnPoints[index];

        if (point == null)
            return;

        GameObject pack = Instantiate(
            healthPackPrefab,
            point.position,
            Quaternion.identity);

        pack.tag = "NPCHealthPickup";
        pack.SetActive(true);
    }
}
