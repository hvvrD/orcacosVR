using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private BarLogic[] bars;

    public bool CheckUserInterfaceReady()
    {
        if (bars.Length <= 0)
        {
            Debug.LogError($"{name}: No bars set");
            return false;
        }

        foreach (var bar in bars)
            if (!bar.IsReady)
                return false;

        return true;
    }
}
