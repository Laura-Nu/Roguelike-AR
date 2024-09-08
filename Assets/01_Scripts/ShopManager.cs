using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public List<ItemSO> items = new List<ItemSO>();
    public List<ItemUI> itemsUI = new List<ItemUI>();
    public TextMeshProUGUI descriptionText;
    public Button buyButton;
    public Player player;
    public Image dashButton;
    public Image stompButton;
    public Image shootButton;
    public GameObject GunPrefab;
    public int maxUpgrades = 3;
    public int currentLifeUpgrade = 0;
    public int currentSpeedUpgrade = 0;
    ItemSO selectedItem;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        descriptionText.text = "Greetings.\nWhat do you need?";
        for (int i = 0; i < items.Count; i++)
        {
            itemsUI[i].Init(items[i]);
        }
    }

    public void BuyItem()
    {
        if (selectedItem != null)
        {
            // Resta las monedas del jugador
            player.coinsCount -= selectedItem.coinCost;
            player.CoinsText.text = player.coinsCount.ToString();
            switch (selectedItem.itemName)
            {
                //Se pueden comprar una sola vez
                case "Stomp":
                    stompButton.gameObject.SetActive(true);
                    MakeItemSoldOut("Stomp");
                    break;
                case "Gun":
                    shootButton.gameObject.SetActive(true);
                    GunPrefab.SetActive(true);
                    MakeItemSoldOut("Gun");
                    break;
                case "Dash/Dodge":
                    dashButton.gameObject.SetActive(true);
                    MakeItemSoldOut("Dash/Dodge");
                    break;
                case "Charge Attack":
                    break;

                //Se pueden comprar limitadamente
                case "Increase MAX Life":
                    if (currentLifeUpgrade < maxUpgrades)
                    {
                        player.maxLife += 20f;
                        player.lifeBar.fillAmount = player.life / player.maxLife;
                        currentLifeUpgrade++;
                        if (currentLifeUpgrade == maxUpgrades)
                        {
                            MakeItemSoldOut("Increase MAX Life");
                        }
                    }
                    break;
                case "Increase Speed":
                    if (currentSpeedUpgrade < maxUpgrades)
                    {
                        player.speed += 2f;
                        currentSpeedUpgrade++;
                        if (currentSpeedUpgrade == maxUpgrades)
                        {
                            MakeItemSoldOut("Increase Speed");
                        }
                    }
                    break;

                //Se pueden comprar sin limite
                case "Last Will":
                    // Anadir el item al inventario
                    InventoryManager.Instance.AddItem(selectedItem);
                    break;
                case "Life Potion":
                    InventoryManager.Instance.AddItem(selectedItem);
                    break;
                case "One-Time Shield":
                    InventoryManager.Instance.AddItem(selectedItem);
                    break;
                case "Temporal Invincibility":
                    InventoryManager.Instance.AddItem(selectedItem);
                    break;
                case "Attack Up":
                    InventoryManager.Instance.AddItem(selectedItem);
                    break;
            }

            // Actualizar UI o feedback
            descriptionText.text = $"Thanks for buying [{selectedItem.itemName}].";
            buyButton.gameObject.SetActive(false);
            selectedItem = null;
        }
    }

    // Llamado cuando un item es seleccionado en la tienda
    public void SelectItem(ItemSO item)
    {
        selectedItem = item;
        descriptionText.text = "[" + item.itemName + "]\n" + item.description;
        if (player.coinsCount >= item.coinCost)
        {
            buyButton.gameObject.SetActive(true);
        }
        else
        {
            buyButton.gameObject.SetActive(false);
        }
    }

    public void ResetShopText()
    {
        descriptionText.text = "Greetings.\nWhat do you need?";
        selectedItem = null; // Resetear el item seleccionado
        buyButton.gameObject.SetActive(false);
    }

    void MakeItemSoldOut(string name)
    {
        foreach (var item in itemsUI)
        {
            if (item.item.itemName == name)
            {
                item.gameObject.SetActive(false); // Desactiva el GameObject del item
                break;
            }
        }
    }
}
