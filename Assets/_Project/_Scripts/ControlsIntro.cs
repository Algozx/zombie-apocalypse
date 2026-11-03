using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsIntro : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float duration = 6f;

    private IEnumerator Start()
    {
        // Waits one frame so the InterfaceController has already set the time scale on its Start.
        yield return null;
        Time.timeScale = 0;

        float remaining = duration;
        while (remaining > 0 && !PlayerStarted())
        {
            countdownText.text = $"Starting in {Mathf.CeilToInt(remaining)}...";
            remaining -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (!pausePanel.activeSelf) Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    private bool PlayerStarted()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        bool pressedKey = keyboard != null && (keyboard.wKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame ||
                                               keyboard.sKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame ||
                                               keyboard.upArrowKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame ||
                                               keyboard.leftArrowKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame);
        bool clicked = mouse != null && mouse.leftButton.wasPressedThisFrame;
        return pressedKey || clicked;
    }
}
