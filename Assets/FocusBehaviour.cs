using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// http://feel-docs.moremountains.com/screen-shakes.html#how-to-setup-a-regular-non-cinemachine-camera-shake
/// </summary>
[RequireComponent(typeof(BarLogic))]
public class FocusBehaviour : Singleton<FocusBehaviour>
{
    [SerializeField] private MMF_Player mmfPlayer;
    private BarLogic bar;

    public UnityAction OnFocusOpen;

    private void Start()
    {
        bar = GetComponent<BarLogic>();
        mmfPlayer.Initialization();

        bar.OnCharged += SetFocus;
        bar.OnPurgatory += SetUnfocus;
    }

    private void OnDestroy()
    {
        bar.OnCharged -= SetFocus;
    }

    private void SetFocus()
    {
        mmfPlayer.PlayFeedbacks();
        OnFocusOpen?.Invoke();
    }

    private void SetUnfocus()
    {
        mmfPlayer.StopFeedbacks();
    }
}