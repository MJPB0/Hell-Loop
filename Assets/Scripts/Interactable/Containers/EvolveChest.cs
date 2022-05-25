using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveChest : Container
{
    private PlayerInventory playerInventory;

    private void Start()
    {
        base.Start();

        playerInventory = FindObjectOfType<PlayerInventory>();
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        anim.SetTrigger(CONTAINER_OPEN);
    }

    public override void ContainerOpened() => StartCoroutine(EvolveWeapon());

    private IEnumerator EvolveWeapon()
    {
        collider.enabled = false;
        spriteRenderer.sprite = null;

        playerInventory.TryToEvolveWeapon();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(CONTAINER_OPENED));

        if (willBeDestroyed)
            Destroy(gameObject);
    }
}
