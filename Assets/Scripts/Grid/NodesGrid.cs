using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesGrid : MonoBehaviour
{
    [SerializeField]
    private LayerMask obstacleLayer;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    [SerializeField]
    private GameObject tileObject;
    private Node[,] grid; // Matriz que recibe dos parametros y devuelve una posicion
    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;

    public Node[,] Grid
    {
        get
        {
            return grid;
        }
    }

    public int GridSizeX
    {
        get
        {
            return gridSizeX;
        }
    }

    public int GridSizeY
    {
        get
        {
            return gridSizeY;
        }
    }

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        
        MazeGenerator.OnMazeChangedEvent += ResetNodes;
        PathfindingAStar.OnGeneratePathEvent += ResetNodes;
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY]; // tamaño de la matriz
        //Obtenemos la esquina superior izquierda del grid
        Vector3 worldUpLeft = transform.position - Vector3.right * gridWorldSize.x/2 + Vector3.forward * gridWorldSize.y/2 ;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Obtiene la posicion del siguiente nodo
                Vector3 worldPoint = worldUpLeft + Vector3.right * (x * nodeDiameter + nodeRadius) - Vector3.forward * (y * nodeDiameter + nodeRadius);
                
                // Revisa si el nodo en el que esta tiene un obstaculo, si no lo tiene es walkable
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, obstacleLayer));
                
                // Crea un vector con la posicion dentro de la matriz grid
                Vector2Int gridPosition = new Vector2Int(x, y);
                Node node = new Node(walkable, worldPoint, gridPosition);
                grid [x,y] = node;

                InstantiateGraphicNode(node, worldPoint);
            }
        }
    }

    //Resetea los valores de todos los nodos de la grid, verificando si son walkable o no
    private void ResetNodes()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node node = grid[x,y];
                node.ResetNode(!(Physics.CheckSphere(node.worldPosition, nodeRadius, obstacleLayer)));
            }
        }
    }

    // Permite ver los nodos al instanciar un objeto Tile en la posicion de cada nodo
    private void InstantiateGraphicNode(Node node, Vector3 worldPoint)
    {
        GameObject tile = Instantiate(tileObject, worldPoint, gameObject.transform.rotation);
        tile.transform.SetParent(transform);
        GraphicNode graphicNode =  tile.GetComponent<GraphicNode>();
        graphicNode.Initialize(node, nodeDiameter);
    }

    // Obtiene los vecinos del nodo x // 000  ubicandolos desde el grid, y los retorna
                                      // 0x0
                                      // 000
    
    //El bool indica si queremos que se tomen las diagonales o no
    public List<Node> GetNeighbours(Node node, bool diagonal = true)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) //En ese caso seria el nodo al que le estamos buscando los vecinos
                    continue;

                if(diagonal == false && (x != 0 && y != 0)) //Si no queremos contar las diagonales
                {
                    continue;
                }

                int checkX = node.gridPosition.x + x; //Mi posicion en el grid en x + la x de la iteracion
                int checkY = node.gridPosition.y + y; //Mi posicion en el grid en y + la y de la iteracion

                // Si es una coordenada en la grid valida y no se sale del tamaño de la grid, la agregamos a los vecinos
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    //Permite retornar el nodo correspondiente a la posicion pasada*
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (-worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y; //
        
        percentX = Mathf.Clamp(percentX, 0, 1);
        percentY = Mathf.Clamp(percentY, 0, 1);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y]; 
    }

    public List<Node> path;

    //Para ver en escena el grid
    void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = (node.walkable)? Color.white : Color.red;
                if (path != null)
                {
                    if (path.Contains(node))
                    {
                        Gizmos.color = Color.black;
                    }      
                }
                if(node == grid[5, 5])
                {
                    Gizmos.color = Color.blue;
                }
                    
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
