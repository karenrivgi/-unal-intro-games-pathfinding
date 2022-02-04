using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathfindingAStar : MonoBehaviour
{
    public Transform seeker, target;
    [SerializeField]
    private NodesGrid grid;
    [SerializeField]
    private CharacterMovement character;

    private List<Node> shortestPath = new List<Node>();

    public Action OnPathFoundEvent;
    public static Action OnGeneratePathEvent;
    public static Action OnUnreachableTargetEvent;
    public float tileAnimationMaxSpeed = 0.1f;
    private float tileAnimationSpeed = 0.1f;
    
    // Devuelve una lista de Vector3 que contiene las posiciones en el mundo de los nodos, para pasarselas
    // al robot y que pueda recorrer el camino mas corto para llegar al target
    public List<Vector3> GetShortestPathPoints()
    {
        //Si no se encontró un camino
        if(shortestPath == null || shortestPath.Count == 0)
        {
            return null;
        }

        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < shortestPath.Count; i++)
        {
            points.Add(shortestPath[i].worldPosition);
        }
        return points;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GeneratePath();
        }
    }
    

    public void GeneratePath()
    {
        OnGeneratePathEvent?.Invoke();
        StartCoroutine(FindPath(seeker.position, target.position));
        
    }

    public void ChangeTileAnimationSpeed(float speed) {

        tileAnimationSpeed = speed * tileAnimationMaxSpeed;
        
    }

    //Corrutina que encuentra el camino mas corto entre startPos y targetPos y muestra en juego los nodos evaluados
    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (!startNode.walkable || !targetNode.walkable)
        {
            OnUnreachableTargetEvent?.Invoke(); 
            shortestPath = null;
            yield break;
        }

        List<Node> openSet = new List<Node>(); // Nodos a evaluar
        List<Node> closedSet = new List<Node>(); // Nodos que han sido evaluados

        openSet.Add(startNode);

        while (openSet.Count > 0) //Mientras haya nodos para evaluar...
        {  
            Node currentNode = openSet[0]; //Toma el primero de la lista a evaluar
            for (int i = 1; i < openSet.Count; i++) //loop en todos los nodos de la lista a evaluar
            {
                Node nodeToEvaluate = openSet[i]; 
                if (nodeToEvaluate.FCost < currentNode.FCost || nodeToEvaluate.FCost == currentNode.FCost)
                {
                    //Si el nodo a evaluar tiene un Fcost mas bajo o igual que el currentNode
                    // Comparamos su hCost, para ver siesta mas cerca del targetNode
                    if (nodeToEvaluate.hCost < currentNode.hCost)
                    {
                        //Si ese nodo esta mas cerca, el nodo actual sera nodeToEvaluate, el nodo en el 
                        //OpenSet con el menor Fcost
                        currentNode = nodeToEvaluate;
                    }
                }
            }

            openSet.Remove(currentNode); 
            closedSet.Add(currentNode);

            currentNode.SetState(NodeStates.Evaluated); //El nodo se setea como evaluado
            yield return new WaitForSeconds(tileAnimationSpeed);

            if (currentNode == targetNode)
            {
                //Si el currentNode es el targetNode, llamamos a RetracePath y cambiamos los colores de los Tiles
                //para indicar cual es el nodo inicial, el Target, y los nodos que representan el camino mas corto al target
                shortestPath = RetracePath(startNode, targetNode);

                for (int i = 0; i < shortestPath.Count; i++)
                {
                    if (i == 0)
                    {
                        shortestPath[i].SetState(NodeStates.Initial);
                    }
                    else if (i == shortestPath.Count - 1)
                    {
                        shortestPath[i].SetState(NodeStates.Target);
                    }
                    else
                    {
                        shortestPath[i].SetState(NodeStates.Selected);
                        yield return new WaitForSeconds(tileAnimationSpeed);
                    }
                }

                OnPathFoundEvent?.Invoke();
                yield break;
            }

            //Por cada vecino del currentNode
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                //Si el vecino no es walkable o esta en el closedSet, lo pasamos
                if (neighbour.walkable == false || closedSet.Contains(neighbour))
                {
                    continue;
                }

                // Costo de moverse al vecino, gCost del current mas la distancia del current al vecino
                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                // Si el costo de moverme al vecino es menor que su gCost, o si el vecino no esta en el openSet
                if (newCostToNeighbour < neighbour.gCost || openSet.Contains(neighbour) == false)
                {
                    //gCost del vecino es el que calculamos arriba
                    neighbour.gCost = newCostToNeighbour;

                    //El hCost se halla con la distancia entre el nodo y el nodo target
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    //Seteamos el parent del vecino como el currentNode
                    neighbour.parent = currentNode;

                    //Si el vecino no esta en el openSet, lo agregamos
                    if (openSet.Contains(neighbour) == false)
                    {
                        openSet.Add(neighbour);
                        neighbour.SetState(NodeStates.Evaluated);
                        yield return new WaitForSeconds(tileAnimationSpeed);
                    }
                }
            }
        }

        OnUnreachableTargetEvent?.Invoke(); 
        shortestPath = null;
        yield break;
    }

    // El path se construye de atras para adelante, acá revertimos agregando los nodos parent a una lista hasta
    // que se llegue al startNode, ahí lo agregamos y hacemos path.reverse() 
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Add(startNode);
        path.Reverse();
        grid.path = path;

        return path;
    }

    // https://youtu.be/mZfyt03LDH4?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&t=841
    private int GetDistance(Node nodeA, Node nodeB)
    {
        // 14 costo de moverse en diagonal
        // 10 costo de moverse horizontalmente
        
        // distancia en X de del punto A al punto B
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);

        // distancia en Y de del punto A al punto B
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        //Si la distancia en X es mas grande que en Y
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY); //Movimiento en diagonal + (X-Y) movimiento horizontal

        return 14 * dstX + 10 * (dstY - dstX); //Movimiento en diagonal + (Y-X) movimiento vertical
    }
}
