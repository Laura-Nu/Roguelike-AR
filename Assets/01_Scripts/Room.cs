using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    bool killAllEnemies = false;
    public int enemiesInRoom = 0;
    ActivadorSpawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(MakeDoorsVisibleAfterDelay());
        }
        if (other.CompareTag("Enemy"))
        {
            spawner = GameObject.FindWithTag("Spawner").GetComponent<ActivadorSpawner>();

            if (spawner != null)
            {
                enemiesInRoom = spawner.sendenemies(); // Llamar al método de ActivadorSpawner
            }
            Debug.Log("Trigger: " + enemiesInRoom);
            // Asignar esta habitación al enemigo
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetRoom(this);  // Asigna la referencia del Room al enemigo
            }
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

    public void UpdateEnemyCount()
    {
        enemiesInRoom--;
        Debug.Log("ENEMIES IN ROOM DIE" + enemiesInRoom);
        // Cambia el estado del booleano si no hay más enemigos
        if (enemiesInRoom <= 0)
        {
            killAllEnemies = true;
            Debug.Log("All enemies are dead. killAllEnemies is now: " + killAllEnemies);
            StartCoroutine(MakeDoorsInvisible());
        }
    }
}
