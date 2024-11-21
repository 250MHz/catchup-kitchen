using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonManager : MonoBehaviour
{
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private Transform endLocation;
    [SerializeField] private GameObject[] personPrefabs;
    [SerializeField] private float spawnTime;

    private float elapsedTime;
    private float lastSpawnTime;
    private GameObject person;
    private Vector3 movementVec;
    private float multiplier;

    private void Start()
    {
        elapsedTime = 0f;
        lastSpawnTime = 0f;
        person = null;
        multiplier = 1;
        // Assuming straight line between spawn location and end location
        if (spawnLocation.position.x != endLocation.position.x)
        {
            Vector3 movementVec = new Vector3(1, 0, 0);
            if (spawnLocation.position.x < endLocation.position.x)
            {
                // Increase to reach end
                multiplier = 1;
            }
            else
            {
                multiplier = -1;
            }
        }
        else if (spawnLocation.position.z != endLocation.position.z)
        {
            Vector3 movementVec = new Vector3(0, 0, 1);
            if (spawnLocation.position.z < endLocation.position.z)
            {
                // Increase to reach end
                multiplier = 1;
            }
            else
            {
                multiplier = -1;
            }
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime - lastSpawnTime >= spawnTime)
        {
            person = Instantiate(personPrefabs[Random.Range(0, personPrefabs.Length - 1)], spawnLocation);
            Animator animator = person.GetComponent<Animator>();
            animator.SetBool("Static_b", false);
            animator.SetFloat("Speed_f", 0.3f);
            lastSpawnTime = elapsedTime;
        }
        if (person != null)
        {
            if (movementVec.x > 0) {
                if (person.transform.position.x < multiplier * endLocation.position.z)
                {
                    person.transform.Translate(Vector3.forward * Time.deltaTime * 7);
                }
                else
                {
                    Destroy(person);
                    person = null;
                }
            } else if (movementVec.z > 0) {
                if (person.transform.position.z < multiplier * endLocation.position.z)
                {
                    person.transform.Translate(Vector3.forward * Time.deltaTime * 7);
                }
                else
                {
                    Destroy(person);
                    person = null;
                }
            }
        }
    }
}
