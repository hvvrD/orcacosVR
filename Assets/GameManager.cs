using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum ControlMode
{
    Focus,
    Calm
}

/// <summary>
/// 
/// TODO:
/// - (VR)Calibration
/// - (VR)Detection for focus - idea (squint eyes)
/// - (VR)Detection for calm - idea (relax eyes)
/// 
/// - holiding function
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private UIManager uiManager;
    private bool userInterface;
    private bool toggle;
    private bool charge;

    public UnityAction<ControlMode> OnModeSwitch;
    public UnityAction<bool> OnCharge;

    private void Start()
    {
        //Default auto start as Focus mode
        toggle = true;
        OnModeSwitch?.Invoke(ControlMode.Focus);

        //User interface ready protocol
        userInterface = false;
        StartCoroutine(IEWaitUserInterfaceReady());
    }

    private void Update()
    {
        if (!userInterface)
            return;

        //Left click to charge
        if (Input.GetMouseButtonDown(0) && !charge)
        {
            OnCharge?.Invoke(charge = true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnCharge?.Invoke(charge = false);
        }

        // Right click to toggle modes
        if (Input.GetMouseButtonDown(1))
        {
            OnCharge?.Invoke(charge = false);

            if (Input.GetMouseButton(0))
                OnCharge?.Invoke(charge = true);


            toggle = !toggle;
            if (toggle)
            {
                OnModeSwitch?.Invoke(ControlMode.Focus);
            }
            else
            {
                OnModeSwitch?.Invoke(ControlMode.Calm);
            }
        }
    }

    private IEnumerator IEWaitUserInterfaceReady()
    {
        yield return new WaitUntil(() => uiManager.CheckUserInterfaceReady() == true);
        userInterface = true;
    }
}
