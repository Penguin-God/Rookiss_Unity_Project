using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class UI_StageBlock : UI_Base
{ 
    enum GameObjects
    {
        StageTileSpine,
        ClearFlag,
    }

    enum Texts
    {
        StageText,
    }

    enum Buttons
    {
        SelectStageButton,
    }
    enum Images
    {
        Shadow
    }
    int _stage;

    UI_SelectStageScene _selectStageSceneUI;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));

        GetButton((int)Buttons.SelectStageButton).gameObject.BindEvent(OnClickStageButton);

        return true;
    }

    public void SetInfo(int stage, UI_SelectStageScene scene)
    {
        if (_init == false)
            return;

        _stage = stage;
        _selectStageSceneUI = scene;

        Refresh();

        int selectStage = Managers.Game.HighestChapter > scene.SelectedChapter ? 20 : Managers.Game.HighestStage;
        if (stage == selectStage)
        {
            OnClickStageButton();
            _selectStageSceneUI.SetScrollPosition(selectStage);
        }
    }

    void Refresh()
    {
        GetText((int)Texts.StageText).text = _stage.ToString();

        if(Managers.Game.HighestStage == _stage)
        {
            GetObject((int)GameObjects.ClearFlag).SetActive(false);
        }

        Managers.Resource.LoadAsync<Sprite>(_selectStageSceneUI.ChapterData.StageBlock, callback: (sprite) =>
        {
            Utils.GetOrAddComponent<Image>(GetButton((int)Buttons.SelectStageButton).gameObject).sprite = sprite;
        });

        Managers.Resource.LoadAsync<SkeletonDataAsset>(_selectStageSceneUI.ChapterData.StageTileSpine, callback: (spine) =>
        {
            SkeletonGraphic skeleton = Utils.GetOrAddComponent<SkeletonGraphic>(GetObject((int)GameObjects.StageTileSpine).gameObject);
            skeleton.skeletonDataAsset = spine;
            skeleton.Initialize(true);
            skeleton.startingAnimation = "animation";

        });

        Managers.Resource.LoadAsync<Sprite>(_selectStageSceneUI.ChapterData.StageBlock, callback: (sprite) =>
        {
            GetComponent<Image>().sprite = sprite;
        });

        Managers.Resource.LoadAsync<Sprite>(_selectStageSceneUI.ChapterData.Shadow, callback: (sprite) =>
        {
            GetImage((int)Images.Shadow).sprite = sprite;
            GetImage((int)Images.Shadow).gameObject.SetActive(IsReachableStage());
        });
    }

    bool IsReachableStage()
    {
        if (_selectStageSceneUI.SelectedChapter < Managers.Game.HighestChapter)
            return true;

        //갈 수 없는 지역
        if (Managers.Game.HighestStage < _stage)
            return false;

        return true;
    }

    public void SelectStage(bool selected)
    {
        GetObject((int)Images.Shadow).gameObject.SetActive(!selected);
    }

    public void OnClickStageButton()
    {
        //(Managers.Scene.CurrentScene as SelectStageScene)?.OnSelectStage(_stage);
        if (!IsReachableStage())
            return;

        _selectStageSceneUI?.OnSelectStage(_stage);
        SelectStage(true);
    }
}
