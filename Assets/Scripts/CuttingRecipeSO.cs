using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject
{
    [SerializeField] private UsableObjectSO input;
    [SerializeField] private UsableObjectSO output;

    public UsableObjectSO GetInput()
    {
        return input;
    }

    public UsableObjectSO GetOutput()
    {
        return output;
    }
}
