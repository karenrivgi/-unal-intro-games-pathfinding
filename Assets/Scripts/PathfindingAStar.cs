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
    public float tileAnimationMaxSpeed = 0.1f;
    private float tileAnimationSpeed = 0.1f;

    public List<Vector3> GetShortestPathPoints()
    {
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

    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) //Mientras haya nodos para evaluar...
        {
            Debug.Log("whileando");
            Node currentNode = openSet[0]; //Toma el primero de la lista a evaluar
            for (int i = 1; i < openSet.Count; i++)
            {
                Node nodeToEvaluate = openSet[i];
                if (nodeToEvaluate.FCost < currentNode.FCost || nodeToEvaluate.FCost == currentNode.FCost)
                {
                    if (nodeToEvaluate.hCost < currentNode.hCost)
                    {
                        currentNode = nodeToEvaluate;
                    }
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            currentNode.SetState(NodeStates.Evaluated);
            yield return new WaitForSeconds(tileAnimationSpeed);

            if (currentNode == targetNode)
            {
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

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (neighbour.walkable == false || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || openSet.Contains(neighbour) == false)
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (openSet.Contains(neighbour) == false)
                    {
                        openSet.Add(neighbour);
                        neighbour.SetState(NodeStates.Evaluated);
                        yield return new WaitForSeconds(tileAnimationSpeed);
                    }
                }
            }
        }

        Debug.Log("No puedo llegar :c");
        shortestPath = null;
        yield break;


    }

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

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
