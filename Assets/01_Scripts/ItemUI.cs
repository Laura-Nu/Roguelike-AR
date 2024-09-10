using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    [Header("Referencias")]
    public ItemSO item;
    public Image itemImage;
    public TextMeshProUGUI itemCountText;
    public Image itemCoinImg;
    public TextMeshProUGUI itemCostText;
    GameObject playerGO;
    [Header("Stats")]
    public int itemCount = 0;
    public bool isInInventory = false;

    void Awake()
    {
        playerGO = GameObject.FindWithTag("Player");
    }

    public void Init(ItemSO item)
    {
        this.item = item;
        itemImage.sprite = item.sprite;
        itemImage.gameObject.SetActive(true);
        if (!isInInventory)//Tienda
        {
            itemCoinImg.gameObject.SetActive(true);
            itemCostText.gameObject.SetActive(true);
            itemCostText.text = "x" + item.coinCost.ToString();
        }
        else//Inventario
        {
            itemCoinImg.gameObject.SetActive(false);
            itemCostText.gameObject.SetActive(false);
            itemCountText.gameObject.SetActive(true);
            itemCountText.text = itemCount.ToString();
        }
    }

    public void UpdateInfo()
    {
        //itemImage.sprite = item.sprite;
        //itemImage.gameObject.SetActive(true);
        //itemCountText.gameObject.SetActive(true);
        itemCountText.text = itemCount.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null)
        {
            if (isInInventory)
            {
                if (itemCount > 0)
                {
                    itemCount--;
                    ApplyItemEffect();
                    if (itemCount == 0)
                    {
                        itemImage.gameObject.SetActive(false);
                        itemCountText.gameObject.SetActive(false);
                        item = null;
                    }
                    UpdateInfo();
                }
            }
            else // Tienda
            {
                // Seleccionar el item en la tienda y habilitar el boton de comprar
                ShopManager.Instance.SelectItem(item);
            }
        }
    }

    void ApplyItemEffect()
    {
        Player p = playerGO.GetComponent<Player>();
        switch (item.itemName)
        {
            case "Last Will":
                p.isLastWillActive = true;
                p.lastWillImgEffect.gameObject.SetActive(true);
                break;
            case "Life Potion":
                p.life = p.life + 10f > p.maxLife ? p.maxLife : p.life + 10f;
                p.lifeBar.fillAmount = p.life / p.maxLife;
                break;
            case "One-Time Shield":
                p.isOneTimeShieldActive = true;
                p.shieldImgEffect.gameObject.SetActive(true);
                break;
            case "Temporal Invincibility":
                p.invincibilityTimeLeft += 10f;
                p.isInvincibilityActive = true;
                p.invincibilityImgEffect.gameObject.SetActive(true);
                break;
            case "Attack Up":
                p.attackTimeLeft += 10f;
                p.isAttackUpActive = true;
                p.atkUpImgEffect.gameObject.SetActive(true);
                break;
        }
    }

    public void SetUIVisibility(bool isVisible)
    {
        itemImage.gameObject.SetActive(isVisible);
        if (isVisible)
        {
            // Actualizar la información si es visible
            UpdateInfo();
        }
        else
        {
            // Desactivar el resto de los componentes
            itemCoinImg.gameObject.SetActive(false);
            itemCountText.gameObject.SetActive(false);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
