using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;

        SpawningPool spawningPool = new GameObject("SpawningPool").AddComponent<SpawningPool>();
        spawningPool.SetKeepMonsterCount(3);

        GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);
        //Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
    }

    public override void Clear()
    {
        
    }
}
