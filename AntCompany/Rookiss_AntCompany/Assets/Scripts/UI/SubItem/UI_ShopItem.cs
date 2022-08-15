using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using static Define;

public class UI_ShopItem : UI_Base
{
	enum Buttons
	{
		BuyButton,
		AdsButton
	}

	enum Texts
	{
		ItemText,
		BuyButtonText,
		AdsButtonText,
	}

	enum GameObjects
	{
		Icon
	}

	ShopData _shopData;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindButton(typeof(Buttons));
		BindText(typeof(Texts));
		BindObject(typeof(GameObjects));

		GetButton((int)Buttons.BuyButton).gameObject.BindEvent(OnClickButton);
		GetButton((int)Buttons.AdsButton).gameObject.BindEvent(OnClickButton);

		RefreshUI();

		return true;
	}

	public void SetInfo(ShopData shopData)
	{
		_shopData = shopData;
		RefreshUI();
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		GetObject((int)GameObjects.Icon).GetOrAddComponent<BaseController>().SetSkeletonAsset(_shopData.icon);
		GetText((int)Texts.ItemText).text = Managers.GetText(_shopData.name);

		if (_shopData.condition == ShopConditionType.Cash)
		{
			GetButton((int)Buttons.BuyButton).gameObject.SetActive(true);
			GetText((int)Texts.BuyButtonText).text = $"{_shopData.price}";
			GetButton((int)Buttons.AdsButton).gameObject.SetActive(false);
		}
		else
		{
			GetButton((int)Buttons.BuyButton).gameObject.SetActive(false);
			GetButton((int)Buttons.AdsButton).gameObject.SetActive(true);
			GetText((int)Texts.AdsButtonText).text = Managers.GetText(Define.WatchAD);
		}
	}

	void OnClickButton()
	{
		Debug.Log("OnClickButton");

		if (_shopData.condition == ShopConditionType.Cash)
		{
			// 현금 구매형
			Managers.IAP.Purchase(_shopData.productID, (product, failureReason) =>
			{
				Debug.Log($"Purchase Done {product.transactionID} {failureReason}");
				// 성공했는지 확인
				if (failureReason == PurchaseFailureReason.Unknown)
					GiveReward();
			});
		}
		else
		{
			// 광고 보상형
			Managers.Ads.ShowRewardedAds(() => { GiveReward(); });
			Managers.Sound.Play(Sound.Bgm, "Sound_MainPlayBGM", volume: 0.2f);
		}
	}

	void GiveReward()
	{
		switch (_shopData.rewardType)
		{
			case ShopRewardType.Block:
				Managers.Game.BlockCount += _shopData.rewardCount;
				Managers.UI.PeekPopupUI<UI_PlayPopup>().RefreshMoney();
				break;
			case ShopRewardType.Money:
				Managers.Game.Money += _shopData.rewardCount;
				Managers.UI.PeekPopupUI<UI_PlayPopup>().RefreshMoney(true);
				break;
			case ShopRewardType.Luck:
				Managers.Game.Luck += _shopData.rewardCount;
				Managers.UI.PeekPopupUI<UI_PlayPopup>().RefreshStat();
				break;
			case ShopRewardType.NoAds:
				Managers.UI.PeekPopupUI<UI_PlayPopup>().PopulateShop();
				break;
		}
	}
}
