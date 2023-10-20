using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TilemapClick : MonoBehaviour
{
    public float cellSize = 0.1f;
    public GameObject prefabToSpawn;
    public GameObject floor;
    private bool isDrawing = false;
    private HashSet<Vector3> settledCells = new();
    private HashSet<GameObject> movingCells = new();
    public TextMeshProUGUI textMeshPro;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }

        if (isDrawing)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 gridPosition = GetGridPosition(mouseWorldPos);

            if (!IsCellOccupied(gridPosition))
            {
                GameObject newPrefab = Instantiate(prefabToSpawn, gridPosition, Quaternion.identity);
                movingCells.Add(newPrefab);
            }
        }

        HashSet<GameObject> toRemove = new HashSet<GameObject>();

        foreach (var prefab in movingCells)
        {
            Vector3 currentPos = prefab.transform.position;
            Vector3 cellBelow = new(currentPos.x, currentPos.y - cellSize, currentPos.z);

            if (CanMoveTo(cellBelow))
            {
                prefab.transform.position = cellBelow;
            }
            else
            {
                Vector3 leftDiagonal = new(currentPos.x - cellSize, currentPos.y - cellSize, currentPos.z);
                Vector3 rightDiagonal = new(currentPos.x + cellSize, currentPos.y - cellSize, currentPos.z);

                if (CanMoveTo(leftDiagonal))
                {
                    prefab.transform.position = leftDiagonal;
                }
                else if (CanMoveTo(rightDiagonal))
                {
                    prefab.transform.position = rightDiagonal;
                }
                else
                {
                    toRemove.Add(prefab);
                    settledCells.Add(currentPos);
                }
            }
        }

        foreach (var removePrefab in toRemove)
        {
            movingCells.Remove(removePrefab);
        }

        textMeshPro.SetText("particles: " + settledCells.Count);
    }

    bool IsCellOccupied(Vector3 position)
    {
        return settledCells.Contains(position);
    }

    bool CanMoveTo(Vector3 position)
    {
        return !IsCellOccupied(position) && position.y > floor.transform.position.y + cellSize;
    }

    Vector3 GetGridPosition(Vector3 worldPosition)
    {
        return new Vector3(
            Mathf.Floor(worldPosition.x / cellSize) * cellSize + cellSize / 2,
            Mathf.Floor(worldPosition.y / cellSize) * cellSize + cellSize / 2,
            0f
        );
    }
}
