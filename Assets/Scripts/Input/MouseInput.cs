using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{

    [SerializeField]
    private LayerMask layerTile;

    [SerializeField]
    private LayerMask layerObstacle;

    [SerializeField]
    private LayerMask layerTouchable;

    [SerializeField]
    private NodesGrid nodesGrid;

    [SerializeField]
    private MazeGenerator mazeGenerator;
    private Transform draggableObject;

    // Update is called once per frame
    void Update()
    {
        //Cuando se toca un objeto con layer "Touchable" con click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = CameraManager.activeCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 500, layerTouchable))
            {
                //Le asigna el transfom del objeto que fue tocado
                draggableObject = hit.transform;
            }
        }

        //Si al soltar el click izquierdo se tiene un draggableObject
        if(Input.GetMouseButtonUp(0))
        {
            if (draggableObject != null)
            {
                //Se suelta el objeto en el nodo que este mas cerca de su posicion actual
                draggableObject.position = nodesGrid.NodeFromWorldPoint(draggableObject.position).worldPosition;
                draggableObject = null;
            }
        }
        
        //Cuando se presiona con click izquierdo
        if (Input.GetMouseButton(0))
        {   
            //si se tiene un draggableObject
            if (draggableObject != null)
            {
                //Se cambia la posicion del draggable object en base a la posicion del mouse en la pantalla
                //Funciona bien con la camara Top Down
                Vector3 newPosition = CameraManager.activeCamera.ScreenToWorldPoint(Input.mousePosition);
                draggableObject.position = new Vector3(newPosition.x, 10, newPosition.z);
            }

            else
            {
                //Si se toco algo que tenia el layer Tile
                RaycastHit hit;
                var ray = CameraManager.activeCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 500, layerTile))
                { 
                    //Se obtiene el nodo correspondiente al lugar donde pego el raycast
                    Node node = nodesGrid.NodeFromWorldPoint(hit.point);
                     
                    //Si si hay un nodo en esa posicion
                    if (node != null)
                    {
                        //Se instancia una "pared" con collider en ese punto
                        mazeGenerator.InstantiateCustomWall(node.worldPosition);
                        node.ResetNode(false); //se marca el nodo como no walkable
                    }
                }
            }
        }

        //Cuando se toca una "pared" con click derecho, se "esconde" 
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            var ray = CameraManager.activeCamera.ScreenPointToRay(Input.mousePosition);

            //Verifica si toco algo con el layer Obstacle
            if (Physics.Raycast(ray, out hit, 500, layerObstacle))
            {
                //Se obtiene el nodo correspondiente al lugar donde pego el raycast
                Node node = nodesGrid.NodeFromWorldPoint(hit.point);

                //Si si hay un nodo en esa posicion
                if (node != null)
                {
                    mazeGenerator.HideCustomWall(node.worldPosition);
                    node.ResetNode(true);   //true indica que ese nodo ahora sera walkable
                }
            }
        }
    }
}
