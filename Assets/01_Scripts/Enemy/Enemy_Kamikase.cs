using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Kamikase : MonoBehaviour
{
    public float normalSpeed = 100f;
    public float chaseSpeed = 65f;
    public float detectionRange = 40f;
    public float explosionDamage = 3f;
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    public int health = 5;

    private Transform player;
    private bool isChasing = false;
    private float currentSpeed;
    private Room room;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            currentSpeed = chaseSpeed;

            if (distanceToPlayer < 10f)
            {
                currentSpeed = chaseSpeed * 1.5f; // Aumentar velocidad al acercarse
            }

            ChasePlayer();
        }
        else
        {
            isChasing = false;
            currentSpeed = normalSpeed;
            Patrol();
        }
    }

    void ChasePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationToPlayer, Time.deltaTime * 5f);
        transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);
    }

    void Patrol()
    {
        transform.position += new Vector3(0, 0, currentSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(explosionDamage);
            }
            Explode();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BulletPlayer"))
        {
            health--;
            if (health <= 0)
            {
                Explode();
            }
            Destroy(other.gameObject);
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        if (room != null)
        {
            room.UpdateEnemyCount(true);
        }

        Destroy(gameObject);
    }
}
