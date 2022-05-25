using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = FindObjectOfType<Player>().transform;
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -15f);
    }
}
