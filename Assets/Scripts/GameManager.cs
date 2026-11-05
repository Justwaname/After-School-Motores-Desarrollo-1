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

    [Header("Debug / Editor Cheats")]
    [Tooltip("Activa esto en el Inspector para simular que ya tienes todo listo para escapar.")]
    [SerializeField] private bool forceCanEscape = false;

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
        // Si el truco del editor está activado, devolvemos true directamente
        if (forceCanEscape)
            return true;

        if (Inventory.Instance == null)
            return false;

        // Exigimos tener tanto el ítem combinado como el Blowtorch original
        bool hasFinalItem = Inventory.Instance.HasItem(Inventory.Instance.GetFinalItem());
        bool hasBlowtorch = Inventory.Instance.HasItem(Inventory.Instance.GetBlowtorch());

        return hasFinalItem && hasBlowtorch;
    }

    public bool WillEscape()
    {
        if (forceCanEscape)
            return true;

        if (Inventory.Instance == null)
            return false;

        // Comprobamos si tiene cada uno de los ítems
        bool hasMagnesiumRibbon = Inventory.Instance.HasItem(Inventory.Instance.GetMagnesiumRibbon());
        bool hasThermite = Inventory.Instance.HasItem(Inventory.Instance.GetThermite());
        bool hasBlowtorch = Inventory.Instance.HasItem(Inventory.Instance.GetBlowtorch());

        return hasMagnesiumRibbon && hasThermite && hasBlowtorch;
    }

    private void UpdateInteractionText()
    {
        if (notificationText == null)
            return;

        if (WillEscape() || forceCanEscape)
        {
            notificationText.text =
                "¡Tienes todo para escapar!";
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

    // --- OPCIONAL: ACCIÓN RÁPIDA DESDE EL INSPECTOR ---
    [ContextMenu("Forzar Escape Disponible")]
    private void DebugForceEscape()
    {
        forceCanEscape = !forceCanEscape;
        Debug.Log($"[GameManager] Force Can Escape: {forceCanEscape}");
    }
}