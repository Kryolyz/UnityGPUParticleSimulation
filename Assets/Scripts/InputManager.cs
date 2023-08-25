using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action OnLeftMouseDown;
    public Action OnLeftMouseUp;

    public Action OnMiddleMouseDown;
    public Action OnMiddleMouseUp;

    public Action OnRightMouseDown;
    public Action OnRightMouseUp;

    public Action<float> OnMouseWheel;

    [HideInInspector]
    public Vector2 mousePosition;

    private bool wasLeftClickDown = false;
    private bool wasRightClickDown = false;
    private bool wasMiddleClickDown = false;

    private void FixedUpdate()
    {
        mousePosition = Input.mousePosition;
        float screenBorders = (Screen.width - Screen.height) / 2;
        mousePosition.x = Mathf.Clamp((mousePosition.x - screenBorders) / (Screen.width - screenBorders * 2), 0, 1.0f);
        mousePosition.y = (mousePosition.y) / (Screen.height);

        if (Input.GetMouseButton(0) && !wasLeftClickDown)
        {
            OnLeftMouseDown?.Invoke();
            wasLeftClickDown= true;
        } else if (!Input.GetMouseButton(0) && wasLeftClickDown)
        {
            OnLeftMouseUp?.Invoke();
            wasLeftClickDown = false;
        }

        if (Input.GetMouseButton(1) && !wasMiddleClickDown)
        {
            OnMiddleMouseDown?.Invoke();
            wasMiddleClickDown = true;
        }
        else if (!Input.GetMouseButton(1) && wasMiddleClickDown)
        {
            OnMiddleMouseUp?.Invoke();
            wasMiddleClickDown = false;
        }

        if (Input.GetMouseButton(2) && !wasRightClickDown)
        {
            OnMiddleMouseDown?.Invoke();
            wasRightClickDown = true;
        }
        else if (!Input.GetMouseButton(2) && wasMiddleClickDown)
        {
            OnRightMouseUp?.Invoke();
            wasRightClickDown = false;
        }

        if (Input.mouseScrollDelta.y != 0)
            OnMouseWheel?.Invoke(Input.mouseScrollDelta.y);
    }
}
