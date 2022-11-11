using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestItem : UI_Base
{
    enum Texts
    {
        DescriptionText,
        RewardText,
        CountText,
    }

    enum Buttons
    {
        QuestButton,
    }

    enum Images
    {
        RewardIcon,
    }

    enum Sliders
    {
        QuestSlider,
    }

    QuestData _questData;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindSlider(typeof(Sliders));

        GetButton((int)Buttons.QuestButton);

        Refresh();
        return true;
    }

    void Refresh()
    {
        if (_init == false)
            return;


        GetText((int)Texts.RewardText).text = _questData.RewardCount.ToString();
        GetText((int)Texts.CountText).text = $"currentCount/{_questData.QuestCount}";
        Managers.Resource.LoadAsync<Sprite>(_questData.RewardType, callback: (sprite) =>
        {
            GetImage((int)Images.RewardIcon).sprite = sprite;
        });
    }

    public void SetInfo(QuestData questData)
    {
        _questData = questData;


        Refresh();
    }

    #region EvenetHandler

    void OnClickQuestButton()
    {

    }

    #endregion
}
