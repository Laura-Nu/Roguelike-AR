using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float timeToDestroy = 4f;
    public float damage = 1f;
    public bool playerBullet = false;  // Diferenciar entre bala de jugador y de enemigo

    void Start()
    {
        Destroy(gameObject, timeToDestroy);  // Destruir la bala después de un tiempo
    }

    // Update is called once per frame
    void Update()
    {
        // Movimiento hacia adelante en el espacio 3D
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Colisión detectada con: " + collision.gameObject.name);
        if (playerBullet && collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Bala del jugador colisionó con el enemigo");
            Enemy e = collision.gameObject.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!playerBullet && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bala del enemigo colisionó con el jugador");
            Player p = collision.gameObject.GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Fin"))
        {
            Debug.Log("Bala colisionó con 'Fin'");
            Destroy(gameObject);
        }
    }

}
