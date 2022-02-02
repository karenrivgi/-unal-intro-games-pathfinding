using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knob : MonoBehaviour
{
    [SerializeField]
    private RectTransform movableKnob;
    [SerializeField]
    private float initialRotationDegrees;
    [SerializeField]
    private float maxRotationDegrees;
    [SerializeField]
    private bool inverted = false;

    public void RotateKnob(float rotationNormalized)
    {
        Quaternion newRotation = movableKnob.rotation;
        float rotation = inverted ? initialRotationDegrees - (Mathf.Abs(rotationNormalized - 1) * maxRotationDegrees) : initialRotationDegrees - (rotationNormalized * maxRotationDegrees);
        newRotation.eulerAngles = new Vector3(0, 0, rotation);
        movableKnob.rotation = newRotation;
    }
}
