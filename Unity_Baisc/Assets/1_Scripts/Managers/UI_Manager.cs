using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager
{
    int _order = 10; // 기본 UI랑 팝업 UI 오더 다르게 하기 위해 초기값 10으로 세팅

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;

    Transform _root;
    public Transform Root
    {
        get
        {
            if(_root == null) _root = new GameObject("@UI_Root").transform;
            return _root;
        }
    }

    public void SetCanvas(GameObject go, bool sort)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true; // canvas안의 canvas가 부모 관계없이 독립적인 sort값을 가지게 하는 옵션

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base 
    {
        if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

        GameObject go = Managers.Resources.Instantiate($"UI/SubItem/{name}");
        if (parent != null) go.transform.SetParent(parent);
        return go.GetOrAddComponent<T>();
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

        GameObject go = Managers.Resources.Instantiate($"UI/Scene/{name}");
        T sceneUI = go.GetOrAddComponent<T>();
        _sceneUI = sceneUI;
        go.transform.SetParent(Root);
        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name)) name = typeof(T).Name;

        GameObject go = Managers.Resources.Instantiate($"UI/Popup/{name}");
        T popup = go.GetOrAddComponent<T>();
        _popupStack.Push(popup);
        go.transform.SetParent(Root);
        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0) return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("팝업창 닫는거 실패!!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0) return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resources.Destroy(popup.gameObject);
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
        {
            ClosePopupUI();
        }
    }

    public void Clear()
    {
        _popupStack.Clear();
        _sceneUI = null;
    }
}
