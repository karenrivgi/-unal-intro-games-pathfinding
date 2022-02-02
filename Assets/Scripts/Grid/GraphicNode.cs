using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicNode : MonoBehaviour
{
    private Node node;
    [SerializeField]
    private SpriteRenderer spriteColor;
    [SerializeField]
    private GameObject tileObject;
    [SerializeField]
    private Animator tileAnimator;
    
    public void Initialize(Node node, float size)
    {
        this.node = node;
        tileObject.transform.localScale = new Vector3(size, size, size); // z es el alto en este caso
        this.node.OnStateChangedEvent += OnNodeStateChange;
    }

    private void OnNodeStateChange(NodeStates state)
    {
        switch(state)
        {
            case NodeStates.Evaluated :
                spriteColor.color = TileColors.evaluatedNodeColor;
                break;

            case NodeStates.Initial :
                spriteColor.color = TileColors.InitialNodeColor;
                break;

            case NodeStates.Selected:
                spriteColor.color = TileColors.selectedPathNodeColor;
                break;

            case NodeStates.Target :
                spriteColor.color = TileColors.targetNodeColor;
                break;

            case NodeStates.Unevaluated:
                spriteColor.color = TileColors.unEvaluatedNodeColor;
                tileAnimator.SetBool("Hide", false);
                tileAnimator.SetBool("Show", true);
                break;

             case NodeStates.Obstacle:
                tileAnimator.SetBool("Show", false);
                tileAnimator.SetBool("Hide", true);
                break;
        }
    }
}
