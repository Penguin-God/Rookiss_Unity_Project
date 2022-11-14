using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    Vector2 targetPosition;
    int _coin;

    Sequence _seq;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        targetPosition = (Managers.UI.SceneUI as UI_GameScene).CoinPosition();
    }


    public void SetInfo(Vector2 dropPos, int amount = 1)
    {
        _coin = amount;
        transform.position = dropPos;
        gameObject.SetActive(true);
        AnimateCoin();
    }

    public void AnimateCoin()
    {
        float duration = Random.Range(0.1f, 0.8f);

        //_seq.Kill();
        _seq = DOTween.Sequence();

        _seq.Append(transform.DOJump(Random.insideUnitCircle + (Vector2)transform.position, 1.7f, 1, 0.25f));
        _seq.AppendInterval(0.0f); 
        _seq.Append(transform.DOMove(targetPosition, duration)
            .SetEase(Ease.InExpo)
            .OnComplete(() =>
            {
                (Managers.UI.SceneUI as UI_GameScene).ShowCoin += _coin;
                Managers.Sound.Play(Define.Sound.Effect, "Sound_CollectCoin");
                gameObject.SetActive(false);
            }));
    }

}
