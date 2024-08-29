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

                if (roomToSpawn != null)
                {
                    foundValidRoom = true;

                    // Generar halls para cada puerta
                    GenerateHall(openSide);

                    // Ajustar la posición de la nueva habitación para que quede al otro lado del hall
                    Vector3 newPosition = AdjustRoomPosition(transform.position, openSide);

                    // Instanciar la habitación en la posición correcta después del hall
                    Instantiate(roomToSpawn, newPosition, roomToSpawn.transform.rotation);
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


    private void GenerateHall(int side)
    {
        Vector3 hallPosition = transform.position;
        Quaternion hallRotation = Quaternion.identity;

        switch (side)
        {
            case 1: // bottom
                hallRotation = Quaternion.Euler(0, 90, 0);
                break;
            case 2: // top
                hallRotation = Quaternion.Euler(0, 90, 0);
                break;
            case 3: // left
                hallRotation = Quaternion.Euler(0, 0, 0);
                break;
            case 4: // right
                hallRotation = Quaternion.Euler(0, 0, 0);
                break;
        }

        Instantiate(templates.hall, hallPosition, hallRotation);
    }

    private Vector3 AdjustRoomPosition(Vector3 originalPosition, int side)
    {
        Vector3 adjustedPosition = originalPosition;

        switch (side)
        {
            case 1: // bottom, mover la nueva habitación hacia abajo
                adjustedPosition += new Vector3(0, 0, -10f);
                break;
            case 2: // top, mover la nueva habitación hacia arriba
                adjustedPosition += new Vector3(0, 0, 10f);
                break;
            case 3: // left, mover la nueva habitación hacia la izquierda
                adjustedPosition += new Vector3(-10f, 0, 0);
                break;
            case 4: // right, mover la nueva habitación hacia la derecha
                adjustedPosition += new Vector3(10f, 0, 0);
                break;
        }

        return adjustedPosition;
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
