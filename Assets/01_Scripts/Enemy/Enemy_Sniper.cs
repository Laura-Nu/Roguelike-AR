using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Sniper : MonoBehaviour
{
    public float life = 2f;
    public float detectionRange = 20f;
    public float rotationSpeed = 5f;
    public float moveSpeed = 3f;
    public float timeBtwShoot = 2.3f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float shootInterval = 0.1f;
    public GameObject coinPrefab;

    private Transform player;
    private float shootTimer = 0f;
    private Room room;  // Referencia a la habitación

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        room = FindObjectOfType<Room>();  // Encuentra la habitación más cercana
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            RotateTowardsPlayer();
            MoveTowardsPlayer();
            Shoot();
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        transform.position += directionToPlayer.normalized * moveSpeed * Time.deltaTime;
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
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Die();
        }
    }

    void Die()
    {
        if (room != null)
        {
            room.UpdateEnemyCount(true);  // Actualiza el contador de enemigos
            Debug.Log("Enemy died, enemies in room: " + room.enemiesInRoom);
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

            if (coinPrefab != null)
            {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }

            Die();
        }
    }
}
