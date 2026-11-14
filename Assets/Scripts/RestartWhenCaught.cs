using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartWhenCaught : MonoBehaviour
{
    public Transform player;
    public float catchDistance = 2.3f;

    void Update()
    {
        if (Time.timeScale == 0f || player == null)
            return;

        Vector3 difference = player.position - transform.position;
        difference.y = 0f;

        if (difference.magnitude <= catchDistance)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}