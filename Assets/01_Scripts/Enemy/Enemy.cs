using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float life = 2f;
    public float detectionRange = 20f; // Rango de detección del jugador
    public float rotationSpeed = 5f; // Velocidad de rotación
    public float timeBtwShoot = 2.3f; // Tiempo entre disparos
    public Transform firePoint; // Punto desde donde se disparan las balas
    public GameObject bulletPrefab; // Prefab de la bala del enemigo
    public float shootInterval = 0.1f; // Intervalo entre disparos de balas

    private Transform player;
    private float shootTimer = 0f;
    private Room room;
    private bool isTakingCover = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && !isTakingCover)
        {
            RotateTowardsPlayer();
            Shoot(); // Disparar hacia el jugador
        }

        // Buscar cobertura si el enemigo está herido
        if (life < 1f && !isTakingCover)
        {
            TakeCover();
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Asegurar que la dirección sea horizontal en el plano X-Z

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= timeBtwShoot)
        {
            shootTimer = 0f;
            StartCoroutine(ShootMultipleBullets());
        }
    }

    IEnumerator ShootMultipleBullets()
    {
        for (int i = 0; i < 3; i++)
        {
            if (firePoint != null && bulletPrefab != null)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            }
            yield return new WaitForSeconds(shootInterval);
        }
    }

    public void TakeDamage(float damage)
    {
        life -= damage;
        if (life <= 0)
        {
            Die();
        }
    }

    void TakeCover()
    {
        isTakingCover = true;
        Vector3 coverPosition = FindClosestCover();
        if (coverPosition != Vector3.zero)
        {
            MoveTo(coverPosition);
        }
    }

    Vector3 FindClosestCover()
    {
        // Lógica para encontrar la cobertura más cercana (placeholder)
        return Vector3.zero;
    }

    void MoveTo(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, rotationSpeed * Time.deltaTime);
    }

    void Die()
    {
        if (room != null)
        {
            room.UpdateEnemyCount(true);
            Debug.Log("Enemy died, updating room's enemy count.");
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(1f);
            }
            Die();
        }
    }
}
