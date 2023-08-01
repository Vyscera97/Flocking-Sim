using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    [SerializeField]
    Button exitButton;
    [SerializeField]
    Button runButton;

    private void OnEnable()
    {
        exitButton.onClick.AddListener(() => { Application.Quit(); });
        runButton.onClick.AddListener(() => { Time.timeScale = 1; });
    }
}
