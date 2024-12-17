using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public GameObject mazeBlockPrefab; 
    public Transform player; 
    public float transitionDistance = 5f; 

    private Queue<MazeBlock> activeBlocks = new Queue<MazeBlock>();
    private MazeBlock currentBlock;
    private MazeBlock nextBlock;

    void Start()
    {
        GenerateBlock();
    }

    void Update()
    {
        
        if (nextBlock != null && Vector3.Distance(player.position, nextBlock.transform.position) < transitionDistance)
        {
            TransitionToNextBlock();
        }
    }

    void GenerateBlock()
    {
        
        MazeBlock newBlock = Instantiate(mazeBlockPrefab, new Vector3(0, 0, activeBlocks.Count * 20), Quaternion.identity).GetComponent<MazeBlock>();
        activeBlocks.Enqueue(newBlock);

       
        if (activeBlocks.Count > 2)
        {
            Destroy(activeBlocks.Dequeue().gameObject);
        }

       
        if (currentBlock == null)
        {
            currentBlock = newBlock;
        }
        else
        {
            nextBlock = newBlock;
        }
    }

    void TransitionToNextBlock()
    {
        
        Destroy(currentBlock.gameObject);
        currentBlock = nextBlock;
        nextBlock = null;

        
        GenerateBlock();
    }
}

public class MazeBlock : MonoBehaviour
{
    public int width = 10; 
    public int height = 10; 
    public GameObject wallPrefab; 
    public GameObject floorPrefab; 
    
    private Cell[,] cells; 

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        cells = new Cell[width, height];

      
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new Cell(x, y);
            }
        }


        Stack<Cell> stack = new Stack<Cell>();
        Cell currentCell = cells[0, 0];
        currentCell.visited = true;

        do
        {
            List<Cell> unvisitedNeighbors = GetUnvisitedNeighbors(currentCell);
            if (unvisitedNeighbors.Count > 0)
            {
                Cell neighbor = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                RemoveWall(currentCell, neighbor);
                stack.Push(currentCell);
                currentCell = neighbor;
                currentCell.visited = true;
            }
            else if (stack.Count > 0)
            {
                currentCell = stack.Pop();
            }
        } while (stack.Count > 0);

        // Создание стен и пола
        CreateMazeVisual();
    }

    List<Cell> GetUnvisitedNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        
        if (cell.x > 0 && !cells[cell.x - 1, cell.y].visited) 
            neighbors.Add(cells[cell.x - 1, cell.y]);
            
        if (cell.x < width - 1 && !cells[cell.x + 1, cell.y].visited)
            neighbors.Add(cells[cell.x + 1, cell.y]);
            
        if (cell.y > 0 && !cells[cell.x, cell.y - 1].visited) 
            neighbors.Add(cells[cell.x, cell.y - 1]);
            
        if (cell.y < height - 1 && !cells[cell.x, cell.y + 1].visited)
            neighbors.Add(cells[cell.x, cell.y + 1]);

        return neighbors;
    }

    void RemoveWall(Cell current, Cell neighbor)
    {
       
        if (current.x < neighbor.x) 
        {
            current.rightWall = false;
            neighbor.leftWall = false;
        }
        else if (current.x > neighbor.x) 
        {
            current.leftWall = false;
            neighbor.rightWall = false;
        }
        else if (current.y < neighbor.y)
        {
            current.bottomWall = false;
            neighbor.topWall = false;
        }
        else if (current.y > neighbor.y)
        {
            current.topWall = false;
            neighbor.bottomWall = false;
        }
    }

    void CreateMazeVisual()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
               
                Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity);

                
                if (cells[x, y].topWall)
                    Instantiate(wallPrefab, new Vector3(x, 1, y + 0.5f), Quaternion.identity);
                    
                if (cells[x, y].bottomWall)
                    Instantiate(wallPrefab, new Vector3(x, 1, y - 0.5f), Quaternion.Euler(0, 180, 0));
                    
                if (cells[x, y].leftWall)
                    Instantiate(wallPrefab, new Vector3(x - 0.5f, 1, y), Quaternion.Euler(0, 90, 0));
                    
                if (cells[x, y].rightWall)
                    Instantiate(wallPrefab, new Vector3(x + 0.5f, 1, y), Quaternion.Euler(0, -90, 0));
            }
        }
    }
}

public class Cell
{
    public int x, y;
    public bool visited = false;
    public bool topWall = true;
    public bool bottomWall = true;
    public bool leftWall = true;
    public bool rightWall = true;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
