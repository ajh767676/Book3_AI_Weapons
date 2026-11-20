using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SmarterNPCTests
{
    private MonoBehaviour soldier;
    private Animator animator;
    private GameObject testPlayer;
    private GameObject testPack;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        yield return SceneManager.LoadSceneAsync("smarter_npcs");
        yield return null;

        // Stop spawning during these controlled tests.
        foreach (MonoBehaviour script in
                 Object.FindObjectsByType<MonoBehaviour>(
                     FindObjectsSortMode.None))
        {
            string typeName = script.GetType().Name;

            if (typeName == "NPCSpawner" ||
                typeName == "SpawnHealthPacks")
            {
                script.enabled = false;
            }
        }

        GameObject npc = GameObject.Find("SoldierNPC");
        Assert.That(npc, Is.Not.Null);

        soldier = npc.GetComponent("SoldierSmart")
            as MonoBehaviour;

        Assert.That(soldier, Is.Not.Null);
        soldier.enabled = false;

        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        Assert.That(agent.isOnNavMesh, Is.True);

        agent.speed = 0f;
        agent.isStopped = true;

        animator = npc.GetComponent<Animator>();
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        // Remove existing packs from the test.
        foreach (GameObject pack in
                 GameObject.FindGameObjectsWithTag("NPCHealthPickup"))
        {
            pack.SetActive(false);
        }

        testPlayer = new GameObject("TestPlayer");
        testPlayer.transform.position =
            npc.transform.position + Vector3.right * 100f;

        SetField("player", testPlayer.transform);
        SetField("ambushArea", null);
        SetField("healthDrainRate", 0f);
        SetField("health", 19f);
        SetField("ammo", 100);

        Time.timeScale = 1f;
    }

    [UnityTest]
    public IEnumerator LowHealthCollectsNearbyPackAndRestoresHealth()
    {
        testPack = new GameObject("TestHealthPack");
        testPack.tag = "NPCHealthPickup";
        testPack.transform.position =
            soldier.transform.position + Vector3.right;

        // Let the real Animator transitions and script run.
        for (int i = 0; i < 20; i++)
        {
            Tick();
        }

        float health = (float)soldier.GetType()
            .GetField("health").GetValue(soldier);

        Assert.That(
            health,
            Is.EqualTo(100f),
            "Collecting the pack should restore health to 100.");

        Assert.That(
            testPack.activeSelf,
            Is.False,
            "The collected health pack should be deactivated.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator LowHealthWithoutPacksEntersFleeState()
    {
        for (int i = 0; i < 20; i++)
        {
            Tick();
        }

        Assert.That(
            animator.GetBool("healthPackAvailable"),
            Is.False,
            "No health packs should be available.");

        Assert.That(
            animator.GetCurrentAnimatorStateInfo(0).IsName("flee"),
            Is.True,
            "Low health without a pack should trigger fleeing.");

        Assert.That(
            animator.GetBool("canShootPlayer"),
            Is.False,
            "The low-health soldier should not shoot.");

        yield return null;
    }

    private void SetField(string name, object value)
    {
        soldier.GetType().GetField(name).SetValue(soldier, value);
    }

    private void Tick()
    {
        soldier.GetType().GetMethod(
            "Update",
            BindingFlags.Instance | BindingFlags.NonPublic
        ).Invoke(soldier, null);

        animator.Update(0.1f);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(testPlayer);
        Object.Destroy(testPack);

        Time.timeScale = 1f;

        yield return null;
    }
}