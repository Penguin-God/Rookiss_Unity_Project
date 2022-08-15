using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using static Define;

public class UI_EndingPopup : UI_Popup
{
	enum Texts
	{
		EndingText,
		YesButtonText,
		NoButtonText
	}

	enum Buttons
	{
		YesButton,
		NoButton
	}

	enum GameObjects
	{
		CollectionImage
	}

	EndingData _endingData;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindButton(typeof(Buttons));
		BindObject(typeof(GameObjects));

		GetButton((int)Buttons.YesButton).gameObject.BindEvent(OnContinueButton);
		GetButton((int)Buttons.NoButton).gameObject.BindEvent(OnCloseButton);

		Managers.Sound.Clear();
		Managers.Sound.Play(Sound.Effect, "Sound_Ending2");
		
		RefreshUI();

		// 엔딩 기록
		int endingIndex = _endingData.ID - 1;
		if (Managers.Game.Endings[endingIndex] == CollectionState.None)
			Managers.Game.Endings[endingIndex] = CollectionState.Uncheck;
		Managers.Game.SaveGame();

		return true;
	}

	public void SetInfo(EndingData endingData)
	{
		_endingData = endingData;
		RefreshUI();
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		if (_endingData == null)
			return;

		GetObject((int)GameObjects.CollectionImage).GetOrAddComponent<BaseController>().SetSkeletonAsset(_endingData.aniPath);
		GetText((int)Texts.EndingText).text = Managers.GetText(_endingData.nameID);
		GetText((int)Texts.YesButtonText).text = Managers.GetText(Define.ContinueGame);
		GetText((int)Texts.NoButtonText).text = Managers.GetText(Define.GoToTitle);

		if (_endingData.type == EndingType.Stress)
			GetButton((int)Buttons.YesButton).gameObject.SetActive(true);
		else
			GetButton((int)Buttons.YesButton).gameObject.SetActive(false);
	}

	void OnContinueButton()
	{
		if (Managers.IAP.IsNoAds == false)
			Managers.Ads.ShowInterstitialAds();

		Managers.Game.Stress = 0;
		Managers.UI.ClosePopupUI();
		Managers.Sound.Play(Sound.Bgm, "Sound_MainPlayBGM", volume: 0.2f);
	}

	void OnCloseButton()
	{
		Managers.Game.Init();

		Managers.UI.ClosePopupUI(this);
		Managers.UI.PeekPopupUI<UI_PlayPopup>().ClosePopupUI();
		Managers.Game.OnStressChanged = null;
		Managers.Game.OnNewCollection = null;

		Managers.UI.ShowPopupUI<UI_TitlePopup>();
	}
}
