using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openSide; // 1 -> bottom, 2 -> top, 3 -> left, 4 -> right
    public GameObject roomTemplate;
    public GameObject hall;
    private bool spawned = false;

    // Contador estático para contar el número de habitaciones generadas
    public static int roomCount = 0;

    // Límite mínimo y máximo de habitaciones
    private int minRooms = 10;
    private int maxRooms = 20;

    // Probabilidad de cerrar una puerta
    public float closeDoorChance = 0.5f;
    private bool[] openDoors = new bool[4]; // Almacena el estado de las 4 puertas

    void Start()
    {
        SetRandomOpenDoors();
        StartCoroutine(Spawn());
    }

    void SetRandomOpenDoors()
    {
        for (int i = 0; i < 4; i++)
        {
            openDoors[i] = Random.value > closeDoorChance;
        }
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(0.2f); // Espera para asegurarse de que otros SpawnPoints hayan hecho sus verificaciones

        if (!spawned && roomCount < maxRooms)
        {
            GameObject roomToSpawn = null;
            int attempts = 0;
            bool foundValidRoom = false;

            while (attempts < 10 && !foundValidRoom)
            {
                if (openDoors[openSide - 1])
                {
                    roomToSpawn = roomTemplate;

                    if (roomToSpawn != null)
                    {
                        Vector3 newPosition = AdjustRoomPosition(transform.position, openSide);

                        if (IsPositionFree(newPosition))
                        {
                            foundValidRoom = true;
                            GenerateHall(openSide);
                            Instantiate(roomToSpawn, newPosition, roomToSpawn.transform.rotation);
                            roomCount++;
                            spawned = true;
                        }
                    }
                }
                attempts++;
            }

            if (!foundValidRoom)
            {
                spawned = true;
            }
        }
    }

    private bool IsPositionFree(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, new Vector3(9, 9, 9));
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Room"))
            {
                return false;
            }
        }
        return true;
    }

    private void GenerateHall(int side)
    {
        Vector3 hallPosition = transform.position;
        Quaternion hallRotation = Quaternion.identity;

        switch (side)
        {
            case 1: hallRotation = Quaternion.Euler(0, 90, 0); break;
            case 2: hallRotation = Quaternion.Euler(0, 90, 0); break;
            case 3: hallRotation = Quaternion.Euler(0, 0, 0); break;
            case 4: hallRotation = Quaternion.Euler(0, 0, 0); break;
        }

        Instantiate(hall, hallPosition, hallRotation);
    }

    private Vector3 AdjustRoomPosition(Vector3 originalPosition, int side)
    {
        Vector3 adjustedPosition = originalPosition;

        switch (side)
        {
            case 1: adjustedPosition += new Vector3(0, 0, -18f); break;
            case 2: adjustedPosition += new Vector3(0, 0, 18f); break;
            case 3: adjustedPosition += new Vector3(-18f, 0, 0); break;
            case 4: adjustedPosition += new Vector3(18f, 0, 0); break;
        }

        return adjustedPosition;
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
