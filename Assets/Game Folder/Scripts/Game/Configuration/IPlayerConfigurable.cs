using UnityEngine;

public interface IPlayerConfigurable
{
    float InteractRange { get; }
    LayerMask InteractLayers { get; }
}
