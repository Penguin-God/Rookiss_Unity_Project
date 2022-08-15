using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_GoHomePopup : UI_Popup
{
	enum Texts
	{
		DialogueText,
		Button1Text,
		Button2Text,
		Button3Text,
		Button4Text
	}

	enum Images
	{
		CharacterImage
	}

	enum Buttons
	{
		Button1,
		Button2,
		Button3,
		Button4
	}

	string _text;
	float _textSpeed = 0.05f;
	int _index = 0;
	Coroutine _coShowText;

	const int BUTTON_COUNT = 4;
	Button[] _buttons = new Button[BUTTON_COUNT];
	TextMeshProUGUI[] _texts = new TextMeshProUGUI[BUTTON_COUNT];

	public override bool Init()
	{
		
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindImage(typeof(Images));
		BindButton(typeof(Buttons));

		for (int i = 0; i < BUTTON_COUNT; i++)
		{
			_buttons[i] = GetButton((int)Utils.ParseEnum<Buttons>($"Button{i + 1}"));
			_texts[i] = GetText((int)Utils.ParseEnum<Texts>($"Button{i + 1}Text"));
		}

		for (int i = 0; i < BUTTON_COUNT; i++)
		{
			int index = i;
			_buttons[i].gameObject.BindEvent(() => OnClickDialogueButton(index));
		}

		Managers.Data.Players.TryGetValue((int)JobTitleType.Sinib, out PlayerData data);
		GetImage((int)Images.CharacterImage).sprite = Managers.Resource.Load<Sprite>(data.illustPath);

		RefreshUI();

		GetText((int)Texts.DialogueText).text = Managers.GetText(Define.GoHomeEvent);

		return true;
	}

	List<GoHomeData> _randomDatas = new List<GoHomeData>();

	public void SetInfo()
	{
		List<GoHomeData> goHomeDatas = Managers.Data.GoHomes.Values.ToList();

		List<int> randIndexes = new List<int>();
		for (int i = 1; i < goHomeDatas.Count; i++)
			randIndexes.Add(i);
		randIndexes.Shuffle();

		_randomDatas.Clear();

		_randomDatas.Add(goHomeDatas[0]);

		for (int i = 0; i < BUTTON_COUNT - 1; i++)
			_randomDatas.Add(goHomeDatas[randIndexes[i]]);

		RefreshUI();
	}	

	void RefreshUI()
	{
		Managers.Sound.Play(Sound.Effect, "Sound_GoHome");
		if (_init == false)
			return;

		for (int i = 0; i < _randomDatas.Count; i++)
		{
			_buttons[i].gameObject.SetActive(true);
			_texts[i].text = Managers.GetText(_randomDatas[i].nameID);
		}
	} 

	void OnClickDialogueButton(int index)
	{
		Debug.Log("OnClickButton");
		Managers.UI.ClosePopupUI(this);

		GoHomeData goHomeData = _randomDatas[index];
		List<RewardValuePair> rewards = MakeRewards(goHomeData);

		if (string.IsNullOrEmpty(goHomeData.aniPath))
			goHomeData.aniPath = "Spine/home_1_save_SkeletonData";

		string animationText = Managers.GetText(goHomeData.textID);
		Managers.UI.ShowPopupUI<UI_ResultPopup>().SetInfo(ResultType.GoHome, rewards, goHomeData.aniPath, animationText);
	}

	List<RewardValuePair> MakeRewards(GoHomeData goHomeData)
	{
		List<RewardValuePair> rewards = new List<RewardValuePair>();

		if (goHomeData.difWorkAbility != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.WorkAbility, value = goHomeData.difWorkAbility });

		if (goHomeData.difLikeability != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Likeability, value = goHomeData.difLikeability });

		if (goHomeData.difLuck != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Luck, value = goHomeData.difLuck });

		if (goHomeData.difStress != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Stress, value = goHomeData.difStress });

		if (goHomeData.difMoney != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Money, value = goHomeData.difMoney });

		return rewards;
	}

	void OnClickEventText()
	{
		Debug.Log("OnClickEventText");
		_textSpeed -= 0.02f;

		if (_index >= _text.Length)
		{
			Managers.UI.ClosePopupUI(this);
		}
	}

	void ShowText(string text)
	{
		_text = text;
		_index = 0;
		_textSpeed = 0.05f;

		if (_coShowText != null)
		{
			StopCoroutine(_coShowText);
			_coShowText = null;
		}

		_coShowText = StartCoroutine(CoShowText());
	}

	IEnumerator CoShowText()
	{
		while (true)
		{
			if (_index >= _text.Length)
			{
				GetText((int)Texts.DialogueText).text = _text;
				break;
			}

			_index++;
			string text = _text.Substring(0, _index);
			GetText((int)Texts.DialogueText).text = text;

			yield return new WaitForSeconds(_textSpeed);
		}
	}
}
