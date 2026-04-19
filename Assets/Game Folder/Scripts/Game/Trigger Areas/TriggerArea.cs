using System;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] private Collider _collider;

    public event Action<TriggerArea> PlayerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other is CharacterController)
            PlayerEntered?.Invoke(this);
    }
}
