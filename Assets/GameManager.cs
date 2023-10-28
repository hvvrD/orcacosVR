using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// - Calibration
/// - Detection for focus - idea (squint eyes)
/// - Detection for calm - idea (relax eyes)
/// </summary>

public enum ControlMode
{
    Focus,
    Calm
}

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
        //do
        //{
        //    if (uiManager.CheckUserInterfaceReady())
        //        break;
            
        //    yield return null;

        //}while (true);
        yield return new WaitUntil(() => uiManager.CheckUserInterfaceReady() == true);
        userInterface = true;
    }
}
