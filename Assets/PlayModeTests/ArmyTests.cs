using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ArmyTests
{
    [UnitySetUp]
    public IEnumerator LoadArmyScene()
    {
        SceneManager.LoadScene("npc_army");
        yield return null;

        Time.timeScale = 0f;
    }

    [UnityTest]
    public IEnumerator DamageReducesHealth()
    {
        GameObject target = GameObject.Find("ArmyTarget");
        Assert.IsNotNull(target, "ArmyTarget is missing.");

        Component health = target.GetComponent("ArmyHealth");
        Assert.IsNotNull(health, "ArmyHealth is missing.");

        var type = health.GetType();

        type.GetMethod("TakeDamage").Invoke(
            health, new object[] { 20, null });

        int remainingHealth =
            (int)type.GetField("health").GetValue(health);

        Assert.AreEqual(80, remainingHealth);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LethalDamageDestroysTarget()
    {
        GameObject target = GameObject.Find("ArmyTarget");
        Assert.IsNotNull(target, "ArmyTarget is missing.");

        Component health = target.GetComponent("ArmyHealth");
        Assert.IsNotNull(health, "ArmyHealth is missing.");

        health.GetType().GetMethod("TakeDamage").Invoke(
            health, new object[] { 100, null });

        yield return null;

        Assert.IsTrue(target == null,
            "The target should be destroyed at zero health.");
    }

    [TearDown]
    public void RestoreTime()
    {
        Time.timeScale = 1f;
    }
}