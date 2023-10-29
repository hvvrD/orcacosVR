using System;
using System.Collections;
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
    [SerializeField] private Color barColour;

    [Header("Reference")]
    [SerializeField] private Image contentBar;
    [SerializeField] private Image contentIcon;
    [SerializeField] private Image contentIconActive;
    [SerializeField] private TextMeshProUGUI[] contentText;
    //Should be private
    [SerializeField] private bool isActive;
    [SerializeField] private int chargeAddValue = 1;
    [SerializeField] private int chargeMaxValue = 100;//100;//normal
    [SerializeField] private int chargeMinValue = 1;//normal
    [SerializeField] private float chargeAddRate = 0.01f;//normal
    [SerializeField] private int chargeValue = 0;//normal 
    [SerializeField] private float chargeMaxValueCurve = 99f;//100f;//curve
    [SerializeField] private float chargeMinValueCurve = 1f;//100f;//curve
    [SerializeField] private float chargeAddRateCurve = 0.02f;//curve
    [SerializeField] private float chargeValueCurve = 0;//curve

    [Header("Curve - smoothing settings")]
    [SerializeField] private bool isCurve;
    [SerializeField] private AnimationCurve curve;
    [ShowOnly][SerializeField] float time = 0f;
    [ShowOnly][SerializeField] bool decharging;

    private bool isCharged => chargeValue >= chargeMaxValue;
    private bool isDepleted => chargeValue <= chargeMinValue;
    private bool isChargedCurve => Mathf.Approximately(chargeValueCurve, chargeMaxValueCurve);
    private bool isDepletedCurve => Mathf.Approximately(chargeValueCurve, chargeMinValueCurve);

    public bool IsReady { get; private set; }
    public UnityAction OnCharged;
    public UnityAction OnDepleted;

    private void Start()
    {
        GameManager.Instance.OnModeSwitch += SetActive;
        GameManager.Instance.OnCharge += SetCharge;

        //Init setup
        IsReady = false;
        chargeValue = chargeMaxValue;
        chargeValueCurve = chargeMaxValueCurve;
        contentIcon.sprite = inactiveIcon;
        contentIconActive.sprite = activeIcon;
        contentBar.color = barColour;
        Array.ForEach(contentText, text => text.text = barMode.ToString());

        if (!isCurve)
            StartCoroutine(IESetCharge(chargeAddRate, -chargeAddValue, () => chargeValue >= chargeMinValue, true));
        else
            StartCoroutine(IESetChargeCurve(chargeAddRateCurve, -1, () => chargeValueCurve > chargeMinValueCurve, true));
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
            //Debug.Log($"{name}, charge");
            StopAllCoroutines();

            if (!isCurve)
                StartCoroutine(IESetCharge(chargeAddRate, chargeAddValue, () => chargeValue <= chargeMaxValue));
            else
                StartCoroutine(IESetChargeCurve(chargeAddRateCurve, 1, () => chargeValueCurve < chargeMaxValueCurve));

            decharging = true;
        }
        else if (decharging)
        {
            //Debug.Log($"{name}, de-charge");
            StopAllCoroutines();

            if (!isCurve)
                StartCoroutine(IESetCharge(chargeAddRate, -chargeAddValue, () => chargeValue >= chargeMinValue));
            else
                StartCoroutine(IESetChargeCurve(chargeAddRateCurve, -1, () => chargeValueCurve > chargeMinValueCurve));

            decharging = false;
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

            if (isCharged || isDepleted)
            {
                EndConditionCheck(isStart);
                break;
            }
        }
        while (condition());
        yield break;
    }

    private IEnumerator IESetChargeCurve(float rate, int lerp, Func<bool> condition, bool isStart = false)
    {
        float t;
        float curveT;
        time = 0;

        do
        {
            time += Time.deltaTime;

            t = Mathf.Clamp01(time / chargeMaxValue);
            curveT = curve.Evaluate(t);

            chargeValueCurve = lerp == 1 ?
                Mathf.Lerp(chargeValueCurve, chargeMaxValueCurve, curveT)
                : Mathf.Lerp(chargeValueCurve, 0f, curveT);
            contentBar.fillAmount = chargeValueCurve / chargeMaxValueCurve;
            contentIconActive.color = new Color(1, 1, 1, contentBar.fillAmount);

            if (isChargedCurve || isDepletedCurve)
            {
                EndConditionCheck(isStart);
                break;
            }

            yield return null;
        }
        while (condition());
        yield break;
    }

    private void EndConditionCheck(bool isStart)
    {
        if (!isCurve ? isCharged : isChargedCurve)
        {
            //Debug.Log($"Do while is charged, {chargeValueCurve}");
            if (!isCurve)
                chargeValue = chargeMaxValue;
            else
                chargeValueCurve = chargeMaxValueCurve;

            OnCharged?.Invoke();
        }
        else if (!isCurve ? isDepleted : isDepletedCurve)
        {
            //Debug.Log($"Do while is depleted, {chargeValueCurve}");
            if (!isCurve)
                chargeValue = 0;
            else
                chargeValueCurve = 0f;

            if (isStart)
                IsReady = true;
            else
                OnDepleted?.Invoke();
        }
    }
}