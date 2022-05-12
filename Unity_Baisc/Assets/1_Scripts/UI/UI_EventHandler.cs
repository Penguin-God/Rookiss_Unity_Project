using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UI_EventHandler : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public event Action<PointerEventData> OnClickHandler = null;
    public event Action<PointerEventData> OnDragHandler = null;

    // 클릭 시 자동 실행되는 Uinty Interface 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke(eventData);
    }

    // 드래그 시 자동 실행되는 Uinty Interface 함수
    public void OnDrag(PointerEventData eventData)
    {
        OnDragHandler?.Invoke(eventData);
    }
}
