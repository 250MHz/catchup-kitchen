using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private Vector3 player1SpawnVector;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private Vector3 player2SpawnVector;
    [SerializeField] private GameObject player3Prefab;
    [SerializeField] private Vector3 player3SpawnVector;

    private void Awake()
    {
        Player player1 = PlayerInput.Instantiate(
            player1Prefab, playerIndex: 0, controlScheme: "WASD",
            pairWithDevice: Keyboard.current
        ).GetComponent<Player>();
        player1.SetControlScheme("WASD");
        Player player2 = PlayerInput.Instantiate(
            player2Prefab, playerIndex: 1, controlScheme: "IJKL",
            pairWithDevice: Keyboard.current
        ).GetComponent<Player>();
        player2.SetControlScheme("IJKL");
        Player player3 = PlayerInput.Instantiate(
            player3Prefab, playerIndex: 2, controlScheme: "Arrows",
            pairWithDevice: Keyboard.current
        ).GetComponent<Player>();
        player3.SetControlScheme("Arrows");
        player1.transform.parent.transform.position = player1SpawnVector;
        player2.transform.parent.transform.position = player2SpawnVector;
        player3.transform.parent.transform.position = player3SpawnVector;
    }
}
