using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

public class UI_WeaponGachaPopup : UI_Popup
{
    public class GachaWeapon
    {
        public int TemplateID;
        public int CurrentCount;
        public int GetCount;
    }

    enum GameObjects
    {
        TouchPanel,
        NormalCard,
        RareCard,
        EpicCard,
        NewIcon,
        XPFill,
        XPFill_Full,
        XPIcon,
        XPIcon_Full,
        UI_GachaItem,
        InventoryIcon,
    }

    enum Images
    {
        WeaponImage,
    }

    enum Texts
    {
        DamageText,
        LevelText,
        ExpText,
        GetCardCountText,
    }

    enum Sliders
    {
        ExpSlider,
    }

    enum AnimationSequence
    {
        None,
        StartGacha,
        WaitForOpenIdle,
        Open,
        AfterOpenIdle,
        WaitForShowCardIdle,
        ShowCard,
        EndGacha,
    }

    SkeletonGraphic _gachaAnimation;
    AnimationSequence _animationSequence;
    List<GachaWeapon> _gachaWeapons;

    int _currentIndex;
    Spine.AnimationState state;
    bool _isShowCard = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindSlider(typeof(Sliders));

        GetObject((int)GameObjects.TouchPanel).BindEvent(OnClickTouchPanel);
        GetObject((int)GameObjects.UI_GachaItem).SetActive(false);
        GetObject((int)GameObjects.InventoryIcon).SetActive(false);

        GetText((int)Texts.GetCardCountText).gameObject.SetActive(false);

        GameObject go = Utils.FindChild(gameObject, "WeaponGacha", true);
        _gachaAnimation = Utils.GetOrAddComponent<SkeletonGraphic>(go);
        
        state = _gachaAnimation.AnimationState;
        _animationSequence = AnimationSequence.None;

        _isShowCard = false;

        Refresh();
        
