using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_GameOverPopup : UI_Popup
{
	enum GameObjects
	{
		TicketImage,
		ADImage,
	}

	enum Images
	{

	}

	enum Buttons
	{
		ReviveButton,
		ExitButton,
	}

	enum Texts
	{
		StageText,
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;
		BindObject(typeof(GameObjects));
		BindText(typeof(Texts));
		BindButton(typeof(Buttons));

		GetButton((int)Buttons.ReviveButton).gameObject.BindEvent(OnClickReviveButton);
		GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
		Managers.Sound.Clear();
		Managers.Sound.Play(Sound.Effect, "Sound_GameOverFirst");
		Managers.Sound.Play(Sound.Effect, "Sound_GameOver");

        return true;
		//SceneType
	}

	void RefreshUI()
	{
		//?????? ?????? ???????? ???? ????
		GetObject((int)GameObjects.TicketImage).SetActive(false);
		GetText((int)Texts.StageText).text = $"STAGE {Managers.Game.SelectedStage}";
	}
    #region EventHandler
    void OnClickReviveButton()
	{
		Debug.Log("OnReviveButton");
		//???? ????
		Managers.UI.ClosePopupUI(this);
		(Managers.Scene.CurrentScene as GameScene).RevivePlayer();
		Managers.Sound.Play(Sound.Bgm, "Sound_Battle1");
	}

	void OnClickRestartButton()
	{
		//???? ?? ????
		Debug.Log("OnRestartButton");
		(Managers.Scene.CurrentScene as GameScene).RestartGame();
		Managers.UI.ClosePopupUI(this);
	}

	void OnClickExitButton()
    {
		Debug.Log("OnExitButton");
		Managers.Scene.ChangeScene(Define.SceneType.SelectStageScene);
    }
    #endregion
}
