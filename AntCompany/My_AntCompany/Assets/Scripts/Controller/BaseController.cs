using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Spine.Unity;
using System;

public class BaseController : UI_Base
{
	protected SkeletonGraphic _anim = null;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		_anim = GetComponent<SkeletonGraphic>();
		return true;
	}

	protected virtual void UpdateAnimation() { }

	public virtual void LookLeft(bool flag)
	{
		Vector3 scale = transform.localScale;
		if (flag)
			transform.localScale = new Vector3(Math.Abs(scale.x), scale.y, scale.z);
		else
			transform.localScale = new Vector3(-Math.Abs(scale.x), scale.y, scale.z);
	}

	#region Spine Animation
	public void SetSkeletonAsset(string path)
	{
		Init();
		_anim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(path);
		_anim.Initialize(true);
	}

	public void PlayAnimation(string name, bool loop = true)
	{
		Init();
		_anim.startingAnimation = name;
		_anim.startingLoop = loop;
	}

	public void ChangeSkin(string name)
	{
		Init();
		_anim.initialSkinName = name;
		_anim.Initialize(true);
	}

	public void Refresh()
	{
		Init();
		_anim.Initialize(true);
	}

	public void PlayAnimationOnce(string name)
	{
		StartCoroutine(CoPlayAnimationOnce(name));
	}

	IEnumerator CoPlayAnimationOnce(string name)
	{
		bool defaultLoop = _anim.startingLoop;
		string defaultName = _anim.startingAnimation;

		_anim.startingLoop = false;
		_anim.startingAnimation = name;

		float length = _anim.skeletonDataAsset.GetSkeletonData(true).FindAnimation(name).Duration;
		yield return new WaitForSeconds(length); // 애니 시간만큼 대기

		// 기존 애니 복원
		PlayAnimation(defaultName, defaultLoop);
	}

	public void PlayAnimationOnce(string skin, string name)
	{
		StartCoroutine(CoPlayAnimationOnce(skin, name));
	}

	IEnumerator CoPlayAnimationOnce(string skin, string name)
	{
		bool defaultLoop = _anim.startingLoop;
		string defaultSkin = _anim.initialSkinName;
		string defaultName = _anim.startingAnimation;

		_anim.startingLoop = false;
		_anim.startingAnimation = name;
		ChangeSkin(skin);

		float length = _anim.skeletonDataAsset.GetSkeletonData(true).FindAnimation(name).Duration;
		yield return new WaitForSeconds(length); // 애니 시간만큼 대기

		// 기존 애니 복원
		PlayAnimation(defaultName, defaultLoop);
		ChangeSkin(defaultSkin);
	}
	#endregion
}
