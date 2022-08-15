using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : BaseController
{
	public enum EmoticonType
	{
		None,
		Exclamation,
		Question
	}

	PlayerData _data;

	public JobTitleType JobTitle;

	public AnimState State
	{
		get { return _playerState.state; }
		set 
		{ 
			_playerState.state = value; 
			UpdateAnimation(); 
		}
	}

	public bool GoHomeEvent 
	{ 
		get { return _playerState.goHomeEvent; }
		set 
		{
			_playerState.goHomeEvent = value;
			if (value)
				SetEmoticon(EmoticonType.Exclamation); 
			else
				SetEmoticon(EmoticonType.None);
		}
	}

	public bool DialogueEvent 
	{ 
		get { return _playerState.dialogueEvent; }
		set 
		{
			_playerState.dialogueEvent = value;
			if (value)
				SetEmoticon(EmoticonType.Question);
			else
				SetEmoticon(EmoticonType.None);
		}
	}

	GameObject _exclamation;
	GameObject _question;
	PlayerState _playerState;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		_playerState = Managers.Game.GetPlayerState(JobTitle);
		
		gameObject.BindEvent(OnClickPlayer);

		Managers.Data.Players.TryGetValue((int)JobTitle, out _data);
		_anim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(_data.spine);
		State = AnimState.Working;

		_exclamation = Utils.FindChild(gameObject, "Exclamation");
		_question = Utils.FindChild(gameObject, "Question");
		SetEmoticon(EmoticonType.None);

		return true;
	}

	public override void LookLeft(bool flag)
	{
		base.LookLeft(flag);

		Vector3 scale = transform.localScale;

		if (flag)
		{
			_exclamation.transform.localScale = new Vector3(Math.Abs(scale.x), scale.y, scale.z);
			_question.transform.localScale = new Vector3(Math.Abs(scale.x), scale.y, scale.z);
		}			
		else
		{
			_exclamation.transform.localScale = new Vector3(-Math.Abs(scale.x), scale.y, scale.z);
			_question.transform.localScale = new Vector3(-Math.Abs(scale.x), scale.y, scale.z);
		}	
	}

	EmoticonType _emoticon = EmoticonType.None;

	public void SetEmoticon(EmoticonType type)
	{
		if (_emoticon != EmoticonType.None && _emoticon == type)
			return;

		_emoticon = type;

		switch (type)
		{
			case EmoticonType.None:
				_exclamation?.SetActive(false);
				_question?.SetActive(false);
				break;
			case EmoticonType.Exclamation:
				_exclamation?.SetActive(true);
				_question?.SetActive(false);
				Managers.Sound.Play(Sound.Effect, "Sound_Exclamation");
				break;
			case EmoticonType.Question:
				_exclamation?.SetActive(false);
				_question?.SetActive(true);
				Managers.Sound.Play(Sound.Effect, "Sound_Question");
				break;
		}
	}

	public void SetInfo(JobTitleType jobTitle)
	{
		JobTitle = jobTitle;
		UpdateAnimation();
	}

	protected override void UpdateAnimation()
	{
		Init();

		// 고양이는 애니메이션 없음
		if (JobTitle == JobTitleType.Cat)
			return;

		switch (State)
		{
			case AnimState.Idle:
				PlayAnimation(_data.aniIdle);
				ChangeSkin(_data.aniIdleSkin);
				break;
			case AnimState.Sweat:
				PlayAnimation(_data.aniSweat);
				ChangeSkin(_data.aniSweatSkin);
				break;
			case AnimState.Working:
				PlayAnimation(_data.aniWorking);
				ChangeSkin(_data.aniWorkingSkin);
				break;
			case AnimState.Walking:
				PlayAnimation(_data.aniWalk);
				ChangeSkin(_data.aniWalkSkin);
				break;
			case AnimState.Attack:
				PlayAnimationOnce(_data.aniAttack, _data.aniAttackSkin);
				break;
		}
	}

	void OnClickPlayer()
	{
		Debug.Log("OnClicked");
		
		if (GoHomeEvent)
		{
			Managers.UI.ShowPopupUI<UI_GoHomePopup>().SetInfo();
			GoHomeEvent = false;
			State = AnimState.Working;
		}

		if (DialogueEvent)
		{
			if (JobTitle == JobTitleType.Cat)
			{
				// 고양이는 연봉 협상 :)
				Managers.UI.ShowPopupUI<UI_DialoguePopup>().SetInfo(JobTitleType.Sajang, salaryNegotation: true);
				DialogueEvent = false;
				State = AnimState.Working;
			}
			else
			{
				Managers.UI.ShowPopupUI<UI_DialoguePopup>().SetInfo(JobTitle);
				DialogueEvent = false;
				State = AnimState.Working;
			}
		}
	}
}
