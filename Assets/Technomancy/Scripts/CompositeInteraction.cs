using System.Collections.Generic;
using UnityEngine;

public class CompositeInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] List<GameObject> _interactables;

    public void Interact()
    {
        foreach (var interactable in _interactables)
        {
            var target = interactable.GetComponent<IInteractable>();
            target?.Interact();
        }
    }
}