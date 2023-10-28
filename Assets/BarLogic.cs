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
    [SerializeField] private Color barColour;

    [Header("Reference")]
    [SerializeField] private Image contentBar;
    [SerializeField] private Image contentIcon;
    [SerializeField] private Image contentIconActive;
    [SerializeField] private TextMeshProUGUI[] contentText;
    [ShowOnly][SerializeField] private bool isActive;
    [ShowOnly][SerializeField] private int chargeAddValue = 1;
    [ShowOnly][SerializeField] private int chargeMaxValue = 100;
    [ShowOnly][SerializeField] private float chargeAddRate = 0.01f;//normal
    [ShowOnly][SerializeField] private float chargeAddRateCurve = 0.02f;//curve
    [ShowOnly][SerializeField] private int chargeValue = 0;//normal 
    [ShowOnly][SerializeField] private float chargeValueCurve = 0;//curve

    [Header("Curve - smoothing settings")]
    [SerializeField] private bool isCurve;
    [SerializeField] private bool isYieldNull;
    [SerializeField] private AnimationCurve curve;
    float time = 0f;

    public bool IsReady { get; private set; }

    private void Start()
    {
        GameManager.Instance.OnModeSwitch += SetActive;
        GameManager.Instance.OnCharge += !isCurve ? SetCharge : SetChargeCurve;

        //Init setup
        IsReady = false;
        chargeValue = chargeMaxValue;
        contentBar.color = barColour;
        contentIcon.sprite = inactiveIcon;
        contentIconActive.sprite = activeIcon;
        Array.ForEach(contentText, text => text.text = barMode.ToString());
        
        if (!isCurve)
            StartCoroutine(IESetCharge(chargeAddRate, -chargeAddValue, () => chargeValue >= 0, true));
        else
            StartCoroutine(IESetChargeCurve(chargeAddRateCurve, -1, () => chargeValueCurve > 0, true));
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnModeSwitch -= SetActive;
        GameManager.Instance.OnCharge -= !isCurve ? SetCharge: SetChargeCurve;
    }

    private void SetCharge(bool charge)
    {
        if (charge && isActive)
        {
            Debug.Log($"{name}, charge");
            StopAllCoroutines();
            StartCoroutine(IESetCharge(chargeAddRate, chargeAddValue, () => chargeValue <= chargeMaxValue));
        }
        else
        {
            Debug.Log($"{name}, de-charge");
            StopAllCoroutines();
            StartCoroutine(IESetCharge(chargeAddRate, -chargeAddValue, () => chargeValue >= 0));
        }
    }

    private void SetChargeCurve(bool charge)
    {
        if (charge && isActive)
        {
            Debug.Log($"{name}, charge");
            StopAllCoroutines();
            StartCoroutine(IESetChargeCurve(chargeAddRateCurve, 1, () => chargeValueCurve < chargeMaxValue));
        }
        else //if (chargeValueF > 0)
        {
            Debug.Log($"{name}, de-charge");
            StopAllCoroutines();
            StartCoroutine(IESetChargeCurve(chargeAddRateCurve, -1, () => chargeValueCurve > 0));
        }
    }

    private void SetActive(ControlMode mode)
    {
        isActive = mode == barMode;
    }

    private IEnumerator IESetCharge(float rate, int add, Func<bool> condition, bool isStart = false)
    {
        WaitForSeconds wfs = new WaitForSeconds(rate);

        do
        {
            yield return wfs;

            contentBar.fillAmount = chargeValue / (float)chargeMaxValue;
            contentIconActive.color = new Color(1, 1, 1, contentBar.fillAmount);

            chargeValue += add;
        }
        while (condition());

        //Only for init purposes
        if (isStart)
            IsReady = true;
    }

    private IEnumerator IESetChargeCurve(float rate, int lerp, Func<bool> condition, bool isStart = false)
    {
        WaitForSeconds wfs = new WaitForSeconds(rate);
        float t;
        float curveT;
        time = 0;

        do
        {
            time += Time.deltaTime;

            t = Mathf.Clamp01(time / (float)chargeMaxValue);
            curveT = curve.Evaluate(time);

            chargeValueCurve = lerp == 1 ? Mathf.Lerp(chargeValueCurve, 1f, curveT) : Mathf.Lerp(chargeValueCurve, 0f, curveT);
            contentBar.fillAmount = chargeValueCurve;
            contentIconActive.color = new Color(1, 1, 1, chargeValueCurve);

            yield return isYieldNull ? null : wfs;
        }
        while (condition());

        //Only for init purposes
        if (isStart)
            IsReady = true;
    }
}