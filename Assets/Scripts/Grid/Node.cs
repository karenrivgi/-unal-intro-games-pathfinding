using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node 
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gCost; //distancia desde el nodo inicial
    public int hCost; //distancia desde el nodo final (heuristica)
    public Vector2Int gridPosition;
    public Node parent; //Nodo anterior a mi en el camino mas corto
    private NodeStates state = NodeStates.Unevaluated;

    public Action<NodeStates> OnStateChangedEvent;

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    // Constructor del nodo
   public Node (bool _walkable, Vector3 _worldPosition, Vector2Int positionGrid)
   {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridPosition = positionGrid;
   }

    //
   public void SetState(NodeStates newState)
   {
       state = newState;
       OnStateChangedEvent?.Invoke(state);
   }

    // Resetea el gCost y el hCost, y cambia la variable walkable de acuerdo a lo pasado como parametro
   public void ResetNode(bool _walkable)
   {
        walkable = _walkable;
        gCost = 0; 
        hCost = 0;

        if (_walkable)
        {
            SetState(NodeStates.Unevaluated);
        }
        else
        {
            SetState(NodeStates.Obstacle);
        }
   }
}
