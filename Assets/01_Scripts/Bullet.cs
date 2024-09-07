using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;  // Velocidad de la bala
    public float timeToDestroy = 100f;  // Tiempo antes de destruir la bala
    public float damage = 1f;  // Daño que inflige la bala
    public bool playerBullet = false;  // Diferenciar entre bala de jugador y de enemigo

    private Rigidbody rb;

    void Start()
    {
        // Destruir la bala después de un tiempo
        Destroy(gameObject, timeToDestroy);

        // Si hay un Rigidbody, desactivar la gravedad
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Desactivar la gravedad
        }
    }

    void Update()
    {
        // Movimiento hacia adelante en el eje Z
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collision)
    {
        // Manejar colisiones con el jugador o con objetos con tag "Lobby"
        if (!playerBullet && (collision.CompareTag("Player") || collision.CompareTag("Lobby")))
        {
            // Si colisiona con el jugador, infligir daño
            if (collision.CompareTag("Player"))
            {
                Debug.Log("Bala del enemigo colisionó con el jugador");
                Player p = collision.GetComponent<Player>();
                if (p != null)
                {
                    p.TakeDamage(damage);  // Infligir daño al jugador
                }
            }
            // Si colisiona con un objeto con tag "Lobby"
            else if (collision.CompareTag("Lobby"))
            {
                Debug.Log("Bala colisionó con el Lobby");
            }

            // Destruir la bala después de colisionar
            Destroy(gameObject);
        }
    }
}
