using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;

public class RadarController : BaseController
{
	string _spriteKey;

	SpriteRenderer _spriteRenderer;
	PolygonCollider2D _collider2D;

	Sequence _seq;
	bool _isStop = false;

	protected override bool Init()
	{
		if (base.Init() == false)
			return false;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_collider2D = GetComponent<PolygonCollider2D>();
		Refresh();

		return true;
	}

	public void SetInfo(string spriteKey)
	{
		_spriteKey = spriteKey;
		Refresh();
    }

    public void Refresh()
	{
		if (_init == false)
			return;

		_spriteRenderer.sprite = null;

        Managers.Resource.LoadAsync<Sprite>(_spriteKey, callback: (sprite) =>
		{
			_spriteRenderer.sprite = sprite;
			_collider2D.TryUpdateShapeToAttachedSprite();
		});
	}

	void Update()
	{
		if (_isStop == false)
			Rotate();
	}

	public void Rotate()
	{
		Managers.Game.ZRotation = (Managers.Game.ZRotation - Managers.Game.RadarAngleSpeed * Time.deltaTime) % 360.0f;
		transform.rotation = Quaternion.Euler(0, -2.2f, Managers.Game.ZRotation);
	}
	
	public void RadarFadeInOut()
	{
		_isStop = true;
		_seq = DOTween.Sequence();
		_seq.Append(_spriteRenderer.DOFade(0f, 0.1f).SetLoops(5, LoopType.Yoyo).OnComplete(()=>StartAttack()));
	}

	void StartAttack()
	{
		_isStop = false;
		_spriteRenderer.color = new Vector4(1f, 1f, 1f, 0.5f);
		(Managers.Scene.CurrentScene as GameScene).PlayerAttackStart();
	}
}
