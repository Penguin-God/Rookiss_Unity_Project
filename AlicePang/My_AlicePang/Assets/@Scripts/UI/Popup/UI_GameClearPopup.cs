using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_GameClearPopup : UI_Popup
{
    enum GameObjects
    {

    }

    enum Images
    {
        TicketImage,
        ADImage,
    }

    enum Texts
    {
        StageText,
        KillMonsterRewardText,
        ClearStageRewardText,
    }

    enum Buttons
    {
        DoubleRewardButton,
        ExitButton,
    }

    int _killReward;
    int _stageReward;
    int _stage;

    public override bool Init()
    {
        

        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
        GetButton((int)Buttons.DoubleRewardButton).gameObject.BindEvent(OnClickDoubleRewardButton);

        Refresh();

        return true;
    }

    public void SetInfo(int killReward, int stageReward, int stage)
    {
        _killReward = killReward;
        _stageReward = stageReward;
        _stage = stage;
    }

    void Refresh()
    {
        if (_init == false)
            return;

        GetText((int)Texts.ClearStageRewardText).text = _stageReward.ToString();
        GetText((int)Texts.KillMonsterRewardText).text = _killReward.ToString();
        GetText((int)Texts.StageText).text = _stage.ToString();
        Managers.Sound.Clear();
        Managers.Sound.Play(Sound.Effect, "Sound_StageClear");
    }

    #region EventHandler

    void OnClickDoubleRewardButton()
    {
        Debug.Log("On click show ads button");
        //???? ?????? ???? ????
        //?????? ???? ????
    }

    void OnClickExitButton()
    {
        Managers.Scene.ChangeScene(Define.SceneType.SelectStageScene);
    }

    #endregion
}