        return true;
    }

    void Gacha(int gachaCount)
    {
        float totalGachaRate = 0f;

        for (int i = 1; i <= Managers.Data.Weapons.Count; i++)
        {
            totalGachaRate += Managers.Data.Weapons[i].GachaRate;
        }

        _gachaWeapons = new List<GachaWeapon>();
        for (int i = 0; i < gachaCount; i++)
        {
            float rand = Random.Range(0f, totalGachaRate);
            float rate = 0f;
            for (int j = 1; j <= Managers.Data.Weapons.Count; j++)
            {
                rate += Managers.Data.Weapons[j].GachaRate;
                if (rate >= rand)
                {
                    int count = Random.Range(40, 50);
                    GachaWeapon a = new GachaWeapon
                    {
                        TemplateID = Managers.Data.Weapons[j].TemplateID,
                        CurrentCount = Managers.Game.WeaponExp[j - 1] + count,
                        GetCount = count, 
                    };
                    _gachaWeapons.Add(a);
                    Managers.Game.WeaponExp[j - 1] += count;
                    if (Managers.Game.WeaponLevel[j - 1] < 1)
                        Managers.Game.WeaponLevel[j - 1] = 1;
                    break;
                }
            }
        }
        Managers.Game.SaveGame();

        _currentIndex = 0;
    }

    public void SetInfo(int gachaCount)
    {
        Gacha(gachaCount);
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        StartGachaAnimation();
    }

    void SetCard(int index)
    {
        int templateID = _gachaWeapons[index].TemplateID;
        int getCount = _gachaWeapons[index].GetCount;
        int currentCount = _gachaWeapons[index].CurrentCount;

        Managers.Data.Weapons.TryGetValue(templateID, out WeaponData weaponData);

        int weaponIndex = weaponData.TemplateID - 1;
        int weaponLevel = Managers.Game.WeaponLevel[weaponIndex];

        GetText((int)Texts.LevelText).text = $"LV{weaponLevel}";
        GetText((int)Texts.GetCardCountText).text = $"X {getCount}";

        if (weaponLevel < weaponData.weaponLevelData.Count)
        {
            GetText((int)Texts.ExpText).text = $"{currentCount} / {weaponData.weaponLevelData[weaponLevel - 1].Exp}";
            GetSlider((int)Sliders.ExpSlider).value = currentCount / (float)weaponData.weaponLevelData[weaponLevel - 1].Exp;
            if (currentCount >= weaponData.weaponLevelData[weaponLevel - 1].Exp)
                ExpFullResource(true);
            else
                ExpFullResource(false);

            GetText((int)Texts.DamageText).text = weaponData.weaponLevelData[weaponLevel - 1].Damage.ToString();
        }
        else
        {
            GetImage((int)Images.WeaponImage).color = Color.white;
            GetText((int)Texts.ExpText).text = $"MAX";
            GetSlider((int)Sliders.ExpSlider).value = 1f;
            GetText((int)Texts.DamageText).text = weaponData.weaponLevelData[weaponLevel - 1].Damage.ToString();
        }

        Managers.Resource.LoadAsync<Sprite>(weaponData.Sprite, callback: (sprite) =>
        {
            GetImage((int)Images.WeaponImage).sprite = sprite;
        });
    }

    void ExpFullResource(bool isFull)
    {
        //GetObject((int)GameObjects.ExpFullIcon).SetActive(isFull);
        GetObject((int)GameObjects.XPFill).SetActive(!isFull);
        GetObject((int)GameObjects.XPFill_Full).SetActive(isFull);
        GetObject((int)GameObjects.XPIcon).SetActive(!isFull);
        GetObject((int)GameObjects.XPIcon_Full).SetActive(isFull);
    }

    void StartGachaAnimation()
    {
        _gachaAnimation.AnimationState.SetAnimation(0, "start", false);
        _animationSequence = AnimationSequence.StartGacha;

        state.Complete -= MotionToWaitForOpenIdle;
        state.Complete += MotionToWaitForOpenIdle;       
    }

    void MotionToWaitForOpenIdle(Spine.TrackEntry entry)
    {
        state.Complete -= MotionToWaitForOpenIdle;

        WaitForOpenAnimation();
    }

    void WaitForOpenAnimation()
    {
        _gachaAnimation.AnimationState.SetAnimation(0, "idle1", true);
        _animationSequence = AnimationSequence.AfterOpenIdle;
    }
    void OpenAnimation()
    {
        _gachaAnimation.AnimationState.SetAnimation(0, "open", false);
        _animationSequence = AnimationSequence.Open;

        Spine.AnimationState state = _gachaAnimation.AnimationState;

        state.Complete -= MotionToAfterOpen;
        state.Complete += MotionToAfterOpen;
    }

    void MotionToAfterOpen(Spine.TrackEntry entry)
    {
        state.Complete -= MotionToAfterOpen;
        AfterOpenAnimation();
    }

    void AfterOpenAnimation()
    {
        _gachaAnimation.AnimationState.SetAnimation(0, "idle2", true);
        _animationSequence = AnimationSequence.AfterOpenIdle;
    }

    void WaitForShowCardAnimation()
    {
        _gachaAnimation.AnimationState.SetAnimation(0, "idle3", false);
        _animationSequence = AnimationSequence.WaitForShowCardIdle;
    }

    void ShowCardAnimation()
    {
        if(_isShowCard)
        {
            CardMoveToInventory();
            return;
        }

        if(_currentIndex >= _gachaWeapons.Count)
        {
            //°¡Ã­ Á¾·á
            EndGachaAnimation();
            return;
        }
        _animationSequence = AnimationSequence.ShowCard;
        SetCard(_currentIndex);

        GetObject((int)GameObjects.UI_GachaItem).transform.DOKill(true);
        GetObject((int)GameObjects.UI_GachaItem).transform.position = new Vector3(548f, 883f);
        GetObject((int)GameObjects.UI_GachaItem).transform.localScale = Vector3.one * 0.5f;

        GetObject((int)GameObjects.UI_GachaItem).SetActive(true);

        GetObject((int)GameObjects.UI_GachaItem).transform.DOJump(new Vector3(548f, 1340f), 300f, 1, 1f);
        GetObject((int)GameObjects.UI_GachaItem).transform.DOScale(Vector3.one, 0.5f);

        GetText((int)Texts.GetCardCountText).gameObject.SetActive(true);

        _isShowCard = true;

        _currentIndex++;
        _gachaAnimation.AnimationState.SetAnimation(0, "card", false);
    }

    void CardMoveToInventory()
    {
        Vector3 targetPos = GetObject((int)GameObjects.InventoryIcon).transform.position;
        GetObject((int)GameObjects.UI_GachaItem).transform.localScale = Vector3.one;
        GetObject((int)GameObjects.InventoryIcon).SetActive(true);

        GetObject((int)GameObjects.UI_GachaItem).transform.DOKill(true);
        GetObject((int)GameObjects.UI_GachaItem).transform.position = new Vector3(548f, 1340f);
        GetObject((int)GameObjects.UI_GachaItem).transform.DOScale(Vector3.one * 0.3f, 0.3f);
        GetObject((int)GameObjects.UI_GachaItem).transform.DOMove(targetPos, 0.3f).OnComplete(() =>
        {
            GetObject((int)GameObjects.InventoryIcon).SetActive(false);
            GetObject((int)GameObjects.UI_GachaItem).SetActive(false);
        });

        GetText((int)Texts.GetCardCountText).gameObject.SetActive(false);
        WaitForShowCardAnimation();

        _isShowCard = false;
    }

    void EndGachaAnimation()
    {
        _gachaAnimation.AnimationState.SetAnimation(0, "close2", false);
        _animationSequence = AnimationSequence.EndGacha;
        //ÆË¾÷ Á¾·á
        GetObject((int)GameObjects.TouchPanel).SetActive(false);

        ClosePopup();
    }

    void ClosePopup()
    {
        Managers.UI.ClosePopupUI(this);
    }

    #region EventHandler
    void OnClickTouchPanel()
    {
        Debug.Log("ClickTouchPanel");
        switch(_animationSequence)
        {
            case AnimationSequence.StartGacha:
                state.Complete -= MotionToWaitForOpenIdle;
                WaitForOpenAnimation();
                break;
            case AnimationSequence.WaitForOpenIdle:
                OpenAnimation();
                break;
            case AnimationSequence.Open:
                state.Complete -= MotionToAfterOpen;
                AfterOpenAnimation();
                break;
            case AnimationSequence.AfterOpenIdle:
                WaitForShowCardAnimation();
                break;
            case AnimationSequence.WaitForShowCardIdle:
                ShowCardAnimation();
                break;
            case AnimationSequence.ShowCard:
                ShowCardAnimation();
                break;
            case AnimationSequence.EndGacha:
                break;
        }
    }
    #endregion
}
