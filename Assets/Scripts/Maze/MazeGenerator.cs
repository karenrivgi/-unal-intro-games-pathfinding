using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private NodesGrid grid;

    [SerializeField]
    private GameObject wallPrefab;

    private MazeNode[,] nodesWalkable;

    private int walkableGridSizeX;
    private int walkableGridSizeY;

    private MazeNode startNode;
    private MazeNode currentNode;

    private Stack<MazeNode> backTracking = new Stack<MazeNode>();

    private List<GameObject> defaultWalls = new List<GameObject>();
    private List<GameObject>  mazeWalls = new List<GameObject>();

    public static Action OnMazeChangedEvent;

    void Start()
    {
        InitializeMazeNodes();
        startNode = nodesWalkable[0, 0];        
    }

    private void InitializeMazeNodes()
    {
        walkableGridSizeX = Mathf.FloorToInt(grid.GridSizeX / 2f);
        walkableGridSizeY = Mathf.FloorToInt(grid.GridSizeY / 2f);
        nodesWalkable = new MazeNode[walkableGridSizeX, walkableGridSizeY];

        for (int x = 0; x < walkableGridSizeX; x++)
        {
            for (int y = 0; y < walkableGridSizeY; y++)
            {
                nodesWalkable[x, y] = new MazeNode(x, y);
            }
        }

        // Para poder setear los vecinos y las paredes, los nodos deben estar previamente creados
        for (int x = 0; x < Mathf.Floor(grid.GridSizeX / 2f); x++)
        {
            for (int y = 0; y < Mathf.Floor(grid.GridSizeY / 2f); y++)
            {
                MazeNode node = nodesWalkable[x, y];
                node.SetNeighbours(GetNeighbours(node), GetNeighbourWallsPositions(node));
            }
        }
    }

    private void GenerateMaze()
    {
        currentNode = startNode.MoveToNextNode();
        while (currentNode.ID != startNode.ID)
        {
            currentNode = currentNode.MoveToNextNode();
            if(currentNode == null)
            {
                if(backTracking.Count == 0)
                {
                    break;
                }
                
                currentNode = backTracking.Pop();
                continue;
            }
            backTracking.Push(currentNode);
        }

        foreach (MazeNode node in nodesWalkable)
        {
            foreach (Vector3 wallPosition in node.activeWalls)
            {
                InstantiateWall(wallPosition, gameObject.transform, mazeWalls);
            }
        } 
    }

    private void GenerateCornerWalls()
    {
        foreach (MazeNode node in nodesWalkable)
        {
            List<Vector3> cornerWalls = GetCornerWalls(node);
            foreach (Vector3 wallPosition in cornerWalls)
            {
                InstantiateWall(wallPosition, gameObject.transform, defaultWalls);
            }
        }
    }

    private void InstantiateWall(Vector3 position, Transform parent, List<GameObject> storeList)
    {
        foreach (GameObject item in storeList)
        {
            if(item.transform.position == position)
            {
                item.SetActive(true);
                return;
            }
        }
        GameObject wallGameObject = Instantiate(wallPrefab, position, Quaternion.identity);
        wallGameObject.transform.SetParent(parent);
        storeList.Add(wallGameObject);
    }

    public void InstantiateCustomWall(Vector3 position)
    {
        InstantiateWall(position, gameObject.transform, mazeWalls);
    }

    public void HideCustomWall(Vector3 position)
    {
        foreach (GameObject item in mazeWalls)
        {
            if(item.transform.position == position)
            {
                item.SetActive(false);
                return;
            }
        }
    }

    public List<Vector3> GetNeighbourWallsPositions(MazeNode node)
    {
        Node gridNode = grid.Grid[node.gridPositionX * 2, node.gridPositionY * 2];

        List<Node> neighbourNodes = grid.GetNeighbours(gridNode, false);
        List<Vector3> worldPoints = new List<Vector3>();

        foreach (Node neighbour in neighbourNodes)
        {
            worldPoints.Add(neighbour.worldPosition);
        }

        return worldPoints;
    }

    public List<MazeNode> GetNeighbours(MazeNode node)
    {
        List<MazeNode> neighbours = new List<MazeNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) //En ese caso seria el nodo al que le estamos buscando los vecinos
                    continue;

                if(x == 0 || y == 0)
                {
                    int checkX = node.gridPositionX + x; //Mi posicion en el grid en x + la x de la iteracion
                    int checkY = node.gridPositionY + y; //Mi posicion en el grid en y + la y de la iteracion

                    // Si es una coordenada en la grid valida y no se sale del tama�o de la grid, la agregamos a los vecinos
                    if (checkX >= 0 && checkX < walkableGridSizeX && checkY >= 0 && checkY < walkableGridSizeY)
                    {
                        neighbours.Add(nodesWalkable[checkX, checkY]);
                    }
                }
            }
        }
        return neighbours;
    }

    public List<Vector3> GetCornerWalls(MazeNode node)
    {
        List<Vector3> cornerWalls = new List<Vector3>();
        Node gridNode = grid.Grid[node.gridPositionX * 2, node.gridPositionY * 2];

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) //En ese caso seria el nodo al que le estamos buscando los vecinos
                    continue;

                if(x!= 0 && y!=0)
                {
                    int checkX = gridNode.gridPosition.x + x; //Mi posicion en el grid en x + la x de la iteracion
                    int checkY = gridNode.gridPosition.y + y; //Mi posicion en el grid en y + la y de la iteracion

                    // Si es una coordenada en la grid valida y no se sale del tamaño de la grid, la agregamos a los vecinos
                    if (checkX >= 0 && checkX < grid.GridSizeX && checkY >= 0 && checkY < grid.GridSizeY)
                    {
                        cornerWalls.Add(grid.Grid[checkX, checkY].worldPosition);
                    }
                }
            }
        }
        return cornerWalls;
    }

    public void EliminateMaze()
    {
        foreach  (GameObject wall in defaultWalls)
        {
            wall.SetActive(false);
        }

        defaultWalls.Clear();

        foreach (GameObject wall in mazeWalls)
        {
            wall.SetActive(false);
            Destroy(wall);
        }

        mazeWalls.Clear();
        backTracking.Clear();
        ResetMazeNodes();
        OnMazeChangedEvent?.Invoke();
    }

    public void GenerateNewMaze()
    {
        EliminateMaze();
        backTracking.Clear();

        GenerateMaze();

        if (defaultWalls.Count == 0)
        {
            GenerateCornerWalls();
        }else
        {
            foreach  (GameObject wall in defaultWalls)
            {
                wall.SetActive(true);
            }
        }
        
        OnMazeChangedEvent?.Invoke();
    }

    public void ResetMazeNodes()
    {
        foreach (MazeNode node in nodesWalkable)
        {
            node.ResetMazeNode();
        } 
    }
}
