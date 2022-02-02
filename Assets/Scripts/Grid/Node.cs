using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node 
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gCost;
    public int hCost;
    public Vector2Int gridPosition;
    public Node parent;
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

   public void SetState(NodeStates newState)
   {
       state = newState;
       OnStateChangedEvent?.Invoke(state);
   }

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
