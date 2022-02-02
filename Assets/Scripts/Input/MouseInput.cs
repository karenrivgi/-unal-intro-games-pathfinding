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
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = CameraManager.activeCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 500, layerTouchable))
            {
                draggableObject = hit.transform;
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            if (draggableObject != null)
            {
                draggableObject.position = nodesGrid.NodeFromWorldPoint(draggableObject.position).worldPosition;
                draggableObject = null;
            }
        }
        

        if (Input.GetMouseButton(0))
        {
            if (draggableObject != null)
            {
                Vector3 newPosition = CameraManager.activeCamera.ScreenToWorldPoint(Input.mousePosition);
                draggableObject.position = new Vector3(newPosition.x, 10, newPosition.z);
            }

            else
            {
                RaycastHit hit;
                var ray = CameraManager.activeCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 500, layerTile))
                { 
                    Node node = nodesGrid.NodeFromWorldPoint(hit.point);
                    if (node != null)
                    {
                        mazeGenerator.InstantiateCustomWall(node.worldPosition);
                        node.ResetNode(false);
                    }
                }
            }
        }

        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            var ray = CameraManager.activeCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 500, layerObstacle))
            {
                Node node = nodesGrid.NodeFromWorldPoint(hit.point);

                if (node != null)
                {
                    mazeGenerator.HideCustomWall(node.worldPosition);
                    node.ResetNode(true);
                }
            }
        }
    }
}
