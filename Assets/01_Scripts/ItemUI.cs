using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    public ItemSO item;
    public Image itemImage;
    public TextMeshProUGUI itemCountText;
    public int itemCount = 0;

    public void Init(ItemSO item)
    {
        this.item = item;
        itemImage.sprite = item.sprite;
        itemImage.gameObject.SetActive(true);
        itemCountText.gameObject.SetActive(true);
        itemCountText.text = itemCount.ToString();
    }

    public void UpdateInfo()
    {
        itemImage.sprite = item.sprite;
        itemImage.gameObject.SetActive(true);
        itemCountText.gameObject.SetActive(true);
        itemCountText.text = itemCount.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
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

    void ApplyItemEffect()
    {
        Player p = GetComponent<Player>();
        switch (item.itemName)
        {

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
