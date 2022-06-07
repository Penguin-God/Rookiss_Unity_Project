using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using Random = UnityEngine.Random;

public class UI_DialoguePopup : UI_Popup
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

	JobTitleType _npcJob;

	bool _salaryNegotation = false;
	DialogueEventData _data;

	const float START_SECOND_PER_CHAR = 0.05f;

	string _text;
	float _secondPerCharacter = START_SECOND_PER_CHAR;
	Action _onTextEndCallback;
	int _index = 0;
	Coroutine _coShowText;

	const int BUTTON_COUNT = 4;
	Button[] _buttons = new Button[BUTTON_COUNT];
	TextMeshProUGUI[] _texts = new TextMeshProUGUI[BUTTON_COUNT];

	[SerializeField]
	Sprite _normalButtonImage;
	[SerializeField]
	Sprite _blockButtonImage;

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

			if (_salaryNegotation)
				_buttons[i].gameObject.BindEvent(() => OnClickNegotiationButton(index));
			else
				_buttons[i].gameObject.BindEvent(() => OnClickSpeechButton(index));
		}

		gameObject.BindEvent(OnClickText);
		GetText((int)Texts.DialogueText).gameObject.BindEvent(OnClickText);

		RefreshUI();

		return true;
	}

	void OnClickText()
	{
		if (_index >= _text.Length)
		{
			if (_coShowText != null)
			{
				StopCoroutine(_coShowText);
				_coShowText = null;
			}

			_onTextEndCallback?.Invoke();
		}
		else
		{
			_secondPerCharacter = Math.Max(_secondPerCharacter - 0.01f, 0);
		}
	}

	public void SetInfo(JobTitleType npcJob, bool salaryNegotation = false)
	{
		_npcJob = npcJob;
		_salaryNegotation = salaryNegotation;

		if (salaryNegotation)
		{
			// TODO
		}
		else if ((int)Managers.Game.JobTitle >= (int)npcJob)
			_data = Managers.Data.InferiorEvents.GetRandom();
		else
			_data = Managers.Data.SuperiorEvents.GetRandom();

		RefreshUI();
	}	

	void RefreshUI()
	{
		
		if (_init == false)
			return;

		if (_salaryNegotation)
			RefreshSalaryNegotiationUI();
		else
			RefreshSpeechDialogueUI();

		if (Managers.Data.Players.TryGetValue((int)_npcJob, out PlayerData enemyData))
		{
			Sprite sprite = Managers.Resource.Load<Sprite>(enemyData.battleIconPath);
			GetImage((int)Images.CharacterImage).sprite = sprite;
			GetImage((int)Images.CharacterImage).SetNativeSize();
			GetImage((int)Images.CharacterImage).gameObject.transform.localScale = 2.5f * Vector3.one;
		}
	}

	void RefreshSalaryNegotiationUI()
	{
		if (_salaryNegotation == false)
			return;

		_buttons[0].gameObject.SetActive(true);
		_buttons[0].image.sprite = _normalButtonImage;
		_texts[0].text = Managers.GetText(Managers.Data.SalaryNegotiation.yesAnswerID);

		_buttons[1].gameObject.SetActive(true);
		_buttons[1].image.sprite = _normalButtonImage;
		_texts[1].text = Managers.GetText(Managers.Data.SalaryNegotiation.noAnswerID);

		_buttons[2].gameObject.SetActive(false);
		_buttons[3].gameObject.SetActive(false);

		string questionStr = Managers.GetText(Managers.Data.SalaryNegotiation.questionID);
		ShowText(questionStr);
	}

	void RefreshSpeechDialogueUI()
	{
		if (_salaryNegotation)
			return;

		if (_data == null)
			return;

		for (int i = 0; i < BUTTON_COUNT; i++)
		{
			if (i == _data.answers.Count)
			{
				_buttons[i].gameObject.SetActive(true);
				_buttons[i].image.sprite = _blockButtonImage;
				_texts[i].text = Managers.GetText(Define.BlockHit);

				if (Managers.Game.BlockCount == 0)
					_buttons[i].gameObject.SetActive(false);
			}
			else if (i < _data.answers.Count)
			{
				_buttons[i].gameObject.SetActive(true);
				_buttons[i].image.sprite = _normalButtonImage;
				_texts[i].text = Managers.GetText(_data.answers[i].answerID);
			}
			else
			{
				_buttons[i].gameObject.SetActive(false);
			}
		}

		string questionStr = Managers.GetText(_data.questionID);
		ShowText(questionStr);
	}

	void HideAllButtons()
	{
		for (int i = 0; i < 4; i++)
			_buttons[i].gameObject.SetActive(false);
	}
	
	void OnClickNegotiationButton(int index)
	{
		Debug.Log("OnClickButton");

		if (_salaryNegotation == false)
			return;
		
		HideAllButtons();

		if (index == 0)
		{
			string answerText = Managers.GetText(Managers.Data.SalaryNegotiation.yesResultID);
			ShowText(answerText, onTextEndCallback: () =>
			{
				Managers.UI.ClosePopupUI(this);

				int salaryPercent = Managers.Data.SalaryNegotiation.yesIncreaseSalaryPercent;
				List<RewardValuePair> rewards = new List<RewardValuePair>();
				rewards.Add(new RewardValuePair() { type = RewardType.SalaryIncrease, value = salaryPercent });
				Managers.UI.ShowPopupUI<UI_ResultPopup>().SetInfo(ResultType.SalaryNegotiationSuccess, rewards, path: "", text: "");
			});
		}
		else
		{
			string answerText = "";
			int increasePercent = 0;

			int randNumber = Random.Range(0, 101);

			if (randNumber < Managers.Game.BlockHitSucessPercent)
			{
				answerText = Managers.GetText(Managers.Data.SalaryNegotiation.noResultGoodID);
				increasePercent = Managers.Data.SalaryNegotiation.noIncreaseSalaryPercentGood;
			}	
			else
			{
				answerText = Managers.GetText(Managers.Data.SalaryNegotiation.noResultBadID);
				increasePercent = Managers.Data.SalaryNegotiation.noIncreaseSalaryPercentBad;
			}	

			ShowText(answerText, onTextEndCallback: () =>
			{
				Managers.UI.ClosePopupUI(this);
				List<RewardValuePair> rewards = new List<RewardValuePair>();
				rewards.Add(new RewardValuePair() { type = RewardType.SalaryIncrease, value = increasePercent });
				Managers.UI.ShowPopupUI<UI_ResultPopup>().SetInfo(ResultType.SalaryNegotiationFail, rewards, path: "", text: "");
			});
		}		
	}

	void OnClickSpeechButton(int index)
	{
		Debug.Log("OnClickButton");

		if (_salaryNegotation || _data == null)
			return;

		if (index == _data.answers.Count)
		{
			// 벽돌 때리기 선택
			if (Managers.Data.BlockEvents.TryGetValue((int)BlockEventId, out BlockEventData blockEventData))
			{
				HideAllButtons();

				BlockEventAnsData answer = null;

				int randNumber = UnityEngine.Random.Range(0, 101);
				if (randNumber < Managers.Game.BlockHitSucessPercent)
					answer = blockEventData.ansData.Where(d => { return d.success == 0; }).FirstOrDefault(); // 좋은 케이스
				else
					answer = blockEventData.ansData.Where(d => { return d.success == 1; }).FirstOrDefault(); // 나쁜 케이스

				Debug.Log($"Rand : {randNumber} BlockHitSucessPercent : {Managers.Game.BlockHitSucessPercent}");

				string answerText = Managers.GetText(answer.resultID).Replace("{enemyID}", Utils.GetJobTitleString(_npcJob));
				ShowText(answerText, onTextEndCallback: () =>
				{
					Managers.UI.ClosePopupUI(this);
					List<RewardValuePair> rewards = MakeRewards(answer);
					Managers.UI.ShowPopupUI<UI_ResultPopup>().SetInfo(ResultType.Dialogue, rewards, path: "", text: "");
				});
			}
		}
		else
		{
			HideAllButtons();

			DialogueAnsData answer = _data.answers[index];

			string answerText = Managers.GetText(answer.resultID);
			ShowText(answerText, onTextEndCallback: () =>
			{
				Managers.UI.ClosePopupUI(this);
				List<RewardValuePair> rewards = MakeRewards(answer);
				Managers.UI.ShowPopupUI<UI_ResultPopup>().SetInfo(ResultType.Dialogue, rewards, path: "", text: "");
			});
		}
	}

	List<RewardValuePair> MakeRewards(DialogueAnsData dialgueAnsData)
	{
		List<RewardValuePair> rewards = new List<RewardValuePair>();

		if (dialgueAnsData.difWorkAbility != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.WorkAbility, value = dialgueAnsData.difWorkAbility });

		if (dialgueAnsData.difLikeability != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Likeability, value = dialgueAnsData.difLikeability });

		if (dialgueAnsData.difLuck != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Luck, value = dialgueAnsData.difLuck });

		if (dialgueAnsData.difStress != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Stress, value = dialgueAnsData.difStress });

		if (dialgueAnsData.difMoney != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Money, value = dialgueAnsData.difMoney });
		
		if (dialgueAnsData.difBlock != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Block, value = dialgueAnsData.difBlock });

		return rewards;
	}

	List<RewardValuePair> MakeRewards(BlockEventAnsData blockAnsData)
	{
		List<RewardValuePair> rewards = new List<RewardValuePair>();

		if (blockAnsData.difWorkAbility != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.WorkAbility, value = blockAnsData.difWorkAbility });

		if (blockAnsData.difLikability != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Likeability, value = blockAnsData.difLikability });

		if (blockAnsData.difLuck != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Luck, value = blockAnsData.difLuck });

		if (blockAnsData.difBlock != 0)
			rewards.Add(new RewardValuePair() { type = RewardType.Block, value = blockAnsData.difBlock });

		return rewards;
	}

	void ShowText(string text, Action onTextEndCallback = null)
	{
		Managers.Sound.Play(Sound.Speech, "Sound_DialogSpeak");

		_text = text;
		_index = 0;
		_secondPerCharacter = START_SECOND_PER_CHAR;
		_onTextEndCallback = onTextEndCallback;

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
				Managers.Sound.Stop(Sound.Speech);
				yield return new WaitForSeconds(1.5f);
				_onTextEndCallback?.Invoke();
				break;
			}

			_index++;
			string text = _text.Substring(0, _index);
			GetText((int)Texts.DialogueText).text = text;

			yield return new WaitForSeconds(_secondPerCharacter);
		}
	}
}
