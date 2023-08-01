using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    GameObject sliders;
    [SerializeField]
    GameObject menus;

    void Awake()
    {
        //canvas = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard kb = Keyboard.current;       

        if (kb.escapeKey.wasPressedThisFrame)
        {
            if (menus.activeSelf)
            {
                menus.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                menus.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        if (kb.tabKey.wasPressedThisFrame)
        {
            if (sliders.activeSelf)
            {
                sliders.SetActive(false);
            }
            else
            {
                sliders.SetActive(true);
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
