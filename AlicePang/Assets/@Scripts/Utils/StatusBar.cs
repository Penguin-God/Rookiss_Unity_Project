using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusBar : MonoBehaviour
{
	[SerializeField]
	//Transform _hpBar = null;
	SpriteRenderer _hpBar = null;
	const float _minWidth = 0.2f;
	const float _maxWidth = 2.2f;

	[SerializeField]
	TextMeshPro _turnText = null;

	[SerializeField]
	TextMeshPro _hpText = null;

	public void SetTurnText(int turn)
	{
		_turnText.text = $"{turn}";
	}

	void SetHpText(int hp)
	{
		_hpText.text = hp.ToString();
	}

	void SetHpBar(int hp, int maxHp)
	{
		if(maxHp == 1)
		{
			this.gameObject.SetActive(false);
		}

		float ratio = hp / (float)maxHp;
		ratio = _minWidth + Mathf.Clamp(ratio, 0f, 1f) * 2f;
		_hpBar.size = new Vector2(ratio, _hpBar.size.y);
	}

	public void SetHp(int hp, int maxHp)
    {
		SetHpBar(hp, maxHp);
		SetHpText(hp);
    }
}
