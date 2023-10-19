using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabOnGrid : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Grid grid;
    public float moveDownInterval = 1.0f; // Adjust this value to control the speed of movement

    private List<GameObject> spawnedPrefabs = new List<GameObject>();
    private List<Vector2Int> filledCells = new List<Vector2Int>();
    private float timer = 0.0f;

    private void Update()
    {
        // Spawn pixel and snap it to the grid in a cell
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(mouseWorldPos);
            Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
            spawnPosition.z = 0; // Set the z-coordinate to 0

            GameObject newObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            spawnedPrefabs.Add(newObject);
            filledCells.Add(new Vector2Int(cellPosition.x, cellPosition.y));
        }

        // Move all filled cells one step down after a specific time interval
        timer += Time.deltaTime;
        if (timer >= moveDownInterval)
        {
            MoveCellsDown();
            timer = 0.0f; // Reset the timer
        }
    }

    private void MoveCellsDown()
    {
        for (int i = 0; i < filledCells.Count; i++)
        {
            Vector3Int cellPosition = new Vector3Int(filledCells[i].x, filledCells[i].y - 1, 0);
            Vector3 spawnPosition = grid.GetCellCenterWorld(cellPosition);
            spawnPosition.z = 0;

            bool hitPixel = false;
            foreach (var otherCell in filledCells)
            {
                if (cellPosition.x == otherCell.x && cellPosition.y == otherCell.y)
                {
                    hitPixel = true;
                    break;
                }
            }

            if (hitPixel)
            {
                Vector3Int leftCellPosition = new Vector3Int(cellPosition.x - 1, cellPosition.y, 0);
                Vector3Int rightCellPosition = new Vector3Int(cellPosition.x + 1, cellPosition.y, 0);

                bool canMoveLeft = !IsCellOccupied(leftCellPosition);
                bool canMoveRight = !IsCellOccupied(rightCellPosition);

                if (canMoveLeft)
                {
                    filledCells[i] = new Vector2Int(cellPosition.x - 1, cellPosition.y);
                    spawnedPrefabs[i].transform.position = grid.GetCellCenterWorld(new Vector3Int(cellPosition.x - 1, cellPosition.y, 0));
                }
                else if (canMoveRight)
                {
                    filledCells[i] = new Vector2Int(cellPosition.x + 1, cellPosition.y);
                    spawnedPrefabs[i].transform.position = grid.GetCellCenterWorld(new Vector3Int(cellPosition.x + 1, cellPosition.y, 0));
                }
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(spawnPosition, Vector2.down, grid.cellSize.y - 0.1f);

                if (hit.collider != null && hit.collider.CompareTag("Floor"))
                {
                    continue;
                }

                filledCells[i] = new Vector2Int(filledCells[i].x, filledCells[i].y - 1);
                spawnedPrefabs[i].transform.position = spawnPosition;
            }
        }
    }

    private bool IsCellOccupied(Vector3Int positionToCheck)
    {
        foreach (var cell in filledCells)
        {
            if (cell.x == positionToCheck.x && cell.y == positionToCheck.y)
            {
                return true;
            }
        }
        return false;
    }
}
