using System;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BarLogic : MonoBehaviour
{
    [Header("Mode - require inspector setup")]
    [SerializeField] private ControlMode barMode;

    [Header("Asset")]
    [SerializeField] private Sprite inactiveIcon;
    [SerializeField] private Sprite activeIcon;

    [Header("Reference")]
    [SerializeField] private Image contentBar;
    [SerializeField] private Image contentIcon;
    [SerializeField] private TextMeshProUGUI[] contentText;

    private bool isActive;
    private float chargeAddRate = 0.01f;
    private int chargeAddValue = 1;
    private int chargeValue = 0;
    private int chargeMaxValue = 100;
    public bool IsReady { get; private set; }

    private void Start()
    {
        GameManager.Instance.OnModeSwitch += SetActive;
        GameManager.Instance.OnCharge += SetCharge;

        IsReady = false;
        chargeValue = chargeMaxValue;
        StartCoroutine(IEDecharge(true));
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnModeSwitch -= SetActive;
        GameManager.Instance.OnCharge -= SetCharge;
    }

    private void SetCharge(bool charge)
    {
        if (charge && isActive)
        {
            StopAllCoroutines();
            StartCoroutine(IESetCharge());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(IEDecharge());
        }
    }

    private void SetActive(ControlMode mode)
    {
        isActive = mode == barMode;
    }

    private IEnumerator IESetCharge()
    {
        //Debug.Log($"{this} - Spirit bomb de-charge");
        WaitForSeconds wfs = new WaitForSeconds(chargeAddRate);
        do
        {
            yield return wfs;
            SetBar(chargeValue);
            chargeValue += chargeAddValue;
        }
        while (chargeValue <= chargeMaxValue);
    }

    private IEnumerator IEDecharge(bool isStart = false)
    {
        //Debug.Log($"{this} - Spirit bomb charge");
        WaitForSeconds wfs = new WaitForSeconds(chargeAddRate);
        do
        {
            yield return wfs;
            SetBar(chargeValue);
            chargeValue -= chargeAddValue;
        }
        while (chargeValue >= 0);

        if (isStart)
            IsReady = true;
    }

    private void SetBar(int value)
    {
        chargeValue = value;
        contentBar.fillAmount = chargeValue / (float)chargeMaxValue;
        //Array.ForEach(contentText, x => x.text = chargeValue.ToString());
    }
}