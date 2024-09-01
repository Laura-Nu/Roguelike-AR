using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sniper : MonoBehaviour
{
    public float detectionRange = 40f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.3f;
    public AudioClip shootingSound;
    public int maxHealth = 4;
    public float moveSpeed = 5f; // Ajustado para velocidad de movimiento más realista
    public float rotationSpeed = 5f;
    public float collisionDamage = 3f;

    private Transform player;
    private bool isShooting = false;
    private float nextFireTime = 0f;
    private int currentHealth;
    private Quaternion originalRotation;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isShooting = true;
            RotateTowardsPlayer();
            MoveTowardsPlayer();
        }
        else
        {
            isShooting = false;
            ResetRotation();
            MoveInZ(); // Continuar moviéndose en Z si no está persiguiendo al jugador
        }

        if (isShooting && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().playerBullet = false;

        if (shootingSound != null)
        {
            AudioSource.PlayClipAtPoint(shootingSound, transform.position);
        }
    }

    void MoveInZ()
    {
        // Movimiento en el eje Z cuando no se está persiguiendo al jugador
        transform.position += new Vector3(0, 0, -moveSpeed * Time.deltaTime);
    }

    void MoveTowardsPlayer()
    {
        // Moverse hacia el jugador si está en rango
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Mantener el movimiento en el plano horizontal
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void RotateTowardsPlayer()
    {
        // Rotar hacia el jugador
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Mantener la rotación en el plano horizontal
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    void ResetRotation()
    {
        // Restablecer la rotación original cuando el jugador está fuera de rango
        transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, rotationSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(collisionDamage);
                Die();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BulletPlayer"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage((int)bullet.damage);
                Destroy(other.gameObject);
            }
        }
    }
}
