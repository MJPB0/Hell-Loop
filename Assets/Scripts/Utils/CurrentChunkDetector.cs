using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentChunkDetector : MonoBehaviour
{
    private const string CHUNK_TAG = "Map Chunk";
    private MapGenerator mapGenerator;

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CHUNK_TAG))
        {
            mapGenerator.ChangeCurrentChunk(collision.gameObject.name);
        }
    }
}
