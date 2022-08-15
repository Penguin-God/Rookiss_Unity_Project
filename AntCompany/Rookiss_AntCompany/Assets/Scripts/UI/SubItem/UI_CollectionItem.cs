using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CollectionItem : UI_Base
{
	enum GameObjects
	{
		Collection,
		CollectionLock,
		Gallery,
	}

	enum Texts
	{
		CollectionName,
		LikeabilityText,
		WorkAbilityText,
		LuckText,
		MaxHpText,
	}

	enum Images
	{
		CollectionIcon,
		IconNotice,
		GalleryImage,
	}

	public enum CollectionItemType
	{
		Collection,
		Gallery
	}

	int _collectionId;
	int _galleryId;
	CollectionItemType _type;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindObject(typeof(GameObjects));
		BindText(typeof(Texts));
		BindImage(typeof(Images));

		RefreshUI();

		return true;
	}

	public void SetCollectionInfo(int collectionId)
	{
		if (collectionId > 0)
			_collectionId = collectionId;
	}

	public void SetGalleryInfo(int galleryId)
	{
		if (galleryId > 0)
			_galleryId = galleryId;
	}

	public void SetType(CollectionItemType type)
	{
		_type = type;
		RefreshUI();
	}

	void RefreshCollectionUI()
	{
		gameObject.SetActive(false);

		if (_collectionId == 0)
			return;

		gameObject.SetActive(true);

		CollectionState state = Managers.Game.Collections[_collectionId - 1];

		GetObject((int)GameObjects.Gallery).SetActive(false);

		if (state == CollectionState.None)
		{
			GetObject((int)GameObjects.CollectionLock).SetActive(true);
			GetObject((int)GameObjects.Collection).SetActive(false);
		}
		else
		{
			string path = Managers.Data.Collections[_collectionId].iconPath;
			GetImage((int)Images.CollectionIcon).sprite = Managers.Resource.Load<Sprite>(path);
			GetObject((int)GameObjects.CollectionLock).SetActive(false);
			GetObject((int)GameObjects.Collection).SetActive(true);
		}

		Managers.Data.Collections.TryGetValue(_collectionId, out CollectionData data);

		GetText((int)Texts.CollectionName).text = Managers.GetText(data.nameID);
		GetText((int)Texts.LikeabilityText).text = $"+{data.difLikability}";
		GetText((int)Texts.WorkAbilityText).text = $"+{data.difWorkAbility}";
		GetText((int)Texts.LuckText).text = $"+{data.difLuck}";
		GetText((int)Texts.MaxHpText).text = $"+{data.difMaxHp}";

		if (state == CollectionState.Uncheck)
			SetNoti(true);
		else
			SetNoti(false);
	}

	void RefreshGalleryUI()
	{
		gameObject.SetActive(false);

		if (_galleryId == 0)
			return;

		CollectionState state = Managers.Game.Endings[_galleryId - 1];
		
		gameObject.SetActive(true);

		GetObject((int)GameObjects.Collection).SetActive(false);

		if (state == CollectionState.None)
		{
			GetObject((int)GameObjects.CollectionLock).SetActive(true);
			GetObject((int)GameObjects.Gallery).SetActive(false);
		}
		else
		{
			Managers.Data.Endings.TryGetValue(_galleryId, out EndingData data);
			Debug.Log($"GalleryID {_galleryId}");
			Debug.Log($"Sprite {data.illustPath}");
			GetImage((int)Images.GalleryImage).sprite = Managers.Resource.Load<Sprite>(data.illustPath);
			GetObject((int)GameObjects.CollectionLock).SetActive(false);
			GetObject((int)GameObjects.Gallery).SetActive(true);
		}

		if (state == CollectionState.Uncheck)
			SetNoti(true);
		else
			SetNoti(false);
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		if (_type == CollectionItemType.Collection)
			RefreshCollectionUI();
		else
			RefreshGalleryUI();
	}

	void SetNoti(bool flag)
	{
		if (_init == false)
			return;

		GetImage((int)Images.IconNotice).gameObject.SetActive(flag);
	}
}
