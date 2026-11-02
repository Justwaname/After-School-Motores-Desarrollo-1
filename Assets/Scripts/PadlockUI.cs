using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PadlockUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private int maxDigits = 4;

    public UnityEvent onAccessGranted;
    public UnityEvent onAccessDenied;

    private string currentInput = "";

    void Start()
    {
        ClearDisplay();
    }

    public void PressNumber(string number)
    {
        if (currentInput.Length < maxDigits)
        {
            currentInput += number;
            displayText.text = currentInput;
        }
    }

    public void ClearDisplay()
    {
        currentInput = "";
        displayText.text = "----";
    }

    public void SubmitCode()
    {
        if (currentInput.Length < maxDigits) return;

        bool isCorrect = Padlock.Instance.VerifyCode(currentInput);

        if (isCorrect)
        {
            displayText.text = "OPEN";
            onAccessGranted?.Invoke();
        }
        else
        {
            displayText.text = "ERROR";
            onAccessDenied?.Invoke();
            Invoke("ClearDisplay", 1f);  // Limpia la pantalla
        }
    }
}
