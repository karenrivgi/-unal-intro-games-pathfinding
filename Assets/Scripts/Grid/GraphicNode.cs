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
    
    // Modifica la escala del gameObject tileObject de acuerdo al size pasado como parametro
    public void Initialize(Node node, float size)
    {
        this.node = node;
        tileObject.transform.localScale = new Vector3(size, size, size); // z es el alto en este caso
        this.node.OnStateChangedEvent += OnNodeStateChange;
    }

    //Cambia el spriteColor del Nodo en base a su estado actual
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

            //Activa la animacion Show si el nodo no ha sido evaluado aun
            case NodeStates.Unevaluated:
                spriteColor.color = TileColors.unEvaluatedNodeColor;
                tileAnimator.SetBool("Hide", false);
                tileAnimator.SetBool("Show", true);
                break;

            //Activa la animacion Hide si el nodo es un obstaculo ("pared")
             case NodeStates.Obstacle:
                tileAnimator.SetBool("Show", false);
                tileAnimator.SetBool("Hide", true);
                break;
        }
    }
}
