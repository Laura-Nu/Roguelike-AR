using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorSpawner : MonoBehaviour
{
    public float detectionRadius = 5f;
    public Transform leftPoint;
    public Transform rightPoint;
    public GameObject[] enemyPrefabs;
    public float timeBtwSpawn = 1.5f;
    public int enemiesToSpawn;

    private float timer = 0f;
    private bool isSpawning = false;
    private Room room;  // Referencia a la habitación

    void Start()
    {
        enemiesToSpawn = Random.Range(1, 6);
        Debug.Log("Number of enemies to spawn: " + enemiesToSpawn);

        // Encuentra la habitación más cercana o asigna la referencia de la habitación
        room = FindObjectOfType<Room>();
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        bool playerDetected = false;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && !isSpawning)
            {
                isSpawning = true;
                playerDetected = true;
                Debug.Log("Player detected. Spawning enemies...");
                break;
            }
        }

        if (!playerDetected)
        {
            Debug.Log("Player not detected within radius.");
        }

        if (isSpawning && enemiesToSpawn > 0)
        {
            if (timer < timeBtwSpawn)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0f;

                float x = Random.Range(leftPoint.position.x, rightPoint.position.x);
                float z = Random.Range(leftPoint.position.z, rightPoint.position.z);

                int randomIndex = Random.Range(0, enemyPrefabs.Length);

                GameObject enemy = Instantiate(enemyPrefabs[randomIndex], new Vector3(x, transform.position.y, z), Quaternion.identity);
                Debug.Log("Spawned an enemy at position: " + new Vector3(x, transform.position.y, z));

                enemiesToSpawn--;
                Debug.Log("Remaining enemies to spawn: " + enemiesToSpawn);

                if (room != null)
                {
                    room.enemiesInRoom++;  // Incrementa el contador de enemigos en la habitación
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
