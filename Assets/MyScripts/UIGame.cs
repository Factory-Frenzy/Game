using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _UIGameStartCountdown;
    [SerializeField]
    private TextMeshProUGUI _UIChrono;
    private void Start()
    {
        GameManager.Instance.GameStartCountdown.OnValueChanged += UIGameStartCountdownUpdate;
        GameManager.Instance.TimeLeft.OnValueChanged += UIChronoUpdate;
    }

    private void UIChronoUpdate(int previousValue, int newValue)
    {
        _UIChrono.text = newValue.ToString();
    }

    private void UIGameStartCountdownUpdate(int previousValue, int newValue)
    {
        _UIGameStartCountdown.text = newValue.ToString();
        if (newValue == 0)
        {
            _UIGameStartCountdown.text = "GOOOOO !!!!";
            StartCoroutine(ClearDisplay(_UIGameStartCountdown));
        }
    }
    private IEnumerator ClearDisplay(TextMeshProUGUI display)
    {
        yield return new WaitForSeconds(2);
        display.text = string.Empty;
    }
}
