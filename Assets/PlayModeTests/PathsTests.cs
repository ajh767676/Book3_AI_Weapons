using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PathsTests
{
    private Component patrol;
    private Transform[] waypoints;
    private FieldInfo currentPointField;
    private FieldInfo randomPatrolField;
    private MethodInfo chooseNextPoint;
    private Random.State savedRandomState;

    [UnitySetUp]
    public IEnumerator LoadPathsScene()
    {
        savedRandomState = Random.state;

        yield return SceneManager.LoadSceneAsync("paths");
        yield return null;

        GameObject soldier = GameObject.Find("SoldierNPC");
        Assert.That(soldier, Is.Not.Null,
            "The scene must contain SoldierNPC.");

        patrol = soldier.GetComponent("SoldierPatrol");
        Assert.That(patrol, Is.Not.Null,
            "SoldierNPC needs the SoldierPatrol component.");

        // Pause movement while checking waypoint selection.
        Time.timeScale = 0f;

        var type = patrol.GetType();

        currentPointField = type.GetField(
            "currentPoint",
            BindingFlags.Instance | BindingFlags.NonPublic);

        randomPatrolField = type.GetField("randomPatrol");

        chooseNextPoint = type.GetMethod(
            "ChooseNextPoint",
            BindingFlags.Instance | BindingFlags.NonPublic);

        waypoints = (Transform[])type.GetField("waypoints")
            .GetValue(patrol);

        Assert.That(waypoints, Is.Not.Null);
        Assert.That(waypoints.Length, Is.GreaterThanOrEqualTo(2));

        foreach (Transform waypoint in waypoints)
        {
            Assert.That(waypoint, Is.Not.Null,
                "Each waypoint must be assigned.");
        }

        currentPointField.SetValue(patrol, 0);
    }

    [Test]
    public void FixedPatrolVisitsWaypointsInOrderAndLoops()
    {
        randomPatrolField.SetValue(patrol, false);

        for (int step = 1; step <= waypoints.Length; step++)
        {
            chooseNextPoint.Invoke(patrol, null);

            int actual = (int)currentPointField.GetValue(patrol);
            int expected = step % waypoints.Length;

            Assert.That(actual, Is.EqualTo(expected),
                "Fixed patrol should advance in order and loop to zero.");
        }
    }

    [Test]
    public void RandomPatrolChoosesValidPointWithoutImmediateRepeat()
    {
        randomPatrolField.SetValue(patrol, true);
        Random.InitState(12345);

        for (int attempt = 0; attempt < 50; attempt++)
        {
            int previous = (int)currentPointField.GetValue(patrol);

            chooseNextPoint.Invoke(patrol, null);

            int next = (int)currentPointField.GetValue(patrol);

            Assert.That(next,
                Is.InRange(0, waypoints.Length - 1));

            Assert.That(next, Is.Not.EqualTo(previous),
                "Random patrol should choose a different waypoint.");
        }
    }

    [TearDown]
    public void RestoreState()
    {
        Random.state = savedRandomState;
        Time.timeScale = 1f;
    }
}
