using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Vector3 trackingPosition;
    private Vector3 offset;
    private bool followingPlayer;

    // Start is called before the first frame update
    private void Start()
    {
        offset = transform.position - player.transform.position;
        followingPlayer = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (followingPlayer)
        {
            transform.position = player.transform.position + offset;
        }
    }

    public void FollowPlayer()
    {
        followingPlayer = true;
    }

    // Modified from https://discussions.unity.com/t/moving-the-camera-smoothly/660515/12
    public IEnumerator MoveCameraToPosition(
        Vector3 _locationToMoveTo,
        float _timeToMove,
        bool newFollowingPlayer = false)
    {
        followingPlayer = false;
        Vector3 currentPos = transform.position;
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / _timeToMove;
            transform.position = Vector3.Lerp(currentPos, _locationToMoveTo, t);
            if (t > 0.8f)
            {
                trackingPosition = _locationToMoveTo;
            }
            yield return null;
        }
        followingPlayer = newFollowingPlayer;
    }
}
