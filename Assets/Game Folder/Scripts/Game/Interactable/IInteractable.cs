using System;

public interface IInteractable
{
    event Action<IInteractable> Interacted;
    event Action<IInteractable> Looked;

    void TriggerLookAction();
    void TriggerInteraction();
}
