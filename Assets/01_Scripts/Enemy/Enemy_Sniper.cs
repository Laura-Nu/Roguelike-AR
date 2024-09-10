using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sniper : MonoBehaviour
{
    public float life = 2f;
    public float detectionRange = 20f; // Rango de detección del jugador
    public float rotationSpeed = 5f; // Velocidad de rotación
    public float moveSpeed = 3f; // Velocidad de movimiento del enemigo
    public float timeBtwShoot = 2.3f; // Tiempo entre disparos
    public Transform firePoint; // Punto desde donde se disparan las balas
    public GameObject bulletPrefab; // Prefab de la bala del enemigo
    public float shootInterval = 0.1f; // Intervalo entre disparos de balas
    public GameObject coinPrefab; // Prefab de la moneda que se va a spawnear

    private Transform player;
    private float shootTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            RotateTowardsPlayer();
            MoveTowardsPlayer();
            Shoot(); // Disparar hacia el jugador
        }
    }

    // Método para girar hacia el jugador
    void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Asegurar que la dirección sea horizontal en el plano X-Z

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Método para mover hacia el jugador
    void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Asegurar que el movimiento sea horizontal

        // Mover al enemigo hacia el jugador
        transform.position += directionToPlayer.normalized * moveSpeed * Time.deltaTime;
    }

    // Método para disparar balas
    void Shoot()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= timeBtwShoot)
        {
            shootTimer = 0f;

            // Iniciar la corrutina para disparar 3 balas seguidas
            StartCoroutine(ShootMultipleBullets());
        }
    }

    // Corrutina para disparar 3 balas seguidas
    IEnumerator ShootMultipleBullets()
    {
        for (int i = 0; i < 3; i++)
        {
            if (firePoint != null && bulletPrefab != null)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            }
            yield return new WaitForSeconds(shootInterval); // Esperar antes de disparar la siguiente bala
        }
    }

    // Método para recibir daño
    public void TakeDamage(float damage)
    {
        life -= damage;
        if (life <= 0)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Die();
        }
    }

    void Die()
    {
        Debug.Log("El enemigo ha muerto."); // Mensaje al morir el enemigo
        Destroy(gameObject);
    }

    // Detección de colisiones
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(1f);
            }

            // Spawnear la moneda al colisionar con el jugador
            if (coinPrefab != null)
            {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject); // Destruir el enemigo después de spawnear la moneda
        }
    }
}