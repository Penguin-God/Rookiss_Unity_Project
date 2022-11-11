using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;

public class UI_StartStagePopup : UI_Popup
{
    enum GameObjects
    {
        MissionListPanel,
        WeaponListPanel,
        Popup,
        Block,
    }

    enum Texts
    {
        StageText,
    }

    enum Buttons
    { 
        StartButton,
        CloseButton,
    }

    UI_WeaponItem _shortRangeWeaponItemUI;
    UI_WeaponItem _middleRangeWeaponItemUI;
    UI_WeaponItem _longRangeWeaponItemUI;

    UI_MissionMonsterItem[] _missionMonsterItemUI = new UI_MissionMonsterItem[4];

    StageData _stageData;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        GetObject((int)GameObjects.Block).BindEvent(OnClickCloseButton);

        GameObject parent = GetObject((int)GameObjects.WeaponListPanel);
        Managers.UI.MakeSubItem<UI_WeaponItem>(parent.transform, "UI_WeaponItem", callback: (item) =>
        {
            _shortRangeWeaponItemUI = item;
            _shortRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID]);
        });

        Managers.UI.MakeSubItem<UI_WeaponItem>(parent.transform, "UI_WeaponItem", callback: (item) =>
        {
            _middleRangeWeaponItemUI = item;
            _middleRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID]);
        });

        Managers.UI.MakeSubItem<UI_WeaponItem>(parent.transform, "UI_WeaponItem", callback: (item) =>
         {
             _longRangeWeaponItemUI = item;
             _longRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID]);
         });

        parent = GetObject((int)GameObjects.MissionListPanel);

        List<RespawnData> respawnData = _stageData.respawnData;
        int missionCount = 0;
        for (int i = 0; i < respawnData.Count; i++)
        {
            if (respawnData[i].ClearCount > 0)
            {
                int index = i;
                int missionIndex = missionCount;

                Managers.UI.MakeSubItem<UI_MissionMonsterItem>(parent.transform, "UI_MissionMonsterItem", callback: (item) =>
                {
                    _missionMonsterItemUI[missionIndex] = item;
                    _missionMonsterItemUI[missionIndex]?.SetInfo(respawnData[index].MonsterID, respawnData[index].ClearCount);
                });
                missionCount++;
            }
        }

        Refresh();
        return true;
    }

    public void SetInfo(StageData stageData)
    {
        _stageData = stageData;
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        //???? ???? ????
        _shortRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.ShortRangeWeaponID]);
        _middleRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.MiddleRangeWeaponID]);
        _longRangeWeaponItemUI?.SetInfo(Managers.Data.Weapons[Managers.Game.LongRangeWeaponID]);

        GetText((int)Texts.StageText).text = $"STAGE {_stageData.StageID}";

        //GetObject((int)GameObjects.Popup).transform.DOScale(1f, 0.2f).From(0f);
        GetObject((int)GameObjects.Popup).SetActive(false);
        StartCoroutine(CoWaitLoad());
    }

    IEnumerator CoWaitLoad()
    {
        while (Managers.Resource.HandlesCount > 0)
            yield return null;
        GetObject((int)GameObjects.Popup).SetActive(true);

        GetObject((int)GameObjects.Popup).transform.DOScale(1f, 0.2f).From(0f);
    }

    #region EventHandler
    void OnClickStartButton()
    {
        Managers.Game.SelectedChapter = _stageData.ChapterID;
        Managers.Game.SelectedStage = _stageData.StageID;
        Managers.Game.SaveGame();
        Managers.Scene.ChangeScene(Define.SceneType.GameScene);

        Managers.Sound.Clear();
        Managers.Sound.Play(Sound.Effect, "Sound_ButtonSelected");
        
        
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Sound.Play(Sound.Effect, "Sound_HomeButton");
    }
    #endregion
}
