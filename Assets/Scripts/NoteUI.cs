using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NoteUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject notePanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    [Header("Eventos")]
    public UnityEvent onNoteClosed;

    public void OpenNote(string title, string content)
    {
        if (notePanel == null) return;

        if (titleText != null) titleText.text = title;
        if (contentText != null) contentText.text = content;

        notePanel.SetActive(true);
    }

    // Vincula este método al botón de "Cerrar" o "Salir" de tu UI en el Canvas
    public void CloseNote()
    {
        if (notePanel == null || !notePanel.activeSelf) return;

        notePanel.SetActive(false);
        onNoteClosed?.Invoke();
    }
}