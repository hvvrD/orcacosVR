using MoreMountains.Feedbacks;
using UnityEngine;

public class DoorLogic : MonoBehaviour
{
    private MMF_Player mmfPlayer;
    private bool player;

    private void Start()
    {
        mmfPlayer = GetComponent<MMF_Player>();
        FocusBehaviour.Instance.OnFocusOpen += SetOpen;
    }

    private void OnDestroy()
    {
        FocusBehaviour.Instance.OnFocusOpen -= SetOpen;
    }

    private void OnTriggerEnter(Collider other)
    {
        player = true;
    }

    private void OnTriggerExit(Collider other)
    {
        player = false;
    }

    private void SetOpen()
    {
        if (player)
            mmfPlayer.PlayFeedbacks();
    }
}
