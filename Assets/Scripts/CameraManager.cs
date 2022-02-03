using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Camera topDownCamera;
    [SerializeField]
    private Camera isometricCamera;

    public static Camera activeCamera = null;

    private void Awake()
    {
        topDownCamera.gameObject.SetActive(true);
        isometricCamera.gameObject.SetActive(false);
        activeCamera = topDownCamera;
    }

    // Cambia entre la camara isometrica y la top down
    public void ChangeCamera()
    {
        //Cambia el estado (activo) de las camaras de acuerdo al estado de la camara TopDown
        topDownCamera.gameObject.SetActive(!topDownCamera.gameObject.activeSelf);
        isometricCamera.gameObject.SetActive(!topDownCamera.gameObject.activeSelf);
        
        //Preguntamos si es topDownCamera la que esta activa y la guardamos en active Camera, si no, es isometricCamera
        activeCamera = topDownCamera.gameObject.activeSelf ? topDownCamera : isometricCamera;
    }
}
