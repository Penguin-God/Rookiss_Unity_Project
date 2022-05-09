using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Object = UnityEngine.Object;

public class UI_Button : UI_Base
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

    void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        GetText((int)Texts.ScoreText).text = "안녕?";
    }

    

    int score = 0;
    public void OnButtonClicked()
    {
        score++;
    }
}
