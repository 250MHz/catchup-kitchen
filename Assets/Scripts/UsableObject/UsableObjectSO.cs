using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class UsableObjectSO : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private string objectName;

    public GameObject GetPrefab() {
        return prefab;
    }

    public string GetObjectName() {
        return objectName;
    }
}
