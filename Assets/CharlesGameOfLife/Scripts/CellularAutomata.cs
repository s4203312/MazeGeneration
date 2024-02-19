using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellularAutomata : MonoBehaviour
{
    //[x, y]
    [SerializeField]
    bool[,] grid;
    [SerializeField]
    private int seed;
    [SerializeField]
    private int rows;
    [SerializeField]
    private int columns;
    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private float timeStep = 0.2f;
    private bool isPaused = false;

    CellVisualiser[,] gridVisualisers;
    GameObject[,] gridObjects;
    [SerializeField]
    Transform gridParent;
    [SerializeField]
    GridLayoutGroup gLayout;

    Coroutine CoGameOfLife;

    private void Awake()
    {
        gridObjects = new GameObject[rows, columns];
        gridVisualisers = new CellVisualiser[rows, columns];
        grid = new bool[rows, columns];

        CreateGridObjects();
        Random.InitState(seed);
    }

    // Start is called before the first frame update
    void Start()
    {
        gLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gLayout.constraintCount = columns;

        gLayout.cellSize = new Vector2(1080 / rows, 1920 / columns);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isPaused = !isPaused;
        }
    }

    private void CreateGridObjects()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++) 
            {
                gridObjects[i, j] = Instantiate(cellPrefab, gridParent);
                gridObjects[i, j].name = "Cell" + "[" + i + "," + j + "]";
                gridVisualisers[i, j] = gridObjects[i, j].GetComponent<CellVisualiser>();
            }
        }
    }

    private void StartGame() 
    {
        if (CoGameOfLife != null)
        {
            StopCoroutine(CoGameOfLife);
            ClearGrid();
        }

        Debug.Log("Game Started");
        CoGameOfLife = StartCoroutine(Co_GameOfLife());
    }

    private IEnumerator Co_GameOfLife() 
    {
        isPaused = false;
        SetupGame();
        Debug.Log("Grid Randomised!");
        bool[,] newGrid = grid;

        while (true)
        {
            newGrid = ApplyRulesToGrid();
            yield return new WaitForEndOfFrame();
            grid = newGrid;
            UpdateGridObjects();
            yield return new WaitForSeconds(timeStep);
        }
    }

    private void SetupGame() 
    {
        Debug.Log("Randomising Grid!");

        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                int rand = Random.Range(0, 10);
                if(rand < 3)
                {
                    grid[i, j] = true;
                    gridVisualisers[i, j].SetCellActive(true);
                }
                else
                {
                    grid[i, j] = false;
                    gridVisualisers[i, j].SetCellActive(false);
                }
            }
        }
    }

    private bool[,] ApplyRulesToGrid()
    {
        bool[,] nextGrid = grid;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int n = GetNeighbourActiveCount(i, j);
                gridVisualisers[i, j].DisplayNeighbours(n);

                if(grid[i, j])
                {
                    if (n == 3 || n == 2)
                    {
                        nextGrid[i, j] = true;
                    }
                    else
                    {
                        nextGrid[i, j] = false;
                    }
                }
                else
                {
                    if (n == 3)
                    {
                        nextGrid[i, j] = true;
                    }
                }
            }
        }

        return nextGrid;
    }

    private int GetNeighbourActiveCount(int CellX, int CellY)
    {
        List<CellVisualiser> neighbours = new List<CellVisualiser>();

        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                // skip center cell
                if(i == j) continue;
                // skip rows out of range.
                if((i + CellX) < 0 || (i + CellX >= rows)) continue;
                // skip columns out of range.
                if((j + CellY) < 0 || (j + CellY >= columns)) continue;

                if(!grid[(i + CellX), (j + CellY)]) continue;

                // add to sum.
                neighbours.Add(gridVisualisers[(i + CellX), (j + CellY)]);
            }
        }
        return neighbours.Count;
    }

    private void UpdateGridObjects()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                gridVisualisers[i, j].SetCellActive(grid[i, j]);
            }
        }

        Debug.Log("Grid Updated!");
    }

    private void ClearGrid() 
    {
        Debug.Log("Clearing Grid!");
        grid = new bool[rows, columns];
        UpdateGridObjects();
    }
}
