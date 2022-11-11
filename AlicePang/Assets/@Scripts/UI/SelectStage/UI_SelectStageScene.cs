using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using static Define;

public class UI_SelectStageScene : UI_Scene
{
    enum GameObjects
    { 
        Player,
        Stand,
        SelectedChapter1,
        SelectedChapter2,
        SelectedChapter3,
        SelectedChapter4,
        SelectedChapter5,
        SelectedChapter6,
    }

    enum Buttons
    {
        ChapterButton_1,
        ChapterButton_2,
        ChapterButton_3,
        ChapterButton_4,
        ChapterButton_5,
        ChapterButton_6,
        OptionButton,
    }
    
    enum Images
    {
        MapTop,
        MapBottom,
        MapCenter1,
        MapCenter2,
    }

    UI_StageBlock[] _stageBlockUI = new UI_StageBlock[20];
    UI_StartStagePopup _startStagePopupUI;
    ScrollRect scroll;

    public ChapterResourceData ChapterData;

    public UI_InventoryPopup InventoryPopupUI;

    int _selectedChapter = 1;
    public int SelectedChapter { get { return _selectedChapter; } }

    int _selectedStage = 1;
    public int SelectedStage { get { return _selectedStage; } }

    UI_SelectStageSceneTop _topUI;
    public UI_SelectStageSceneTop TopUI { get { return _topUI; } }

    UI_SelectStageSceneBottom _bottomUI;
    public UI_SelectStageSceneBottom BottomUI { get { return _bottomUI; } }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindObject(typeof(GameObjects));

