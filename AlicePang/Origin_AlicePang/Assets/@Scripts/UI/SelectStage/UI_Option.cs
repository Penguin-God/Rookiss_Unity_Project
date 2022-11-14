using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_Option : UI_Popup
{
    enum GameObjects
    {
        BGMOnImage,
        BGMOffImage,
        EffectSoundOnImage,
        EffectSoundOffImage,
        Block,
    }

    enum Buttons
    {
        BGMButton,
        EffectSoundButton,
        CloseButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.BGMButton).gameObject.BindEvent(OnClickBGMButton);
        GetButton((int)Buttons.EffectSoundButton).gameObject.BindEvent(OnClickEffectSoundButton);

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        GetObject((int)GameObjects.Block).BindEvent(OnClickCloseButton);

        BGMOnOffImage(Managers.Game.BGMOn);
        EffectSoundOnOffImage(Managers.Game.EffectSoundOn);

        Refresh();
        return true;
    }

    public void Refresh()
    {
        if (_init == false)
            return;
    }

    void BGMOnOffImage(bool bgmOn)
    {
        GetObject((int)GameObjects.BGMOnImage).SetActive(bgmOn);
        GetObject((int)GameObjects.BGMOffImage).SetActive(!bgmOn);
    }

    void EffectSoundOnOffImage(bool effectSoundOn)
    {
        GetObject((int)GameObjects.EffectSoundOnImage).SetActive(effectSoundOn);
        GetObject((int)GameObjects.EffectSoundOffImage).SetActive(!effectSoundOn);
    }

    #region EventHnaler

    void OnClickBGMButton()
    {
        bool bgmOn = Managers.Game.BGMOn;
        bgmOn = !bgmOn;
        Managers.Game.BGMOn = bgmOn;

        BGMOnOffImage(bgmOn);

        if (!bgmOn)
            Managers.Sound.Stop(Define.Sound.Bgm);
        else
            Managers.Sound.Play(Define.Sound.Bgm);
    }

    void OnClickEffectSoundButton()
    {
        bool effectSoundOn = Managers.Game.EffectSoundOn;
        effectSoundOn = !effectSoundOn;
        Managers.Game.EffectSoundOn = effectSoundOn;

        EffectSoundOnOffImage(effectSoundOn);
        if (!effectSoundOn)
            Managers.Sound.Stop(Define.Sound.SubBgm);
        else
            Managers.Sound.Play(Define.Sound.SubBgm);
    }

    void OnClickCloseButton()
    {
        Managers.Sound.Play(Sound.Effect, "Sound_HomeButton");
        Managers.UI.ClosePopupUI(this);
        Managers.Game.SaveGame();
    }

    #endregion
}
