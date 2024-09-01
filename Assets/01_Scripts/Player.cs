using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 5f;
    public float life = 50f;
    [Header("Referencias")]
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        float x = SimpleInput.GetAxis("Horizontal");
        float z = SimpleInput.GetAxis("Vertical");

        rb.velocity = new Vector3(x * speed, 0, z * speed);
    }

    public void TakeDamage(float damage)
    {
        life -= damage;
        if (life <= 0)
        {
            SceneManager.LoadScene("Game");  // Reinicia la escena cuando la vida llega a 0
        }
    }
}
