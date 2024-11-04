using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class UsableObjectSO : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private string objectName;
    [SerializeField] private string description;
    [SerializeField] private int price;
    [SerializeField] private Sprite icon;

    public GameObject GetPrefab()
    {
        return prefab;
    }

    public string GetObjectName()
    {
        return objectName;
    }

    public string GetDescription()
    {
        return description;
    }

    public int GetPrice()
    {
        return price;
    }

    public Sprite GetIcon()
    {
        return icon;
    }
}
