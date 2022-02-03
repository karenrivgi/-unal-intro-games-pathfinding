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

    //Constructor, posicion de acuerdo a la grid generada en NodesGrid.cs
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
        //Marca como visitado el nodo actual
        visited = true;

        List<MazeNode> unvisitedNeighbours = new List<MazeNode>();

        // Revisa cuales vecinos no han sido visitados aun
        foreach (MazeNode node in neighbours)
        {
            if(node.visited == false)
            {
                unvisitedNeighbours.Add(node);
            }
        }

        // Si todos han sido visitados, retorna null
        if(unvisitedNeighbours.Count == 0)
        {
            return null;
        }
         
        // Si hay nodos sin visitar
        int randomDirection = Random.Range(0, unvisitedNeighbours.Count); //Devuelve un entero entre (0,unvisitedNeighbours.Count -1)
        MazeNode neighbourSelected = unvisitedNeighbours[randomDirection];
        DestroyWall(neighbourSelected); // Destruye la pared que hay entre el nodo actual y el vecino al que iremos
        return neighbourSelected;
    }

    //Si el vecino no ha sido visitado, destruye la "pared" que esta entre el nodo actual y el vecino de este
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
                    //Remueve la pared de la lista de parecdes activas de ambos
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
