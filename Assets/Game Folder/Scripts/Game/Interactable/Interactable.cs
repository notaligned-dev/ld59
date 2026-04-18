using System;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public event Action<IInteractable> Interacted;
    public event Action<IInteractable> Looked;

    public void TriggerInteraction()
    {
        Debug.Log("Interacted");
        Interacted?.Invoke(this);
    }

    public void TriggerLookAction()
    {
        Debug.Log("Looked");
        Looked?.Invoke(this);
    }
}
