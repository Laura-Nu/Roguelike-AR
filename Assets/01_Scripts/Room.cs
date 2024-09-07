using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool killAllEnemies = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(MakeDoorsVisibleAfterDelay());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && killAllEnemies)
        {
            StartCoroutine(MakeDoorsInvisible());
        }
    }

    IEnumerator MakeDoorsVisibleAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        // Accede a la lista de puertas invisibles directamente desde RoomSpawner
        foreach (GameObject door in RoomSpawner.invisibleDoors)
        {
            if (door != null)
            {
                // Hacer visible la puerta
                door.GetComponent<Collider>().enabled = true;
                door.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    IEnumerator MakeDoorsInvisible()
    {
        yield return new WaitForSeconds(1f);

        // Accede a la lista de puertas invisibles directamente desde RoomSpawner
        foreach (GameObject door in RoomSpawner.invisibleDoors)
        {
            if (door != null)
            {
                // Hacer visible la puerta
                door.GetComponent<Collider>().enabled = false;
                door.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
