using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SimpleAIGameManager.instance.AddScore();
            Debug.Log("Collectible picked up!");
            Destroy(gameObject);
        }
    }
}