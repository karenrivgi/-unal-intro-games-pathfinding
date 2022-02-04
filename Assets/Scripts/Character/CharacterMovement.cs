using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float max_speed = 10;
    [SerializeField]
    private PathfindingAStar pathFinding;

    private List<Vector3> _targetPositions = null;

    private Vector3 _currentNode;
    private Vector3 _nextNode;

    private float _movementTime = 0;
    private float _currentSpeed = 0;
    private int _nextNodeIndex = 1;
    private bool _hasArrived = false;

    public Action OnCharacterArrivedEvent = null;
    public Action OnCharacterWalkingEvent = null;

    private void Awake()
    {
        pathFinding.OnPathFoundEvent += OnPathFoundListener;
        _currentSpeed = 5;
    }

    //Cambia la velocidad actual en base al slider de velocidad que le manda un speed_multiplier de 0 a 1
    public void ChangeSpeed(float speed_multiplier)
    {
        _currentSpeed = max_speed * speed_multiplier;
    }

    //Si se encontró un Path, se Setean las posiciones que debe seguir el character para llegar al target
    private void OnPathFoundListener()
    {
        SetTargetPositions(pathFinding.GetShortestPathPoints());
    }

    //Asigna las posiciones a las que se movera el character
    public void SetTargetPositions(List<Vector3> positions)
    {
        _targetPositions = positions;
        if (_targetPositions == null)
        {
            return;
        }

        // Si podemos hacer un camino hasta el target
        if (_targetPositions.Count >= 2)
        {
            _currentNode = _targetPositions[0];
            _nextNode = _targetPositions[1];
            _nextNodeIndex = 1;
            _hasArrived = false;
            OnCharacterWalkingEvent?.Invoke();
        }
    }

    private void Update()
    {
        //Si el character no ha llegado y hay posiciones a las que ir
        if (!_hasArrived && _targetPositions != null)
        {

            _movementTime += Time.deltaTime * _currentSpeed; 
            
            // Va del currentNode al nextNode con la interpolacion _movementTime
            transform.position = Vector3.Lerp(_currentNode, _nextNode, _movementTime); 
            
            //Cuando llega a 1 significa que llego a _nextNode
            if (_movementTime >= 1)
            {
                _movementTime = 0;
                UpdateNodes();
            }
        }
    }

    private void UpdateNodes()
    {
        _nextNodeIndex += 1;

        // Si puedo ir a otro nodo
        if(_nextNodeIndex < _targetPositions.Count){
            _currentNode = _nextNode;
            _nextNode = _targetPositions[_nextNodeIndex];
            transform.LookAt(_nextNode); //Rota el character para que mire en direccion del siguiente nodo
        } 
        else
        {
            //Si estoy en _targetPositions[-1], he llegado
            _hasArrived = true;
            OnCharacterArrivedEvent?.Invoke();
        }
    }
}
