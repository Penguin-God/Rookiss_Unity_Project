using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
	protected bool _init = false;
	protected Coroutine _coWait;

	void Start()
	{
		Init();
	}

	protected virtual bool Init()
	{
		if (_init)
			return false;

		_init = true;
		return true;
	}

	protected void WaitFor(float seconds)
	{
		if (_coWait != null)
			StopCoroutine(_coWait);

		_coWait = StartCoroutine(CoWait(seconds));
	}

	IEnumerator CoWait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_coWait = null;
	}

	#region 애니메이션
	//protected virtual void UpdateAnimation() { }

	//public void FlipX(bool flag)
	//{
	//	Init();
	//	_mecanim.initialFlipX = flag;
	//	_mecanim.Initialize(true);
	//}

	//public void PlayAnimationByTrigger(string name, float duration = 0)
	//{
	//	Init();

	//	_animator.SetTrigger(name);

	//	float length = _animator.GetCurrentAnimatorClipInfo(0).Length;
	//	if (duration == 0)
	//		duration = length;

	//	_animator.speed = length / duration;
	//}

	//public void PlayAnimation(string name, float duration = 0)
	//{
	//	Init();

	//	_animator.Play(name);

	//	float length = _animator.GetCurrentAnimatorClipInfo(0).Length;
	//	if (duration == 0)
	//		duration = length;

	//	_animator.speed = length / duration;
	//}

	//public void ChangeSkin(string name)
	//{
	//	Init();
	//	_mecanim.initialSkinName = name;
	//	_mecanim.Initialize(true);
	//}

	//public void RefreshAnimation()
	//{
	//	Init();
	//	_mecanim.Initialize(true);
	//}

	//public void SetAlpha(float value)
	//{
	//	Init();
	//	_mecanim.skeleton.A = value;
	//}
	#endregion
}
