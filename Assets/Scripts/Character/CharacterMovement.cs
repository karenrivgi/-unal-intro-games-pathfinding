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

    public void ChangeSpeed(float speed_multiplier)
    {
        _currentSpeed = max_speed * speed_multiplier;
    }

    private void OnPathFoundListener()
    {
        SetTargetPositions(pathFinding.GetShortestPathPoints());
    }

    public void SetTargetPositions(List<Vector3> positions)
    {
        _targetPositions = positions;
        if (_targetPositions == null)
        {
            Debug.Log("Ya estoy aqui");
            return;
        }

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
        if (!_hasArrived && _targetPositions != null)
        {
            _movementTime += Time.deltaTime * _currentSpeed;
            transform.position = Vector3.Lerp(_currentNode, _nextNode, _movementTime); 

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
        if(_nextNodeIndex < _targetPositions.Count){
            _currentNode = _nextNode;
            _nextNode = _targetPositions[_nextNodeIndex];
            transform.LookAt(_nextNode);
        } else
        {
            Debug.Log("Llegue :D");
            _hasArrived = true;
            OnCharacterArrivedEvent?.Invoke();
        }
    }
}
