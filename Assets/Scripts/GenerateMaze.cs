using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    public GameObject wallPrefab;

    // 1 = wall, 0 = empty space
    private int[,] worldMap =
    {
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 1, 1, 1, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 1 }
    };

    void Start()
    {
        if (wallPrefab == null)
            return;

        for (int row = 0; row < worldMap.GetLength(0); row++)
        {
            for (int column = 0;
                 column < worldMap.GetLength(1);
                 column++)
            {
                if (worldMap[row, column] == 1)
                {
                    Vector3 position = new Vector3(
                        (column - 3) * 2f,
                        1f,
                        (row - 2) * 2f
                    );

                    Instantiate(
                        wallPrefab,
                        position,
                        Quaternion.identity,
                        transform
                    );
                }
            }
        }
    }
}