using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ItemData finalKeyData;

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

    public void ShowNotification(string message)
    {
        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.UINotification,
                transform.position
            );
        }

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

    public void ItemCollected()
    {
        itemsCollected++;

        Debug.Log(
            $"Items recogidos: {itemsCollected}/{itemsToEscape}"
        );

        UpdateInteractionText();
    }

    public bool CanEscape()
    {
        if (Inventory.Instance == null)
            return false;

        // Ahora exigimos tener tanto el ítem combinado como el Blowtorch original
        bool hasFinalItem = Inventory.Instance.HasItem(finalKeyData);
        bool hasBlowtorch = Inventory.Instance.HasItem(Inventory.Instance.GetBlowtorch());

        return hasFinalItem && hasBlowtorch;
    }

    private void UpdateInteractionText()
    {
        if (notificationText == null)
            return;

        if (CanEscape())
        {
            notificationText.text =
                "You have everything needed to open the door!";
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