using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    GameObject canvas;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard kb = Keyboard.current;

        if (kb.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }

        if (kb.tabKey.wasPressedThisFrame)
        {
            if (canvas.activeSelf)
            {
                canvas.SetActive(false);
            }
            else
            {
                canvas.SetActive(true);
            }
        }

        if (kb.spaceKey.wasPressedThisFrame)
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

    }
}
