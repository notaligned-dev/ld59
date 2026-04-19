using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private PlayerConfigurationData _playerConfiguration;
    [SerializeField] private StoryObjectsProvider _storyObjectsProvider;
    [SerializeField] private Camera _camera;

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_playerConfiguration, nameof(_playerConfiguration)),
            (_camera, nameof(_camera)),
            (_storyObjectsProvider, nameof(_storyObjectsProvider))
        );
    }

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_playerConfiguration).As<IPlayerConfigurable>();
        builder.RegisterInstance(_camera).AsSelf();
        builder.RegisterInstance(_storyObjectsProvider).AsSelf();
        builder.RegisterComponentInHierarchy<StoryProgressIngameController>().AsSelf();
        builder.RegisterComponentInHierarchy<PlayerLook>();
        builder.RegisterComponentInHierarchy<PlayerController>();
        builder.RegisterComponentInHierarchy<DevilBookView>();
        builder.RegisterComponentInHierarchy<DevilSymbolView>();
    }
}
