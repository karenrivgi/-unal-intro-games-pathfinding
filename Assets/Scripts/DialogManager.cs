using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField]
    Text dialogText;
    [SerializeField]
    GameObject dialogBubble;
    [SerializeField]
    CharacterMovement characterMovement;
    [SerializeField]
    string arrivedMessage;
    [SerializeField]
    string unreachableMessage;

    void Awake()
    {
        dialogBubble.SetActive(false);
        characterMovement.OnCharacterArrivedEvent += OnCharacterArrived;
        PathfindingAStar.OnUnreachableTargetEvent += OnUnreachableTarget;
    }

    private IEnumerator ShowBubble(string message)
    {
        dialogText.text = message;
        dialogBubble.SetActive(true);
        yield return new WaitForSeconds(3f);
        dialogBubble.SetActive(false);
    }
    void OnCharacterArrived()
    {
        StartCoroutine(ShowBubble(arrivedMessage));
    }

    void OnUnreachableTarget()
    {
        StartCoroutine(ShowBubble(unreachableMessage));
    }

}
