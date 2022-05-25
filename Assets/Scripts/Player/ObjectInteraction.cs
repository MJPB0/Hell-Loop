using Assets.Scripts.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    private CircleCollider2D pickupRange;

    [SerializeField] private InteractableObject closestObject;
    [SerializeField] private List<InteractableObject> objectsInRange;

    public InteractableObject ClosestObject { get { return closestObject; } }

    private void Start()
    {
        objectsInRange = new List<InteractableObject>();

        pickupRange = GetComponent<CircleCollider2D>();
        pickupRange.radius = GetComponentInParent<Player>().PickupRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable")) return;

        InteractableObject obj = collision.gameObject.GetComponent<InteractableObject>();

        if (obj.AutomaticInteraction && obj.IsInteractable) 
            obj.Interact();
        else if (!objectsInRange.Contains(obj))
            objectsInRange.Add(obj);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable")) return;

        InteractableObject obj = collision.gameObject.GetComponent<InteractableObject>();

        if (objectsInRange.Contains(obj))
            objectsInRange.Remove(obj);

        if (objectsInRange.Count < 1)
            closestObject = null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Interactable")) 
            return;

        if (objectsInRange.Count > 0) 
            GetClosestObject();
    }

    public void Interact()
    {
        if (!closestObject) 
            return;
        if (!closestObject.IsInteractable)
        {
            Debug.Log($"Can't interact with this object : {closestObject.name}!");
            return;
        }

        closestObject.Interact();
    }

    private void GetClosestObject()
    {
        if (objectsInRange.Count < 1)
        {
            closestObject = null;
            return;
        }

        InteractableObject closest = null;
        foreach (InteractableObject obj in objectsInRange)
        {
            if (closest == null)
            {
                closest = obj;
                continue;
            }

            float d1 = Vector2.Distance(closest.transform.position, transform.position);
            float d2 = Vector2.Distance(obj.transform.position, transform.position);

            if (d2 < d1) closest = obj;
        }
        closestObject = closest;
    }
}