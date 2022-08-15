using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class UI_CollectionPopup : UI_Popup
{
	enum GameObjects
	{
		Content
	}

	enum Texts
	{
		TitleText,
		CommonButtonText,
		GalleryButtonText,
	}

	enum Buttons
	{
		ExitButton,
		CommonButton,
		CommonButtonSelected,
		GalleryButton,
		GalleryButtonSelected,
	}

	enum Images
	{
		CommonIconNotice,
		GalleryIconNotice,
	}

	enum CollectionButtonType
	{
		Common,
		Gallery
	}

	CollectionButtonType _type = CollectionButtonType.Common;
	List<UI_CollectionItem> _items = new List<UI_CollectionItem>();

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindObject(typeof(GameObjects));
		BindText(typeof(Texts));
		BindButton(typeof(Buttons));
		BindImage(typeof(Images));

		GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClosePopup);

		GetButton((int)Buttons.CommonButton).gameObject.BindEvent(() => OnClickButton(CollectionButtonType.Common));
		GetButton((int)Buttons.CommonButtonSelected).gameObject.BindEvent(() => OnClickButton(CollectionButtonType.Common));
		GetButton((int)Buttons.GalleryButton).gameObject.BindEvent(() => OnClickButton(CollectionButtonType.Gallery));
		GetButton((int)Buttons.GalleryButtonSelected).gameObject.BindEvent(() => OnClickButton(CollectionButtonType.Gallery));

		RefreshUI();

		return true;
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		GetImage((int)Images.CommonIconNotice).gameObject.SetActive(false);
		GetImage((int)Images.GalleryIconNotice).gameObject.SetActive(false);
		{
			bool uncheck = Managers.Game.Collections.Where(c => { return c == CollectionState.Uncheck; }).ToList().Count > 0;
			GetImage((int)Images.CommonIconNotice).gameObject.SetActive(uncheck);
		}
		{
			bool uncheck = Managers.Game.Endings.Where(e => { return e == CollectionState.Uncheck; }).ToList().Count > 0;
			GetImage((int)Images.GalleryIconNotice).gameObject.SetActive(uncheck);
		}

		GetText((int)Texts.TitleText).text = Managers.GetText(Define.CollectionPageTitle);
		GetText((int)Texts.CommonButtonText).text = Managers.GetText(Define.CollectionPageTab1);
		GetText((int)Texts.GalleryButtonText).text = Managers.GetText(Define.CollectionPageTab2);

		// Grid
		_items.Clear();

		Transform parent = GetObject((int)GameObjects.Content).gameObject.transform;
		foreach (Transform t in parent)
			Managers.Resource.Destroy(t.gameObject);

		foreach (CollectionData data in Managers.Data.Collections.Values)
		{
			var item = Managers.UI.MakeSubItem<UI_CollectionItem>(parent.transform);
			item.SetCollectionInfo(data.ID);

			_items.Add(item);
		}

		// ?????? ?????? ?????????? ???? ?????? ???? :)
		List<EndingData> endingList = Managers.Data.Endings.Values.ToList();
		for (int i = 0; i < endingList.Count; i++)
			_items[i].SetGalleryInfo(endingList[i].ID);

		RefreshButton();
	}

	void RefreshButton()
	{
		if (_type == CollectionButtonType.Common)
		{
			Managers.Sound.Play(Sound.Effect, ("Sound_UpgradeDone"));
			GetButton((int)Buttons.CommonButton).gameObject.SetActive(false);
			GetButton((int)Buttons.CommonButtonSelected).gameObject.SetActive(true);
			GetButton((int)Buttons.GalleryButton).gameObject.SetActive(true);
			GetButton((int)Buttons.GalleryButtonSelected).gameObject.SetActive(false);
		}
		else
		{
			Managers.Sound.Play(Sound.Effect, ("Sound_UpgradeDone"));
			GetButton((int)Buttons.CommonButton).gameObject.SetActive(true);
			GetButton((int)Buttons.CommonButtonSelected).gameObject.SetActive(false);
			GetButton((int)Buttons.GalleryButton).gameObject.SetActive(false);
			GetButton((int)Buttons.GalleryButtonSelected).gameObject.SetActive(true);
		}

		foreach (UI_CollectionItem item in _items)
		{
			if (_type == CollectionButtonType.Common)
				item.SetType(UI_CollectionItem.CollectionItemType.Collection);
			else
				item.SetType(UI_CollectionItem.CollectionItemType.Gallery);
		}
	}

	void OnClickButton(CollectionButtonType type)
	{
		_type = type;
		RefreshButton();
	}

	void OnClosePopup()
	{
		Managers.Sound.Play(Sound.Effect, ("Sound_CancelButton"));
		Managers.UI.ClosePopupUI(this);

		for (int i = 0; i < Managers.Game.Collections.Length; i++)
		{
			if (Managers.Game.Collections[i] == CollectionState.Uncheck)
				Managers.Game.Collections[i] = CollectionState.Done;
		}

		for (int i = 0; i < Managers.Game.Endings.Length; i++)
		{
			if (Managers.Game.Endings[i] == CollectionState.Uncheck)
				Managers.Game.Endings[i] = CollectionState.Done;
		}

		Managers.Game.SaveGame();
	}
}
