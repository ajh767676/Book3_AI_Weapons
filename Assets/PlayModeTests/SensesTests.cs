using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SensesTests
{
    private MonoBehaviour senses;
    private Animator animator;
    private NavMeshAgent agent;
    private GameObject testPlayer;
    private GameObject wall;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        yield return SceneManager.LoadSceneAsync("senses");
        yield return null;

        GameObject soldier = GameObject.Find("SoldierNPC");

        Assert.That(soldier, Is.Not.Null);

        senses = soldier.GetComponent("SoldierSenses")
            as MonoBehaviour;

        Assert.That(senses, Is.Not.Null);

        // Run sensing manually so movement cannot affect the tests.
        senses.enabled = false;

        agent = soldier.GetComponent<NavMeshAgent>();
        animator = soldier.GetComponent<Animator>();

        Assert.That(agent.isOnNavMesh, Is.True);

        agent.isStopped = true;
        agent.speed = 0f;
        agent.updateRotation = false;
        animator.speed = 0f;

        testPlayer = new GameObject("TestPlayer");

        SetField("player", testPlayer.transform);
        SetField("hearingDistance", 3f);
        SetField("sightDistance", 10f);
        SetField("fieldOfView", 90f);

        Time.timeScale = 1f;
    }

    [UnityTest]
    public IEnumerator HearingDetectsNearbyPlayerBehindSoldier()
    {
        Transform soldier = senses.transform;

        testPlayer.transform.position =
            soldier.position - soldier.forward * 2f;

        RunSensing();

        Assert.That(
            animator.GetBool("canHearPlayer"),
            Is.True,
            "The soldier should hear a player two metres behind.");

        testPlayer.transform.position =
            soldier.position - soldier.forward * 4f;

        RunSensing();

        Assert.That(
            animator.GetBool("canHearPlayer"),
            Is.False,
            "The soldier should not hear beyond three metres.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator VisionRequiresPlayerInFrontAndNoWall()
    {
        Transform soldier = senses.transform;
        // Clear existing obstacles for this controlled vision test.
        // The test creates its own wall afterward.
        foreach (Collider obstacle in
                 Object.FindObjectsByType<Collider>(
                     FindObjectsSortMode.None))
        {
            if (obstacle.transform != soldier &&
                !obstacle.transform.IsChildOf(soldier))
            {
                obstacle.enabled = false;
            }
        }

        testPlayer.transform.position =
            soldier.position + soldier.forward * 6f;

        RunSensing();

        Assert.That(
            animator.GetBool("canSeePlayer"),
            Is.True,
            "The soldier should see a player in front.");

        testPlayer.transform.position =
            soldier.position - soldier.forward * 6f;

        RunSensing();

        Assert.That(
            animator.GetBool("canSeePlayer"),
            Is.False,
            "The soldier should not see behind himself.");

        testPlayer.transform.position =
            soldier.position + soldier.forward * 6f;

        wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "TestVisionWall";
        wall.transform.position =
            soldier.position + soldier.forward * 3f +
            Vector3.up * 1.5f;

        wall.transform.rotation = soldier.rotation;
        wall.transform.localScale = new Vector3(3f, 3f, 0.5f);

        RunSensing();

        Assert.That(
            animator.GetBool("canSeePlayer"),
            Is.False,
            "A wall should block the soldier's sight.");

        yield return null;
    }

    private void SetField(string name, object value)
    {
        senses.GetType().GetField(name).SetValue(senses, value);
    }

    private void RunSensing()
    {
        Physics.SyncTransforms();

        senses.GetType().GetMethod(
            "Update",
            BindingFlags.Instance | BindingFlags.NonPublic
        ).Invoke(senses, null);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(testPlayer);
        Object.Destroy(wall);

        Time.timeScale = 1f;

        yield return null;
    }
}