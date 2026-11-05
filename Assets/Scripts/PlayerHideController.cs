using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHideController : MonoBehaviour
{
    [Header("Transición de pantalla (Arrastra tu Panel/Imagen aquí)")]
    [SerializeField] private Image transitionImage;
    private CanvasGroup transitionCanvasGroup;

    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float blackScreenDuration = 1.5f;

    private LockerInteractable currentLocker;
    private Rigidbody rb;
    private PlayerController playerController;

    // Variable para almacenar las restricciones originales del Rigidbody del jugador
    private RigidbodyConstraints originalConstraints;

    public bool IsHidden { get; private set; }
    public bool IsTransitioning { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();

        // Guardamos las constraints originales que tiene el Rigidbody al arrancar la escena
        if (rb != null)
        {
            originalConstraints = rb.constraints;
        }

        if (transitionImage != null)
        {
            transitionCanvasGroup = transitionImage.GetComponent<CanvasGroup>();
            if (transitionCanvasGroup == null)
            {
                transitionCanvasGroup = transitionImage.gameObject.AddComponent<CanvasGroup>();
            }
        }

        if (transitionCanvasGroup != null)
        {
            transitionCanvasGroup.alpha = 0f;
            transitionCanvasGroup.interactable = false;
            transitionCanvasGroup.blocksRaycasts = false;
        }
    }

    // ENTRAR EN LA TAQUILLA
    public void TryEnterHide(LockerInteractable locker)
    {
        if (locker == null || IsHidden || IsTransitioning)
            return;

        if (locker.HidePoint == null || locker.ExitPoint == null)
        {
            Debug.LogError("La taquilla necesita HidePoint y ExitPoint.");
            return;
        }

        if (!locker.TryOccupy(this))
        {
            Debug.Log("La taquilla ya está ocupada.");
            return;
        }

        currentLocker = locker;

        StartCoroutine(EnterLockerRoutine());
    }

    private IEnumerator EnterLockerRoutine()
    {
        IsTransitioning = true;

        LockPlayer();

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.LockerClose,
                transform.position
            );
        }

        // Oscurecer pantalla
        yield return FadeTo(1f);

        // Mantener la pantalla negra
        yield return new WaitForSecondsRealtime(blackScreenDuration);

        // Colocar al jugador dentro de la taquilla
        MovePlayerTo(currentLocker.HidePoint);

        IsHidden = true;

        // Volver a mostrar la pantalla
        yield return FadeTo(0f);

        IsTransitioning = false;

        Debug.Log("Jugador escondido en la taquilla.");
    }

    // SALIR DE LA TAQUILLA
    public void TryExitHide()
    {
        if (!IsHidden || IsTransitioning || currentLocker == null)
            return;

        StartCoroutine(ExitLockerRoutine());
    }

    private IEnumerator ExitLockerRoutine()
    {
        IsTransitioning = true;

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.LockerOpen,
                transform.position
            );
        }

        // Oscurecer pantalla
        yield return FadeTo(1f);

        // Mantener la pantalla negra
        yield return new WaitForSecondsRealtime(blackScreenDuration);

        // Colocar al jugador fuera de la taquilla
        MovePlayerTo(currentLocker.ExitPoint);

        currentLocker.Release(this);
        currentLocker = null;

        IsHidden = false;

        // Recuperar movimiento
        UnlockPlayer();

        // Volver a mostrar la pantalla
        yield return FadeTo(0f);

        IsTransitioning = false;

        Debug.Log("Jugador salió de la taquilla.");
    }

    // BLOQUEAR AL JUGADOR
    private void LockPlayer()
    {
        if (playerController != null)
        {
            playerController.LockMovement();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Mientras está escondido congelamos completamente la posición para que no flote ni se desplace
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
    }

    // RECUPERAR EL MOVIMIENTO
    private void UnlockPlayer()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Al salir, restauramos exactamente las restricciones originales que tenía al iniciar el juego
            rb.constraints = originalConstraints;
        }

        if (playerController != null)
        {
            playerController.UnlockMovement();
        }
    }

    // CAMBIAR LA POSICIÓN DEL JUGADOR
    private void MovePlayerTo(Transform point)
    {
        if (point == null)
            return;

        transform.SetPositionAndRotation(point.position, point.rotation);

        if (rb != null)
        {
            rb.position = point.position;
            rb.rotation = point.rotation;
        }

        Physics.SyncTransforms();
    }

    // TRANSICIÓN DE PANTALLA
    private IEnumerator FadeTo(float targetAlpha)
    {
        if (transitionCanvasGroup == null)
            yield break;

        float startAlpha = transitionCanvasGroup.alpha;
        float elapsed = 0f;

        if (fadeDuration <= 0f)
        {
            transitionCanvasGroup.alpha = targetAlpha;
            yield break;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / fadeDuration);

            transitionCanvasGroup.alpha =
                Mathf.Lerp(startAlpha, targetAlpha, progress);

            yield return null;
        }

        transitionCanvasGroup.alpha = targetAlpha;
    }

    private void OnDestroy()
    {
        if (currentLocker != null)
        {
            currentLocker.Release(this);
        }
    }
}