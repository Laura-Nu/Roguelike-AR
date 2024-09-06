using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public int coinsCount = 200;
    public float life = 50f;
    public float maxLife = 50f;
    public float speed = 5f;
    public float baseAttackDamage = 1f;
    public float currentAttackDamage = 1f;
    public float poweredAttackDamage = 5f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float stompLifeCost = 10f;
    public float stompForce = 20f;
    public float stompRadius = 5f;
    public float mainAttackDuration = 0.5f;
    public Vector3 mainAttackOffset = new Vector3(0, 0, 1f); // Desplazamiento hacia adelante
    public float knifeAttackDistance = 2f;
    public float knifeAttackDuration = 0.5f;
    public Vector3 knifeAttackDirection = Vector3.forward;
    Vector3 initialLocalPosition;
    bool isShopOpen = false;
    bool isInventoryOpen = false;
    bool isDashing = false;
    float dashTimeLeft = 0f;

    [Header("Referencias")]
    public Rigidbody rb;
    public Transform body;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public Transform knife;
    public CapsuleCollider knifeCollider;
    public Image inventory;
    public Image shopButton;
    public Image shop;

    [Header("Items")]
    public bool isAttackUpActive = false;
    public float attackTimeLeft = 0f;
    public bool isInvincibilityActive = false;
    public float invincibilityTimeLeft = 0f;
    public bool isOneTimeShieldActive = false;
    public bool isLastWillActive = false;
    public float lastWillChance = 0.2f;

    [Header("UI")]
    public TextMeshProUGUI CoinsText;
    public Image lifeBar;

    // Start is called before the first frame update
    void Start()
    {
        CoinsText.text = "" + coinsCount;
        lifeBar.fillAmount = life / maxLife;
        initialLocalPosition = knife.localPosition;
        knifeCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Dash();
        Invincibility();
        AttackUp();
    }

    void Invincibility()
    {
        if (isInvincibilityActive)
        {
            if (invincibilityTimeLeft > 0f)
            {
                invincibilityTimeLeft -= Time.deltaTime;
            }
            else//Desactivar
            {
                invincibilityTimeLeft = 0f;
                isInvincibilityActive = false;
            }
        }
    }

    void AttackUp()
    {
        if (isAttackUpActive)
        {
            if (attackTimeLeft > 0f)
            {
                attackTimeLeft -= Time.deltaTime;
                currentAttackDamage = poweredAttackDamage;
            }
            else//Desactivar
            {
                attackTimeLeft = 0f;
                isAttackUpActive = false;
                currentAttackDamage = baseAttackDamage;
            }
        }
    }

    void Movement()
    {
        float x = SimpleInput.GetAxis("Horizontal");
        float z = SimpleInput.GetAxis("Vertical");

        rb.velocity = new Vector3(x * speed, 0, z * speed);

        if (x != 0 || z != 0)
        {
            body.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    public void MainAttack()
    {
        StartCoroutine(KnifeAttack());
    }

    IEnumerator KnifeAttack()
    {
        // Fijamos la posici?n inicial en cada ataque
        knife.localPosition = initialLocalPosition;

        float elapsedTime = 0f;
        Vector3 startPosition = knife.localPosition;
        Vector3 endPosition = startPosition + knifeAttackDirection.normalized * knifeAttackDistance;

        // Fase de avance
        while (elapsedTime < knifeAttackDuration / 2)
        {
            knife.localPosition = Vector3.Lerp(startPosition, endPosition, (elapsedTime / (knifeAttackDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        knife.localPosition = endPosition;
        knifeCollider.enabled = true;

        // Espera un peque?o intervalo de tiempo antes de volver
        yield return new WaitForSeconds(0.1f);

        elapsedTime = 0f;

        // Fase de regreso
        while (elapsedTime < knifeAttackDuration / 2)
        {
            knife.localPosition = Vector3.Lerp(endPosition, startPosition, (elapsedTime / (knifeAttackDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        knife.localPosition = startPosition;
        knifeCollider.enabled = false;
    }

    public void ShootAttack()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public void StompAttack()
    {
        if (life > stompLifeCost)
        {
            life -= stompLifeCost;
            lifeBar.fillAmount = life / maxLife;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, stompRadius);

            foreach (Collider hitCollider in hitColliders)
            {
                // Verificar si el collider pertenece a un enemigo
                if (hitCollider.CompareTag("Enemy"))
                {
                    // Aplicar empuje al enemigo
                    Rigidbody enemyRigidbody = hitCollider.GetComponent<Rigidbody>();
                    if (enemyRigidbody != null)
                    {
                        Vector3 pushDirection = (hitCollider.transform.position - transform.position).normalized;
                        enemyRigidbody.AddForce(pushDirection * stompForce, ForceMode.Impulse);
                    }

                    // Hacer da?o al enemigo
                    Enemy enemy = hitCollider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(currentAttackDamage);
                    }
                }
            }
        }
    }

    // Visualizaci?n del radio de ataque en la escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stompRadius);
    }

    public void TriggerDash()
    {
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
            Vector3 dashPosition = transform.position + bodyForward * dashSpeed * Time.deltaTime;

            // Verificar colisiones usando Raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position, bodyForward, out hit, dashSpeed * Time.deltaTime))
            {
                if (hit.collider.CompareTag("Room"))
                {
                    isDashing = false; // Detener el dash si hay una colision
                    return;
                }
            }

            rb.MovePosition(dashPosition);
            if (dashTimeLeft <= 0)
                isDashing = false;
        }
    }

    public void OpenCloseInventory()
    {
        if (isInventoryOpen)//cerrar
        {
            inventory.gameObject.SetActive(false);
            isInventoryOpen = false;
        }
        else if (!isShopOpen)//abrir
        {
            inventory.gameObject.SetActive(true);
            isInventoryOpen = true;
        }
    }

    public void OpenCloseShop()
    {
        if (isShopOpen)//cerrar
        {
            shop.gameObject.SetActive(false);
            isShopOpen = false;
        }
        else if (!isInventoryOpen)//abrir
        {
            ShopManager.Instance.ResetShopText();
            shop.gameObject.SetActive(true);
            isShopOpen = true;
        }
    }

    public void AddCoins(int amount)
    {
        coinsCount += amount;
        CoinsText.text = coinsCount.ToString();
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincibilityActive)
        {
            if (isOneTimeShieldActive)
            {
                isOneTimeShieldActive = false;
            }
            else
            {
                life -= damage;
                lifeBar.fillAmount = life / maxLife;
                if (life <= 0)
                {
                    if (isLastWillActive)
                    {
                        if (Random.value <= lastWillChance)
                        {
                            Debug.Log("Last Will Activated. Gods blessed you with another chance");
                            life = 10f;
                            lifeBar.fillAmount = life / maxLife;
                        }
                        else
                        {
                            Debug.Log("Better luck next time");
                            isLastWillActive = false;
                            SceneManager.LoadScene("Game");
                        }
                    }
                    else
                    {
                        SceneManager.LoadScene("Game");  // Reinicia la escena cuando la vida llega a 0
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Lobby"))
        {
            shopButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Lobby"))
        {
            shopButton.gameObject.SetActive(false);
        }
    }
}
