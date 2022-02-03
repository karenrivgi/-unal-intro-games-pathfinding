using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private Vector3 offset;

    // Setea el offset de la camara con respecto al jugador
    void Start()
    {
        offset = transform.position - target.transform.position;
    }

    // Cambiamos nuestra posicion por la del target mas la distancia al target para seguirlo
    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
