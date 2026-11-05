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

        // CONGELAR ENEMIGO
        EnemyIA enemy = Object.FindFirstObjectByType<EnemyIA>();
        if (enemy != null)
        {
            enemy.PauseEnemy();
        }
    }

    public void CloseNote()
    {
        if (notePanel == null || !notePanel.activeSelf) return;

        notePanel.SetActive(false);

        // REANUDAR ENEMIGO
        EnemyIA enemy = Object.FindFirstObjectByType<EnemyIA>();
        if (enemy != null)
        {
            enemy.ResumeEnemy();
        }

        onNoteClosed?.Invoke();
    }
}