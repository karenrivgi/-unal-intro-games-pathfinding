using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColors : MonoBehaviour
{
    [SerializeField]
    private Color initialNode;
     [SerializeField]
    private Color targetNode;
     [SerializeField]
    private Color selectedNode;
     [SerializeField]
    private Color evaluatedNode;
    [SerializeField]
    private Color unEvaluatedNode;


    public static Color InitialNodeColor;
    public static Color targetNodeColor;
    public static Color selectedPathNodeColor;
    public static Color evaluatedNodeColor;
    public static Color unEvaluatedNodeColor;

    void Awake()
    {
        InitialNodeColor = initialNode;
        targetNodeColor = targetNode;
        selectedPathNodeColor = selectedNode;
        evaluatedNodeColor = evaluatedNode;
        unEvaluatedNodeColor = unEvaluatedNode;
    }
}
