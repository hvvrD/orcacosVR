using MoreMountains.Feedbacks;
using UnityEngine;

/// <summary>
/// http://feel-docs.moremountains.com/screen-shakes.html#how-to-setup-a-camera-zoom-feedback
/// </summary>
[RequireComponent(typeof(BarLogic))]
public class CalmBehaviour : MonoBehaviour
{
    [SerializeField] private MMF_Player mmfPlayer;
    [SerializeField] private CharacterMove character;
    private BarLogic bar;

    private void Start()
    {
        bar = GetComponent<BarLogic>();
        mmfPlayer.Initialization();

        bar.OnCharged += SetCalm;
        bar.OnPurgatory += SetPanic;
    }

    private void OnDestroy()
    {
        bar.OnCharged -= SetCalm;
        bar.OnPurgatory -= SetPanic;
    }

    private void SetCalm()
    {
        mmfPlayer.PlayFeedbacks();
        character.IsLeviosa = true;
    }

    private void SetPanic()
    {
        mmfPlayer.StopFeedbacks();
        character.IsLeviosa = false;
    }
}