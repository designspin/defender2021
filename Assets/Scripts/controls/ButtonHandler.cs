using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed;
    public delegate void OnButtonDownHandler();
    public delegate void OnButtonUpHandler();

    public event OnButtonDownHandler OnButtonDown = delegate { };
    public event OnButtonUpHandler OnButtonUp = delegate { };
    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        OnButtonDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        OnButtonUp();
    }
}
