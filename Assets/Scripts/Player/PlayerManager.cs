using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private GameObject player3Prefab;

    // Start is called before the first frame update
    void Awake()
    {
        var player1 = PlayerInput.Instantiate(
            player1Prefab, playerIndex: 0, controlScheme: "WASD",
            pairWithDevice: Keyboard.current
        );
        var player2 = PlayerInput.Instantiate(
            player2Prefab, playerIndex: 1, controlScheme: "IJKL",
            pairWithDevice: Keyboard.current
        );
        var player3 = PlayerInput.Instantiate(
            player3Prefab, playerIndex: 2, controlScheme: "Arrows",
            pairWithDevice: Keyboard.current
        );
        player1.transform.parent.transform.position = new Vector3(0, 0, -3);
        player2.transform.parent.transform.position = new Vector3(-3, 0, -1.5f);
        player3.transform.parent.transform.position = new Vector3(-3, 0, -3);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
