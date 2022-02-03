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
        // XXXXXXXXXXX    Tanto X como O son nodos en NodesGrid, pero para generar el Maze, consideramos los
        // XOXOXOXOXOX    nodos X que estan arriba, abajo, a la derecha o a la izquierda de un nodo O como  
        // XXXXXXXXXXX    sus respectivas "paredes"
        // XOXOXOXOXOX
        // XXXXXXXXXXX

        //El tamaño de la grid que se puede caminar es la mitad de la grid generada en NodesGrid

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
                //Asignacion de los vecinos del nodo O (las O arriba, abajo, a la izquierda o a la derecha)
                //Asignacion de las paredes del nodo O (las X arriba, abajo, a la izquierda o a la derecha)
                node.SetNeighbours(GetNeighbours(node), GetNeighbourWallsPositions(node));
            }
        }
    }

    //Genera un laberinto con el algoritmo de recursive backtracking
    private void GenerateMaze()
    {
        currentNode = startNode.MoveToNextNode(); //Elige un punto en el cual iniciar, por defecto es nodesWalkable[0, 0]
        
        //Iteraremos este proceso hasta que el currentNode sea igual al StartNode
        while (currentNode.ID != startNode.ID)
        {
            //El siguiente nodo será un vecino escogido aleatoriamente, MoveToNextNode devuelve un nodo vecino que
            //aun no ha sido visitado
            currentNode = currentNode.MoveToNextNode();
            if(currentNode == null)
            {
                //Si devolvio nulo, el nodo actual no tenia vecinos sin visitar
                if(backTracking.Count == 0)
                {
                    break;
                }
                // Asi que nos devolvemos al ultimo nodo que fue visitado, para ver si este tiene algun nodo vecino
                // por visitar
                currentNode = backTracking.Pop();
                continue;
            }
            //Ponemos el nodo actual en la pila de nodos visitados
            backTracking.Push(currentNode);
        }

        //Stack push: poner en la pila
        //Stack pop: quitar el elemento que este encima de la pila

        //Por cada nodo en la lista de nodos en los que se puede caminar, se instanciara una "pared" por cada
        // "pared" activa que conserve el nodo
        foreach (MazeNode node in nodesWalkable)
        {
            foreach (Vector3 wallPosition in node.activeWalls)
            {
                InstantiateWall(wallPosition, gameObject.transform, mazeWalls);
            }
        } 
    }

    //Instancia las paredes que estan a las esquinas de los nodos en WalkableGrid
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

    //Instancia una pared en la posicion pasada, a no ser que ya haya una pared alli pero desactivada, en ese 
    //caso la activa, y guardamos la referencia a la "pared" en storeList
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

    // Instancia una "pared" en la posicion pasada como parámetro 
    public void InstantiateCustomWall(Vector3 position)
    {
        InstantiateWall(position, gameObject.transform, mazeWalls);
    }

    // "Esconde" una "pared" en la posicion pasada como parámetro
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

    //Obtiene las paredes vecinas del nodo pasado como parámetro en la grid base
    // 0X0  
    // XOX
    // 0X0
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

    //Retorna los nodos N que son vecinos en la WalkableGrid al nodo O pasado como parametro
    // XXXXXXXXXXX    
    // XOXNXNXNXOX  -> Visto desde la grid normal  
    // XXXXXXXXXXX    
    // XOXNXOXNXOX      //NNN
    // XXXXXXXXXXX      //NON -> visto desde la WalkableGrid
    // XOXNXNXNXOX      //NNN    
    // XXXXXXXXXXX

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

                    // Si es una coordenada en la grid valida y no se sale del tamano de la grid, la agregamos a los vecinos
                    if (checkX >= 0 && checkX < walkableGridSizeX && checkY >= 0 && checkY < walkableGridSizeY)
                    {
                        neighbours.Add(nodesWalkable[checkX, checkY]);
                    }
                }
            }
        }
        return neighbours;
    }

    //Retorna los nodos X que son diagonales a los nodos O ubicandolos desde el grid
    // X0X  
    // 0O0
    // X0X
    public List<Vector3> GetCornerWalls(MazeNode node)
    {
        List<Vector3> cornerWalls = new List<Vector3>();

        //Cambio de la posicion del MazeNode en la WalkableGrid a un Nodo en la grid generada en NodesGrid
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
        // Desactiva las "paredes" default, las esquinas
        foreach  (GameObject wall in defaultWalls)
        {
            wall.SetActive(false);
        }

        defaultWalls.Clear();

        //Desactiva y destruye las "paredes" que se crearon al generar el maze
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

    // Para generar un nuevo laberinto elimina cualquier laberinto anterior, reseteando los nodos y eliminando las
    // "paredes" creadas por el maze, pero poniendo las "paredes" por defecto
    public void GenerateNewMaze()
    {
        EliminateMaze();
        backTracking.Clear();

        GenerateMaze();

        if (defaultWalls.Count == 0)
        {
            GenerateCornerWalls();
        }
        else
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
