using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private CharacterMovement characterMovement;

    private void Awake()
    {
        characterMovement.OnCharacterArrivedEvent += OnCharacterArrivedListener;
        characterMovement.OnCharacterWalkingEvent += OnCharacterWalkingListener;
    }

    private void OnCharacterArrivedListener()
    {
        animator.SetTrigger("Idle");
    }

    private void OnCharacterWalkingListener()
    {
        animator.SetTrigger("Run");
    }
}
