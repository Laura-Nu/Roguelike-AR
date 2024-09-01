using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 5f;
    public float life = 50f;
    public float maxLife = 50f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float stompCost = 10f;
    public int coinsCount = 0;
    float dashTimeLeft = 0f;
    bool isDashing = false;

    [Header("Referencias")]
    public Rigidbody rb;
    public Transform body;
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("UI")]
    public TextMeshProUGUI CoinsText;
    public Image lifeBar;

    // Start is called before the first frame update
    void Start()
    {
        CoinsText.text = ""+coinsCount;
        lifeBar.fillAmount = life / maxLife;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Dash();
    }

    void Movement()
    {
        float x = SimpleInput.GetAxis("Horizontal");
        float z = SimpleInput.GetAxis("Vertical");

        rb.velocity = new Vector3(x * speed, 0, z * speed);

        if(x != 0 || z != 0)
        {
            body.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    public void MainAttack()
    {
        Debug.Log("Fire 1 - Main Attack executed");
    }

    public void ShootAttack()
    {
        Debug.Log("fire 2 - Shoot executed");
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public void StompAttack()
    {
        Debug.Log("Fire 3 - Stomp executed");
        if (life > stompCost)
        {
            life -= stompCost;
            lifeBar.fillAmount = life / maxLife;
            //Codigo stomp
        }
    }


    public void TriggerDash()
    {
        Debug.Log("Dash executed");
        isDashing = true;
        dashTimeLeft = dashDuration;
    }

    void Dash()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            Vector3 bodyForward = body.forward;
            bodyForward.y = 0;
            bodyForward.Normalize();
            transform.Translate(bodyForward * dashSpeed * Time.deltaTime, Space.World);
            if (dashTimeLeft <= 0)
                isDashing = false;
        }
    }

    public void TakeDamage(float damage)
    {
        life -= damage;
        lifeBar.fillAmount = life / maxLife;
        if (life <= 0)
        {
            SceneManager.LoadScene("Game");  // Reinicia la escena cuando la vida llega a 0
        }
    }
}
