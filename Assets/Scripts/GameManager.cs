using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    // --- SINGLETON ---
    public static GameManager Instance { get; private set; }

    [Header("Items")]
    [SerializeField] private int itemsToEscape = 3;
    [SerializeField] private int itemsCollected;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float textDisplayDuration = 3f;

    private Coroutine hideTextCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    // --- NOTIFICACIONES ---
    public void ShowNotification(string message)
    {
        if (notificationText == null)
            return;

        notificationText.text = message;

        if (hideTextCoroutine != null)
        {
            StopCoroutine(hideTextCoroutine);
        }

        notificationText.gameObject.SetActive(true);

        hideTextCoroutine =
            StartCoroutine(
                HideTextAfterDelay(textDisplayDuration)
            );
    }

    // --- OBJETO RECOGIDO ---
    public void ItemCollected()
    {
        itemsCollected++;

        Debug.Log(
            $"Items recogidos: {itemsCollected}/{itemsToEscape}"
        );

        UpdateInteractionText();
    }

    // --- ¿PUEDE ESCAPAR? ---
    public bool CanEscape()
    {
        if (Inventory.Instance == null)
            return false;

        return Inventory.Instance.HasItem("LlaveFinal");
    }

    // --- ACTUALIZAR UI ---
    private void UpdateInteractionText()
    {
        if (notificationText == null)
            return;

        if (CanEscape())
        {
            notificationText.text =
                "You created the final key! You can escape!";
        }
        else
        {
            notificationText.text =
                $"Items: {itemsCollected}/{itemsToEscape}";
        }

        if (hideTextCoroutine != null)
        {
            StopCoroutine(hideTextCoroutine);
        }

        notificationText.gameObject.SetActive(true);

        hideTextCoroutine =
            StartCoroutine(
                HideTextAfterDelay(textDisplayDuration)
            );
    }

    // --- OCULTAR TEXTO ---
    private IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }

        hideTextCoroutine = null;
    }
}