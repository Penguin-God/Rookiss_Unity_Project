using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UI_StoryPopup : UI_Popup
{
    [Serializable]
    class CutSceneSequence
    {
        public GameObject[] CutScenes;
    }

    [SerializeField]
    CutSceneSequence[] _sequence;
    Coroutine _playCutScene;

    enum GameObjects
    {
        SkipPanel,
    }

    int _currentSequence = 0;
    int _scenesIndex = 0;

    bool _isComplete = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));

        GetObject((int)GameObjects.SkipPanel).BindEvent(OnClickSkipPanel);

        _playCutScene = StartCoroutine(CoPlayCutScene());
        return true;
    }

    void NextScene()
    {
        if (_scenesIndex < _sequence[_currentSequence].CutScenes.Length)
        {
            _sequence[_currentSequence].CutScenes[_scenesIndex].SetActive(true);
            _scenesIndex++;
            time = 0f;
        }
        else
        {
            _isComplete = true;
        }
    }

    void NextSequence()
    {
        if (_currentSequence < _sequence.Length - 1)
        {
            for (int i = 0; i < _sequence[_currentSequence].CutScenes.Length; i++)
            {
                _sequence[_currentSequence].CutScenes[i].SetActive(false);
            }
            _currentSequence++;
            _scenesIndex = 0;
            if (_playCutScene != null)
                StopCoroutine(_playCutScene);

            _isComplete = false;
            _playCutScene = StartCoroutine(CoPlayCutScene());
        }
        else
            Managers.UI.ClosePopupUI(this);
    }

        float time = 0f;
    IEnumerator CoPlayCutScene()
    {
        time = 0f;
        while (true)
        {
            time += Time.deltaTime;

            if (time > 2f)
            {
                NextScene();
            }
            if (_scenesIndex >= _sequence[_currentSequence].CutScenes.Length)
                break;

            yield return new WaitForFixedUpdate();
        }


        yield return new WaitForSeconds(1f);

        _isComplete = true;
        _playCutScene = null;
    }

    #region EventHandler
    void OnClickSkipPanel()
    {
        Debug.Log("Click");
        if (_isComplete)
            NextSequence();
        else
            NextScene();
    }
    #endregion
}
