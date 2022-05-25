using Assets.Scripts.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceBook : InteractableObject
{
    private SpriteRenderer spriteRenderer;
    private Player player;

    [SerializeField] private PickableScriptableObject bookObject;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        gameObject.transform.localScale = new Vector3(.8f, .8f, .8f);
        spriteRenderer.sprite = bookObject.sprite;

        player = FindObjectOfType<Player>();
    }

    public override void Interact()
    {
        //Debug.Log($"{gameObject.name} was picked up");
        player.AddExperience(bookObject.Value);
        Destroy(gameObject);
    }
}
