using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildCanvas : MonoBehaviour
{
    [SerializeField] private Button buildButton;
    [SerializeField] private GameObject buildCanvas;


    public void SubsCribeToButtonPress(Action action)
    {
        buildButton.onClick.AddListener(() => action.Invoke());
    }

    public void Show()
    {
        buildCanvas.SetActive(true);
    }

    public void Hide()
    {
        buildCanvas.SetActive(false);
    }
}
