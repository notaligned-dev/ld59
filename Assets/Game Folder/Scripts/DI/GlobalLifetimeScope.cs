using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GlobalLifetimeScope : LifetimeScope
{
    [SerializeField] private bool _isMock;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<PlayerSaveSystem>(Lifetime.Singleton);

        if (_isMock == false)
        {
            builder.RegisterComponentInHierarchy<MainMenuView>().AsSelf();
        }
    }
}
