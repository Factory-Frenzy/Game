using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class UIGame : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _UIGameStartCountdown;
    [SerializeField]
    private TextMeshProUGUI _UIChrono;
    [SerializeField]
    private TextMeshProUGUI _UIWaitingEndGame;
    private void Start()
    {
        GameManager.Instance.GameStartCountdown.OnValueChanged += UIGameStartCountdownUpdate;
        GameManager.Instance.TimeLeft.OnValueChanged += UIChronoUpdate;
        GameObject.FindObjectOfType(typeof(PlatformEndScript)).GetComponent<PlatformEndScript>().EndGameForMyPlayer += _UIWaitingEndGameUpdate;
    }

    private void _UIWaitingEndGameUpdate(object sender, EventArgs e)
    {
        _UIWaitingEndGame.text = "Waiting EndGame ...";
    }

    private void UIChronoUpdate(int previousValue, int newValue)
    {
        if (newValue > 0)
            _UIChrono.text = newValue.ToString();
        else
            ClearDisplay(_UIChrono);
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
