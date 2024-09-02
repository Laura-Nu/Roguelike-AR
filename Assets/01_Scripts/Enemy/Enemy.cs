using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5f;  // Velocidad base
    public float life = 2f;
    public float damage = 1f;
    public float timeBtwShoot = 2f; // Aumentado para disparar más lentamente
    private float timer = 0f;
    private bool canShoot = false;
    public Transform firePoint;
    public GameObject bulletPrefab;

    void Start()
    {
        speed = Random.Range(3f, 6f);
        if (Random.Range(0, 2) == 0)
        {
            canShoot = true;
        }
    }

    void Update()
    {
        Shoot();
        //transform.Translate(-Vector3.forward * speed * Time.deltaTime);
    }

    void Shoot()
    {
        if (canShoot)
        {
            timer += Time.deltaTime;
            if (timer >= timeBtwShoot)
            {
                timer = 0f;
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Enemigo recibió daño: " + damage);
        life -= damage;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Knife"))
        {
            TakeDamage(1f); //Cuando el enemigo se choca con el cuchillo, el enemigo recibe daño
        }
    }
}
