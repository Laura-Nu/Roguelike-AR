using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyRoom : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Encontrar el UI del item y habilitarlo
            ItemUI itemUI = other.GetComponentInChildren<ItemUI>();
            if (itemUI != null)
            {
                itemUI.SetUIVisibility(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Encontrar el UI del item y deshabilitarlo
            ItemUI itemUI = other.GetComponentInChildren<ItemUI>();
            if (itemUI != null)
            {
                itemUI.SetUIVisibility(false);
            }
        }
    }
}
