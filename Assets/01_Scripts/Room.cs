using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    bool killAllEnemies = false;
    public int enemiesInRoom = 0;  // Contador de enemigos en la habitación

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
                // Hacer invisible la puerta
                door.GetComponent<Collider>().enabled = false;
                door.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public void UpdateEnemyCount(bool isEnemyDead)
    {
        if (isEnemyDead)
        {
            enemiesInRoom--;  // Decrementa el contador de enemigos si uno muere
        }
        Debug.Log("ENEMIES IN ROOM" + enemiesInRoom);
        // Cambia el estado del booleano si no hay más enemigos
        if (enemiesInRoom <= 0)
        {
            killAllEnemies = true;
            Debug.Log("All enemies are dead. killAllEnemies is now: " + killAllEnemies);
            StartCoroutine(MakeDoorsInvisible());
        }
    }
}
