using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GlobalLifetimeScope : LifetimeScope
{
    [SerializeField] private bool _isMock;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<PlayerSaveSystem>(Lifetime.Singleton);
        builder.Register<StoryProgress>(Lifetime.Singleton);
        builder.Register<StoryService>(Lifetime.Scoped).AsSelf();

        if (_isMock == false)
        {
            builder.RegisterComponentInHierarchy<MainMenuView>().AsSelf();
        }
    }
}
