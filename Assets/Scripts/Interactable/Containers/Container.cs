using Assets.Scripts.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Container : InteractableObject
{
    protected const string CONTAINER_OPEN = "Open";
    protected const string CONTAINER_OPENED = "Opened";

    protected const string INTERACTABLES_TAG = "Interactables Parent";
    protected const string PICKABLES_TAG = "Pickables Parent";

    protected SpriteRenderer spriteRenderer;
    protected CircleCollider2D collider;
    protected Animator anim;

    protected AnimatorOverrider overrider;

    [SerializeField] protected bool willBeDestroyed = true;

    [Space]
    [SerializeField] protected ContainerScriptableObject containerObject;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        overrider = GetComponentInChildren<AnimatorOverrider>();
        anim = GetComponentInChildren<Animator>();
        collider = GetComponent<CircleCollider2D>();
    }

    protected void Start()
    {
        if (containerObject.overrideControllers.Length > 0)
            SetController(Random.Range(0, containerObject.overrideControllers.Length));
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        anim.SetTrigger(CONTAINER_OPEN);
    }

    public void SetController(int index)
    {
        overrider.SetAnimations(containerObject.overrideControllers[index]);
    }

    public abstract void ContainerOpened();
}
