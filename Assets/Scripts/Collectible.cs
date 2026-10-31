using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Collectible : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        FirstPersonController player =
            other.GetComponentInParent<FirstPersonController>();

        if (player != null)
        {
            if (TrainingCampManager.instance != null)
            {
                TrainingCampManager.instance.AddScore();
            }
            else if (SimpleAIGameManager.instance != null)
            {
                SimpleAIGameManager.instance.AddScore();
            }

            Debug.Log("Collectible picked up!");
            Destroy(gameObject);
        }
    }
}