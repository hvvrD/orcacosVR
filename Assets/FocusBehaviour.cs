using MoreMountains.Feedbacks;
using UnityEngine;

/// <summary>
/// http://feel-docs.moremountains.com/screen-shakes.html#how-to-setup-a-regular-non-cinemachine-camera-shake
/// </summary>
[RequireComponent(typeof(BarLogic))]
public class FocusBehaviour : MonoBehaviour
{
    [SerializeField] private MMF_Player mmfPlayer;
    private BarLogic bar;

    private void Start()
    {
        bar = GetComponent<BarLogic>();
        bar.OnCharged += SetFocus;

        mmfPlayer.Initialization();
    }

    private void OnDestroy()
    {
        bar.OnCharged -= SetFocus;
    }

    private void SetFocus()
    {
        Debug.Log("Run focus");
        mmfPlayer.PlayFeedbacks();
    }
}