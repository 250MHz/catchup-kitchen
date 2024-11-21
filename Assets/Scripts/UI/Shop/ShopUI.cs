using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    // TODO: I don't think these need [SerializeField]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    [SerializeField] private UsableObjectSO tableSO;
    [SerializeField] private UsableObjectSO plateSO;
    [SerializeField] private UsableObjectSO potSO;
    [SerializeField] private UsableObjectSO panSO;


    private List<UsableObjectSO> availableItems;
    private int selectedItem;

    private List<ItemSlotUI> slotUIList;

    private const int itemsInViewport = 6;
    private RectTransform itemListRectTransform;

    private void Awake()
    {
        itemListRectTransform = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<UsableObjectSO> availableItems)
    {
        this.availableItems = availableItems;
        gameObject.SetActive(true);
        RemoveItemsIfPossible();
    }

    public void OnMove(Vector2 moveAmount)
    {
        // Debug.Log($"selectedItem: {selectedItem}");
        // Debug.Log($"moveAmount.y: {moveAmount.y}");
        if (moveAmount.y < 0)
        {
            // Down
            selectedItem = (selectedItem + 1) % availableItems.Count;
        }
        else if (moveAmount.y > 0)
        {
            // Up
            if (selectedItem == 0)
            {
                selectedItem = availableItems.Count - 1;
            }
            else
            {
                selectedItem--;
            }
        }
        UpdateItemSelection();
    }

    public void Close()
    {
        selectedItem = 0;
        gameObject.SetActive(false);
    }

    public UsableObjectSO GetSelectedItem()
    {
        return availableItems[selectedItem];
    }

    public void RemoveItemsIfPossible()
    {
        List<UsableObjectSO> newAvailableItems = new List<UsableObjectSO>();
        foreach (UsableObjectSO item in availableItems)
        {
            if ((item == tableSO && !TableManager.Instance.HasInactiveTable())
                || (item == plateSO && PlateManager.Instance.MaxPlateCountReached())
                || (item == potSO && PotPanManager.Instance.MaxPotCountReached())
                || (item == panSO && PotPanManager.Instance.MaxPanCountReached()))
            {
                continue;
            }
            newAvailableItems.Add(item);
        }
        availableItems = newAvailableItems;
        UpdateItemList();
    }

    public void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (UsableObjectSO item in availableItems)
        {
            ItemSlotUI slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetNameAndPrice(item);
            slotUIList.Add(slotUIObj);
        }
        UpdateItemSelection();
    }

    private void UpdateItemSelection()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            slotUIList[i].SetSelected(i == selectedItem);
        }

        if (availableItems.Count > 0)
        {
            UsableObjectSO item = availableItems[selectedItem];
            itemIcon.sprite = item.GetIcon();
            itemDescription.text = item.GetDescription();
        }

        HandleScrolling();
    }

    private void HandleScrolling()
    {
        // TODO: this doesn't seem like a great solution when the UI list
        // is dynamically sized
        if (slotUIList.Count <= itemsInViewport)
        {
            upArrow.gameObject.SetActive(false);
            downArrow.gameObject.SetActive(false);
            return;
        }

        float scrollPos = Mathf.Clamp(
                selectedItem - itemsInViewport / 2, 0, selectedItem
            ) * slotUIList[0].Height;
        itemListRectTransform.localPosition = new Vector2(
            itemListRectTransform.localPosition.x,
            scrollPos
        );

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        Debug.Log(availableItems.Count);
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem * itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
}
