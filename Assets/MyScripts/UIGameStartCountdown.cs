using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGameStartCountdown : MonoBehaviour
{
    private TextMeshProUGUI _UIGameStartCountdown;
    private void Start()
    {
        _UIGameStartCountdown = this.GetComponent<TextMeshProUGUI>();
        GameManager.Instance.GameStartCountdown.OnValueChanged += UIGameStartCountdownUpdate;
    }

    private void UIGameStartCountdownUpdate(int previousValue, int newValue)
    {
        _UIGameStartCountdown.text = newValue.ToString();
        if (newValue == 0)
        {
            _UIGameStartCountdown.text = "GOOOOO !!!!";
            StartCoroutine(ClearDisplay());
        }
    }
    private IEnumerator ClearDisplay()
    {
        yield return new WaitForSeconds(2);
        _UIGameStartCountdown.text = string.Empty;
    }
}
