using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openSide; // 1 -> bottom, 2 -> top, 3 -> left, 4 -> right
    private RoomTemplates templates;
    private bool spawned = false;

    void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(0.2f); // Espera para asegurarse de que otros SpawnPoints hayan hecho sus verificaciones

        if (!spawned)
        {
            GameObject roomToSpawn = null;
            int attempts = 0;
            bool foundValidRoom = false;

            while (attempts < 10 && !foundValidRoom)
            {
                roomToSpawn = GetRandomRoomByOppositeSide(openSide);

                if (roomToSpawn != null && IsValidRoom(roomToSpawn))
                {
                    foundValidRoom = true;
                    Instantiate(roomToSpawn, transform.position, roomToSpawn.transform.rotation);
                    spawned = true;
                }
                attempts++;
            }

            // Si no se encuentra una habitación válida en 10 intentos, marca como spawneado para evitar loops infinitos
            if (!foundValidRoom)
            {
                spawned = true;
            }
        }
    }

    private GameObject GetRandomRoomByOppositeSide(int side)
    {
        switch (side)
        {
            case 1: // bottom, necesita una puerta en la parte superior de la nueva habitación
                if (templates.topRooms.Length == 0) return null;
                return templates.topRooms[Random.Range(0, templates.topRooms.Length)];
            case 2: // top, necesita una puerta en la parte inferior de la nueva habitación
                if (templates.bottomRooms.Length == 0) return null;
                return templates.bottomRooms[Random.Range(0, templates.bottomRooms.Length)];
            case 3: // left, necesita una puerta en la parte derecha de la nueva habitación
                if (templates.rightRooms.Length == 0) return null;
                return templates.rightRooms[Random.Range(0, templates.rightRooms.Length)];
            case 4: // right, necesita una puerta en la parte izquierda de la nueva habitación
                if (templates.leftRooms.Length == 0) return null;
                return templates.leftRooms[Random.Range(0, templates.leftRooms.Length)];
            default:
                return null;
        }
    }

    private bool IsValidRoom(GameObject room)
    {
        // si la habitación tiene una puerta en el lado opuesto, es válida
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            if (!other.GetComponent<RoomSpawner>().spawned && !spawned)
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
            else if (!spawned)
            {
                Destroy(gameObject);
            }

            spawned = true;
        }
    }
}
