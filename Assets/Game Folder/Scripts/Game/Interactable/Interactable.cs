using System;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public event Action<IInteractable> Interacted;
    public event Action<IInteractable> Looked;

    public void TriggerInteraction()
    {
        Interacted?.Invoke(this);
    }

    public void TriggerLookAction()
    {
        Looked?.Invoke(this);
    }
}
