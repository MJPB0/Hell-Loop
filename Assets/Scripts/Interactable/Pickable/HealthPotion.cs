﻿using Assets.Scripts.Interactable;
using System.Collections;
using UnityEngine;

public class HealthPotion : InteractableObject
{
    private SpriteRenderer spriteRenderer;
    private Player player;

    [SerializeField] private PickableScriptableObject potionObject;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        spriteRenderer.sprite = potionObject.sprite;

        player = FindObjectOfType<Player>();
    }

    public override void Interact()
    {
        player.Heal(potionObject.Value);
        Destroy(gameObject);
    }
}