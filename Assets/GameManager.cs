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
/// future TODO:
/// - (VR)Calibration
/// - (VR)Detection for focus - idea (squint eyes)
/// - (VR)Detection for calm - idea (relax eyes)
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private UIManager uiManager;
    [ShowOnly][SerializeField] private bool userInterface;
    [ShowOnly][SerializeField] private bool toggle;
    [ShowOnly][SerializeField] private bool charge;
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";
    private const string Jump = "Jump";
    private const string MouseX = "Mouse X";
    private const string MouseY = "Mouse Y";

    public UnityAction<ControlMode> OnModeSwitch;
    public UnityAction<bool> OnCharge;

    public UnityAction<KeyCode> OnMove;
    public UnityAction<Vector3> OnMoveAxis;
    public UnityAction OnJump;
    public UnityAction<float, float> OnLookAxis;

    private void Start()
    {
        //Default auto start as Focus mode
        toggle = true;
        OnModeSwitch?.Invoke(ControlMode.Focus);

        //User interface ready protocol
        uiManager.SetLoading(!(userInterface = false));
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
        else if (Input.GetMouseButtonUp(0) && charge)
        {
            OnCharge?.Invoke(charge = false);
        }

        // Right click to toggle modes
        if (Input.GetMouseButtonDown(1) && !charge)
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

        //WASD - Move
        //if (Input.GetKey(KeyCode.W))
        //    OnMove?.Invoke(KeyCode.W);
        //if (Input.GetKey(KeyCode.A))
        //    OnMove?.Invoke(KeyCode.A);
        //if (Input.GetKey(KeyCode.S))
        //    OnMove?.Invoke(KeyCode.S);
        //if (Input.GetKey(KeyCode.D))
        //    OnMove?.Invoke(KeyCode.D);

        //GetAxis - Move
        Vector3 move = new Vector3(Input.GetAxis(Horizontal), 0, Input.GetAxis(Vertical));
        OnMoveAxis(move);

        //Jump
        if (Input.GetButtonDown(Jump))
            OnJump?.Invoke();

        //Look around
        OnLookAxis(Input.GetAxis(MouseX), Input.GetAxis(MouseY));
    }

    private IEnumerator IEWaitUserInterfaceReady()
    {
        yield return new WaitUntil(() => uiManager.CheckUserInterfaceReady() == true);

        Debug.Log("User interface is ready");
        uiManager.SetLoading(!(userInterface = true));
    }
}
