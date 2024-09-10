using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openSide; // 1 -> bottom, 2 -> top, 3 -> left, 4 -> right
    public GameObject roomTemplate;
    public GameObject hall;
    private bool spawned = false;

    public static List<GameObject> invisibleDoors = new List<GameObject>(); // Lista para puertas invisibles

    // Contador estático para contar el número de habitaciones generadas
    public static int roomCount = 0;

    // Límite mínimo y máximo de habitaciones
    private int minRooms = 10;
    private int maxRooms = 20;

    // Probabilidad de cerrar una puerta
    public float closeDoorChance = 0.5f;
    private bool[] openDoors = new bool[4]; // Almacena el estado de las 4 puertas

    public void Initialize()
    {
        // Reinicia el estado de generación
        spawned = false;
        roomCount = 0; // Reinicia el contador de habitaciones generadas

        // Limpia la lista de puertas invisibles para que no se guarden las del laberinto anterior
        invisibleDoors.Clear();

        // Vuelve a configurar puertas abiertas de manera aleatoria
        SetRandomOpenDoors();

        // Reinicia la generación de habitaciones
        StartCoroutine(Spawn());

        Debug.Log("RoomSpawner reinicializado.");
    }


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
            if (openDoors[openSide - 1] || roomCount < minRooms)
            {
                Vector3 newPosition = AdjustRoomPosition(transform.position, openSide);

                if (IsPositionFree(newPosition))
                {
                    // Generamos el hall en el spawn point actual
                    GameObject newHall = GenerateHall(openSide);

                    // Instancia la habitación
                    GameObject newRoom = Instantiate(roomTemplate, newPosition, roomTemplate.transform.rotation);

                    // Invisibilizar las puertas conectadas al hall
                    HandleDoorsVisibility(newRoom, newHall);

                    // Cerrar puertas no conectadas
                    CloseNonConnectedDoors(newRoom);

                    roomCount++;
                    spawned = true;
                }
                else
                {
                    spawned = true; // Marca como generado si no se puede colocar la habitación
                }
            }
        }
    }

    private bool IsPositionFree(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, new Vector3(8, 0, 8));
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Room"))
            {
                return false;
            }
        }
        return true;
    }

    private GameObject GenerateHall(int side)
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

        return Instantiate(hall, hallPosition, hallRotation);
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

    private void HandleDoorsVisibility(GameObject newRoom, GameObject newHall)
    {
        // Busca todas las puertas dentro de RoomTemplate
        GameObject[] doorsInRoom = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject door in doorsInRoom)
        {
            // Verificar si la puerta está cerca del hall
            if (IsDoorNearHall(door, newHall))
            {
                // Si está alineada y en contacto, invisibilizar la puerta
                door.GetComponent<Collider>().enabled = false;
                door.GetComponent<MeshRenderer>().enabled = false;

                // Añadir la puerta a la lista de puertas invisibles
                invisibleDoors.Add(door);
            }
        }
    }

    // Verifica si la puerta está en contacto cercano con el hall
    private bool IsDoorNearHall(GameObject door, GameObject hall)
    {
        Collider doorCollider = door.GetComponent<Collider>();
        Collider hallCollider = hall.GetComponent<Collider>();

        // Usamos Physics.OverlapBox para verificar si hay solapamiento entre la puerta y el hall
        Collider[] colliders = Physics.OverlapBox(door.transform.position, doorCollider.bounds.extents);

        foreach (Collider col in colliders)
        {
            if (col == hallCollider) // Verificamos si la puerta está tocando el hall
            {
                return true;
            }
        }

        return false;
    }

    private void CloseNonConnectedDoors(GameObject newRoom)
    {
        // Busca todas las puertas dentro del RoomTemplate
        GameObject[] doorsInRoom = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject door in doorsInRoom)
        {
            // Si la puerta no está en la lista de invisibles, debería estar cerrada (visible)
            if (!invisibleDoors.Contains(door))
            {
                door.GetComponent<Collider>().enabled = true;
                door.GetComponent<MeshRenderer>().enabled = true;
            }
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
