using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Player Configuration", menuName = "LD59/Player/Configuration")]
public class PlayerConfigurationData : ScriptableObject, IPlayerConfigurable
{
    [SerializeField, Min(0f)] private float _interactRange;
    [SerializeField] private LayerMask _interactLayers;

    public float InteractRange => _interactRange;
    public LayerMask InteractLayers => _interactLayers;

    private void OnValidate()
    {
        if (_interactLayers == 0)
            throw new ArgumentOutOfRangeException(nameof(_interactLayers));
    }
}
