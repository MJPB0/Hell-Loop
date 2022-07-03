using Assets.Scripts.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPouch : InteractableObject
{
    private SpriteRenderer spriteRenderer;
    private Player player;

    [SerializeField] private PickableScriptableObject pouchObject;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        spriteRenderer.sprite = pouchObject.sprite;

        player = FindObjectOfType<Player>();
    }

    public override void Interact()
    {
        player.AddGold(pouchObject.Value);
        Destroy(gameObject);
    }
}
