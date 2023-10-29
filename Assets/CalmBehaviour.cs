using MoreMountains.Feedbacks;
using UnityEngine;

/// <summary>
/// http://feel-docs.moremountains.com/screen-shakes.html#how-to-setup-a-regular-non-cinemachine-camera-shake
/// </summary>
[RequireComponent(typeof(BarLogic))]
public class CalmBehaviour : MonoBehaviour
{
    [SerializeField] private MMF_Player mmfPlayer;
    private BarLogic bar;

    private void Start()
    {
        bar = GetComponent<BarLogic>();
        bar.OnCharged += SetCalm;

        mmfPlayer.Initialization();
    }

    private void OnDestroy()
    {
        bar.OnCharged -= SetCalm;
    }

    private void SetCalm()
    {
        Debug.Log("Run calm");
        mmfPlayer.PlayFeedbacks();
    }
}