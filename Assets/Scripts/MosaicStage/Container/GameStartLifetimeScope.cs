using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameStartLifetimeScope : LifetimeScope
{
    [SerializeField]
    private ObstacleBall obstacleBallPrefab;

    [SerializeField]
    private Transform[] obstacleBallTrans;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(obstacleBallPrefab);
        builder.RegisterComponent(obstacleBallTrans);

        builder.RegisterComponentInHierarchy<LifeView>();
        builder.RegisterComponentInHierarchy<MainGameInfoView>();
        builder.RegisterComponentInHierarchy<TileGridBehaviour>();
        builder.RegisterComponentInHierarchy<ObstacleBehaviour>();

        builder.Register<LifeModel>(Lifetime.Singleton);
        builder.Register<MainGameManager>(Lifetime.Singleton);
        builder.Register<GridCalculator>(Lifetime.Singleton);
        builder.Register<MainGamePresenter>(Lifetime.Singleton);

        builder.UseEntryPoints(Lifetime.Singleton, entryPoints => {
            //entryPoints.Add<MainGameManager>();

            // Presenter から Entry させる(そうしないとコンストラクタが2回走るため)
            //entryPoints.Add<GridCalculator>();


            //entryPoints.Add<ObstacleBehaviour>();
            entryPoints.Add<MainGamePresenter>();
        });
        //builder.RegisterEntryPoint<MainGamePresenter>();
        //builder.RegisterEntryPoint<GridCalculator>();
        //builder.RegisterEntryPoint<ObstacleBehaviour>();
    }
}
