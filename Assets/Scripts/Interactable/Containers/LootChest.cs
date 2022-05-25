using Assets.Scripts.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootChest : Container
{
    [Space]
    [SerializeField] protected Transform interactablesParent;
    [SerializeField] protected Transform pickablesParent;

    [Space]
    [SerializeField] protected int minDropAmount = 0;
    [SerializeField] protected int maxDropAmount = 3;

    [Space]
    [SerializeField] protected float timeBetweenDropSpawns = .2f;
    [SerializeField] protected float minSpawnRadius = 5f;
    [SerializeField] protected float maxSpawnRadius = 20f;

    [Space]
    [SerializeField] protected float timeToToggleCollider = .5f;

    private void Start()
    {
        base.Start();

        interactablesParent = GameObject.FindGameObjectWithTag(INTERACTABLES_TAG).transform;
        pickablesParent = GameObject.FindGameObjectWithTag(PICKABLES_TAG).transform;
    }

    public override void ContainerOpened() => StartCoroutine(SpawnDrops());

    private IEnumerator SpawnDrops()
    {
        int amount = Random.Range(minDropAmount, maxDropAmount + 1);

        collider.enabled = false;
        spriteRenderer.sprite = null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(CONTAINER_OPENED));

        for (int i = 0; i < amount; i++)
        {
            int dropIndex = Random.Range(0, containerObject.Drops.Length);

            GameObject objToSpawn = containerObject.Drops[dropIndex];
            bool isAutomaticallyPickable = objToSpawn.GetComponent<InteractableObject>().AutomaticInteraction;
            GameObject obj = Instantiate(objToSpawn, isAutomaticallyPickable ? pickablesParent : interactablesParent);

            StartCoroutine(WaitAndToggleCollider(obj.GetComponent<Collider2D>()));

            int xMult = Random.Range(-1, 1) >= 0 ? 1 : -1;
            int yMult = Random.Range(-1, 1) >= 0 ? 1 : -1;

            float x = Random.Range(minSpawnRadius, maxSpawnRadius) * xMult;
            float y = Random.Range(minSpawnRadius, maxSpawnRadius) * yMult;

            obj.transform.position = new Vector3(
                gameObject.transform.position.x + x,
                gameObject.transform.position.y + y,
                gameObject.transform.position.z
            );

            yield return new WaitForSeconds(timeBetweenDropSpawns);
        }

        yield return new WaitForSeconds(timeToToggleCollider);
        if (willBeDestroyed)
            Destroy(gameObject);
    }

    private IEnumerator WaitAndToggleCollider(Collider2D lootCollider)
    {
        lootCollider.enabled = false;
        yield return new WaitForSeconds(timeToToggleCollider);
        lootCollider.enabled = true;
    }
}
