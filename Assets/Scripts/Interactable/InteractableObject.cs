using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Interactable
{
    public abstract class InteractableObject : MonoBehaviour
    {
        [Header("Interactable options")]
        public bool IsInteractable;

        public bool AutomaticInteraction;

        public abstract void Interact();
    }
}