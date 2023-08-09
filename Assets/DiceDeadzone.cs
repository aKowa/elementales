using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiceDeadzone : MonoBehaviour
{
    public UnityEvent<GameObject> sendDiceToRespawnPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dice"))
        {
            Debug.LogWarning("Dice went offscreen! Respawning it...");
            sendDiceToRespawnPosition.Invoke(other.gameObject);
        }
    }
}
