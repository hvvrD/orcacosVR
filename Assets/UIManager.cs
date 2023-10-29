using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject loading;
    [SerializeField] private BarLogic[] bars;

    public void SetLoading(bool isLoading)
    {
        loading.SetActive(isLoading);
    }

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