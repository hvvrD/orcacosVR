using System;
using System.Collections;
using TMPro;
using UnityEngine;
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
    [SerializeField] private Image contentIconActive;
    [SerializeField] private TextMeshProUGUI[] contentText;
    [SerializeField] private AnimationCurve curve;

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

        //Init setup
        IsReady = false;
        chargeValue = chargeMaxValue;
        contentIcon.sprite = inactiveIcon;
        contentIconActive.sprite = activeIcon;
        StartCoroutine(IESetCharge(inactiveIcon, chargeAddRate, -chargeAddValue,
            () => chargeValue >= 0, true));
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
            StartCoroutine(IESetCharge(activeIcon, chargeAddRate, chargeAddValue,
                () => chargeValue <= chargeMaxValue));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(IESetCharge(inactiveIcon, chargeAddRate, -chargeAddValue,
                () => chargeValue >= 0));
        }
    }

    private void SetActive(ControlMode mode)
    {
        isActive = mode == barMode;
    }

    private IEnumerator IESetCharge(Sprite icon, float rate, int add, Func<bool> condition,
        bool isStart = false)
    {
        WaitForSeconds wfs = new WaitForSeconds(rate);

        do
        {
            yield return wfs;
            SetBar(chargeValue);
            chargeValue += add;
        }
        while (condition());

        if (isStart)
            IsReady = true;
    }

    private void SetBar(int value)
    {
        chargeValue = value;
        contentBar.fillAmount = chargeValue / (float)chargeMaxValue;
        contentIconActive.color = new Color(1, 1, 1, contentBar.fillAmount);
    }
}