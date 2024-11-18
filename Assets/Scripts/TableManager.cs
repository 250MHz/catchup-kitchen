using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance { get; private set; }

    [SerializeField] private bool isTesting;
    [SerializeField] private GlassTable[] tables;
    [SerializeField] private UsableObjectSO tableShopItemSO;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitiateTables();
    }

    public bool HasInactiveTable()
    {
        for (int i = 0; i < tables.Length; i++)
        {
            if (!tables[i].gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public int GetNextTablePrice()
    {
        // If multiple players have menu UI open after all tables are bought,
        // the table option still shows up, so make sure it doesn't cost
        // any money if they try to buy it.
        int price = 0;
        for (int i = 0; i < tables.Length; i++)
        {
            if (!tables[i].gameObject.activeSelf)
            {
                price = i * tableShopItemSO.GetPrice();
                break;
            }
        }
        return price;
    }

    private void InitiateTables()
    {
        if (isTesting)
        {
            return;
        }
        for (int i = 0; i < tables.Length; i++)
        {
            // Only have first table active
            tables[i].gameObject.SetActive(i == 0);
        }
    }

    public void ActivateNextTable()
    {
        for (int i = 1; i < tables.Length; i++)
        {
            if (!tables[i].gameObject.activeSelf)
            {
                tables[i].gameObject.SetActive(true);
                return;
            }
        }
    }
}
