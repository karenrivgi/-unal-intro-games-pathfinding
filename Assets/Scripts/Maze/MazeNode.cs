using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeNode
{
    private List<Vector3> walls;
    public List<Vector3> activeWalls;
    private List<MazeNode> neighbours;

    public int gridPositionX;
    public int gridPositionY;

    public bool visited = false;
    public string ID
    {
        get
        {
            return gridPositionX.ToString() + gridPositionY.ToString();
        }
    }

    public MazeNode (int gridPositionX, int gridPositionY)
    {
        this.gridPositionX = gridPositionX;
        this.gridPositionY = gridPositionY;
    }

    public void SetNeighbours(List<MazeNode> neighbourNodes, List<Vector3> neighbourWalls)
    {
        neighbours = neighbourNodes;
        walls = neighbourWalls;
        activeWalls = new List<Vector3>(walls);
    }

    public MazeNode MoveToNextNode()
    {
        visited = true;
        List<MazeNode> unvisitedNeighbours = new List<MazeNode>();

        foreach (MazeNode node in neighbours)
        {
            if(node.visited == false)
            {
                unvisitedNeighbours.Add(node);
            }
        }

        if(unvisitedNeighbours.Count == 0)
        {
            return null;
        }

        int randomDirection = Random.Range(0, unvisitedNeighbours.Count);
        MazeNode neighbourSelected = unvisitedNeighbours[randomDirection];
        DestroyWall(neighbourSelected);
        return neighbourSelected;
    }

    private void DestroyWall(MazeNode neighbourSelected)
    {
        if(neighbourSelected == null)
        {
            return;
        }
        for (int i = 0; i < activeWalls.Count; i++)
        {
            for (int j = 0; j < neighbourSelected.activeWalls.Count; j++)
            {
                if (activeWalls[i] == neighbourSelected.activeWalls[j])
                {
                    activeWalls.RemoveAt(i);
                    neighbourSelected.activeWalls.RemoveAt(j);
                    return;
                }
            }
        }
    }

    public void ResetMazeNode()
    {
        visited = false;
        activeWalls = new List<Vector3>(walls);
    }
}
