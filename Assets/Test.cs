using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.GetComponent<Renderer>().bounds.center);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
