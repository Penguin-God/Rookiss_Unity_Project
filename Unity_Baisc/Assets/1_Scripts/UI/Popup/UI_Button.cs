using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Button : UI_Popup
{
    enum Buttons
    {
        PointButton,
    }

    enum Texts
    {
        PointText,
        ScoreText,
    }

    enum GameObjects
    {
        HaHaHa,
    }

    enum Images
    {
        ItemIcon,
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.PointButton).gameObject.BindEvnet(OnButtonClicked);

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvnet(go, data => go.transform.position = data.position, Define.UI_Event.Drag);
    }


    int score = 0;
    public void OnButtonClicked(PointerEventData data)
    {
        score++;
        GetText((int)Texts.ScoreText).text = $"score : {score}";
    }
}
