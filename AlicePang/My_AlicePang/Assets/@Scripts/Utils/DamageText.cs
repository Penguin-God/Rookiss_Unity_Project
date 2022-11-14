using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
	TextMeshPro _damageText;

	public void SetInfo(Vector2 pos, int damage)
	{
		_damageText = GetComponent<TextMeshPro>();
		transform.position = pos;
		_damageText.text = $"{damage}";
	}

	private void OnEnable()
	{
		Sequence seq = DOTween.Sequence();

		transform.localScale = new Vector3(1f, 1f, 1f);

		seq.Append(transform.DOScale(2.7f, 0.17f).SetEase(Ease.InOutBounce));
		seq.AppendInterval(0.1f);
		seq.Append(transform.DOScale(0f, 0.1f).OnComplete(()=>Destroy(gameObject)));
	}
}
