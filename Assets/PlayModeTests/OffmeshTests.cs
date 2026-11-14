using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class OffmeshTests
{
    private Vector3 start;
    private Vector3 destination;
    private int jumpArea;

    [UnitySetUp]
    public IEnumerator LoadOffmeshScene()
    {
        yield return SceneManager.LoadSceneAsync("offmesh");
        yield return null;

        jumpArea = NavMesh.GetAreaFromName("Jump");

        Assert.That(jumpArea, Is.GreaterThanOrEqualTo(0),
            "The Jump area must exist.");

        bool foundStart = NavMesh.SamplePosition(
            new Vector3(-109f, 0f, 12f),
            out NavMeshHit startHit, 1f, NavMesh.AllAreas);

        bool foundDestination = NavMesh.SamplePosition(
            new Vector3(-114f, 0f, 12f),
            out NavMeshHit destinationHit, 1f, NavMesh.AllAreas);

        Assert.That(foundStart, Is.True,
            "The starting platform needs a baked NavMesh.");

        Assert.That(foundDestination, Is.True,
            "The destination platform needs a baked NavMesh.");

        start = startHit.position;
        destination = destinationHit.position;
    }

    [Test]
    public void JumpAllowedCreatesCompletePathAcrossGap()
    {
        NavMeshPath path = new NavMeshPath();

        bool foundPath = NavMesh.CalculatePath(
            start, destination, NavMesh.AllAreas, path);

        Assert.That(foundPath, Is.True);
        Assert.That(path.status,
            Is.EqualTo(NavMeshPathStatus.PathComplete),
            "The Jump link should connect the two platforms.");
    }

    [Test]
    public void JumpExcludedPreventsCompletePathAcrossGap()
    {
        int maskWithoutJump =
            NavMesh.AllAreas & ~(1 << jumpArea);

        NavMeshPath path = new NavMeshPath();

        bool foundPath = NavMesh.CalculatePath(
            start, destination, maskWithoutJump, path);

        bool completePath = foundPath &&
            path.status == NavMeshPathStatus.PathComplete;

        Assert.That(completePath, Is.False,
            "The gap should be unreachable when Jump is excluded.");
    }

    [TearDown]
    public void RestoreTimeScale()
    {
        Time.timeScale = 1f;
    }
}
