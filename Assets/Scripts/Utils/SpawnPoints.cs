using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField] private Transform[] nodes;
    public Transform[] Nodes { get { return nodes; } }

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        transform.position = new(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);
    }
}