        GetButton((int)Buttons.ChapterButton_1).gameObject.BindEvent(() => { OnClickChapterButton(1); });
        GetButton((int)Buttons.ChapterButton_2).gameObject.BindEvent(() => { OnClickChapterButton(2); });
        GetButton((int)Buttons.ChapterButton_3).gameObject.BindEvent(() => { OnClickChapterButton(3); });
        GetButton((int)Buttons.ChapterButton_4).gameObject.BindEvent(() => { OnClickChapterButton(4); });
        GetButton((int)Buttons.ChapterButton_5).gameObject.BindEvent(() => { OnClickChapterButton(5); });
        GetButton((int)Buttons.ChapterButton_6).gameObject.BindEvent(() => { OnClickChapterButton(6); });

        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OnClickOptionButton);

        _topUI = Utils.FindChild<UI_SelectStageSceneTop>(gameObject, "UI_SelectStageSceneTop", true);
        _topUI.SetInfo(this);

        _bottomUI = Utils.FindChild<UI_SelectStageSceneBottom>(gameObject, "UI_SelectStageSceneBottom", true);
        _bottomUI.SetInfo(this);

        scroll = Utils.FindChild<ScrollRect>(gameObject, recursive: true);
        _selectedStage = 0;

        for (int i = 0; i < _stageBlockUI.Length; i++)
        {
            _stageBlockUI[i] = Utils.FindChild<UI_StageBlock>(gameObject, "UI_StageBlock" + (i + 1), recursive: true);
        }

        Refresh();

        return true;
    }



    public void SetInfo(int chapter)
    {
        // 챕터 이미지 데이터 세팅해줘야함
        _selectedChapter = chapter;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (Managers.Data.ChapterResources.TryGetValue(_selectedChapter, out ChapterResourceData chapterData) == false)
            return;

        ChapterData = chapterData;
        
        //stage에 따라서 앨리스 위치 지정 필요
        Managers.Resource.LoadAsync<Sprite>(chapterData.MapTop, callback: (sprite)=>
        {
            GetImage((int)Images.MapTop).sprite = sprite;
        });

        Managers.Resource.LoadAsync<Sprite>(chapterData.MapBottom, callback: (sprite) =>
        {
            GetImage((int)Images.MapBottom).sprite = sprite;
        });

        Managers.Resource.LoadAsync<Sprite>(chapterData.MapCenter, callback: (sprite) =>
        {
            GetImage((int)Images.MapCenter1).sprite = sprite;
            GetImage((int)Images.MapCenter2).sprite = sprite;
        });

        for(int i=0; i< _stageBlockUI.Length; i++)
        {
            _stageBlockUI[i].SetInfo(i + 1, this);
        }

        DisableButtonSelectedImage();
        EnableButtonSelectedImage();
    }

    void DisableButtonSelectedImage()
    {
        GetObject((int)GameObjects.SelectedChapter1).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter2).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter3).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter4).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter5).SetActive(false);
        GetObject((int)GameObjects.SelectedChapter6).SetActive(false);
    }

    void EnableButtonSelectedImage()
    {
        switch (_selectedChapter)
        {
            case 1:
                GetObject((int)GameObjects.SelectedChapter1).SetActive(true);
                break;
            case 2:
                GetObject((int)GameObjects.SelectedChapter2).SetActive(true);
                break;
            case 3:
                GetObject((int)GameObjects.SelectedChapter3).SetActive(true);
                break;
            case 4:
                GetObject((int)GameObjects.SelectedChapter4).SetActive(true);
                break;
            case 5:
                GetObject((int)GameObjects.SelectedChapter5).SetActive(true);
                break;
            case 6:
                GetObject((int)GameObjects.SelectedChapter6).SetActive(true);
                break;
        }
    }

    public void SetScrollPosition(int stageNum)
    {
        float bottomPositionY = _stageBlockUI[0].transform.position.y;
        float topPositionY = _stageBlockUI[_stageBlockUI.Length - 1].transform.position.y;
        float focusPositionY = _stageBlockUI[stageNum - 1].transform.position.y;
        //scroll.verticalNormalizedPosition = focusPositionY / (topPositionY - bottomPositionY);
        scroll.verticalNormalizedPosition = (stageNum - 1f) / ((float)_stageBlockUI.Length - 1f);
    }

    void MovePlayer(int stage)
    {
        GetObject((int)GameObjects.Player).SetActive(false);
        GetObject((int)GameObjects.Stand).SetActive(false);

        int index = stage - 1;
        GetObject((int)GameObjects.Stand).transform.position = _stageBlockUI[index].transform.position;
        GetObject((int)GameObjects.Player).transform.position = _stageBlockUI[index].transform.position;

        GetObject((int)GameObjects.Player).SetActive(true);
        GetObject((int)GameObjects.Stand).SetActive(true);

        GetObject((int)GameObjects.Player).transform.DOKill();
        GetObject((int)GameObjects.Player).transform.rotation = Quaternion.Euler(Vector3.zero);
        GetObject((int)GameObjects.Player).transform.DOPunchRotation(new Vector3(-90f, 0f, 0f), 1f, 4);
        Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");
    }    

    bool CheckChapter(int chapter)
    {
        if (chapter > Managers.Game.HighestChapter)
            return false;
        
        if (chapter == _selectedChapter)
            return false;
        
        _selectedStage = 0;
        return true;
    }

    public void ShowStartStagePopup()
    {
        Managers.UI.ShowPopupUI<UI_StartStagePopup>(callback: (popup) =>
        {
            int templateID = (_selectedChapter - 1) * 20 + _selectedStage;
            popup.SetInfo(Managers.Data.Stages[templateID]);
        });

        Managers.Sound.Play(Sound.Effect, "Sound_ButtonMain");
    }

    #region EventHandler
    public void OnSelectStage(int stage)
    {
        if (_selectedStage == stage)
        {
            ShowStartStagePopup();            
            return;
        }

        for (int i = 0; i < _stageBlockUI.Length; i++)
        {
            if(i+1 != stage)
            _stageBlockUI[i].SelectStage(false);
        }

        _selectedStage = stage;
        MovePlayer(_selectedStage);
    }

	void OnClickChapterButton(int chapter)
	{
		if (CheckChapter(chapter) == false)
			return;

		_selectedChapter = chapter;
		_selectedStage = 0;
		Refresh();
	}

    void OnClickOptionButton()
    {
        Managers.UI.ShowPopupUI<UI_Option>();
    }
    #endregion
}
