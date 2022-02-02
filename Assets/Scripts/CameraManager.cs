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

    public void ChangeCamera()
    {
        topDownCamera.gameObject.SetActive(!topDownCamera.gameObject.activeSelf);
        isometricCamera.gameObject.SetActive(!topDownCamera.gameObject.activeSelf);

        activeCamera = topDownCamera.gameObject.activeSelf ? topDownCamera : isometricCamera;
    }
}
