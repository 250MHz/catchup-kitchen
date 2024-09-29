using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    PlayerInputManager manager;
    // Start is called before the first frame update
    void Awake()
    {
        // manager = GetComponent<PlayerInputManager>();
        // manager.playerPrefab = playerPrefab;
        var player1 = PlayerInput.Instantiate(playerPrefab, playerIndex: 0, controlScheme: "WASD", pairWithDevice: Keyboard.current);
        var player2 = PlayerInput.Instantiate(playerPrefab, playerIndex: 1, controlScheme: "IJKL", pairWithDevice: Keyboard.current);
        var player3 = PlayerInput.Instantiate(playerPrefab, playerIndex: 2, controlScheme: "Arrows", pairWithDevice: Keyboard.current);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
