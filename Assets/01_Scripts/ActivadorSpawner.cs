using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorSpawner : MonoBehaviour
{
    public float detectionRadius = 5f;  // Radio de detección del jugador
    public Transform leftPoint;  // El punto izquierdo del área de spawn
    public Transform rightPoint;  // El punto derecho del área de spawn
    public GameObject[] enemyPrefabs;  // Lista de prefabs de enemigos (Enemy, Kamikaze, Sniper)
    public float timeBtwSpawn = 1.5f;

    private float timer = 0f;
    private bool isSpawning = false;
    private int enemiesToSpawn;  // Número de enemigos a spawnear

    void Start()
    {
        enemiesToSpawn = Random.Range(1, 6);  // Elige un número aleatorio entre 1 y 5 al inicio
        Debug.Log("Number of enemies to spawn: " + enemiesToSpawn);
    }

    void Update()
    {
        // Detecta si el jugador está en el radio de detección
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        bool playerDetected = false;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && !isSpawning)
            {
                isSpawning = true;
                playerDetected = true;
                Debug.Log("Player detected. Spawning enemies...");
                break;  // Sale del bucle una vez que detecta al jugador
            }
        }

        if (!playerDetected)
        {
            Debug.Log("Player not detected within radius.");
        }

        // Si el spawner está activado, empieza a generar enemigos
        if (isSpawning && enemiesToSpawn > 0)
        {
            Debug.Log("Spawner is active, checking timer...");

            if (timer < timeBtwSpawn)
            {
                timer += Time.deltaTime;
                Debug.Log("Waiting for next spawn... Timer: " + timer);
            }
            else
            {
                timer = 0f;

                float x = Random.Range(leftPoint.position.x, rightPoint.position.x);
                float z = Random.Range(leftPoint.position.z, rightPoint.position.z);

                int randomIndex = Random.Range(0, enemyPrefabs.Length);

                Instantiate(enemyPrefabs[randomIndex], new Vector3(x, transform.position.y, z), Quaternion.identity);
                Debug.Log("Spawned an enemy at position: " + new Vector3(x, transform.position.y, z));

                enemiesToSpawn--;  // Disminuye el contador de enemigos a spawnear
                Debug.Log("Remaining enemies to spawn: " + enemiesToSpawn);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
