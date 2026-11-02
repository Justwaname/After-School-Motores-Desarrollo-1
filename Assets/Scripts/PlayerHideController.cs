using System.Collections;
using UnityEngine;

public class PlayerHideController : MonoBehaviour
{
    [Header("Animación del jugador")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string enterHideTrigger = "EnterHide";
    [SerializeField] private string exitHideTrigger = "ExitHide";

    [Header("Duración de animaciones")]
    [SerializeField] private float enterAnimationDuration = 0.8f;
    [SerializeField] private float exitAnimationDuration = 0.8f;
    [SerializeField] private float doorOpenDelay = 0.25f;

    public bool IsHidden { get; private set; }
    public bool IsTransitioning { get; private set; }

    private PlayerController playerController;
    private Rigidbody rb;

    private LockerInteractable currentLocker;
    private Transform hidePoint;
    private Transform exitPoint;

    private bool previousKinematic;
    private bool previousControllerEnabled;

    private Collider[] playerColliders;
    private bool[] originalColliderStates;

    private Renderer[] playerRenderers;
    private bool[] originalRendererStates;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInChildren<Animator>();
        }

        playerColliders = GetComponentsInChildren<Collider>(true);
        originalColliderStates = new bool[playerColliders.Length];

        for (int i = 0; i < playerColliders.Length; i++)
        {
            originalColliderStates[i] = playerColliders[i].enabled;
        }

        playerRenderers = GetComponentsInChildren<Renderer>(true);
        originalRendererStates = new bool[playerRenderers.Length];

        for (int i = 0; i < playerRenderers.Length; i++)
        {
            originalRendererStates[i] = playerRenderers[i].enabled;
        }
    }

    public bool TryEnterHide(LockerInteractable locker)
    {
        if (IsHidden || IsTransitioning || locker == null)
            return false;

        if (locker.HidePoint == null || locker.ExitPoint == null)
        {
            Debug.LogError(
                "Asigna HidePoint y ExitPoint en la taquilla."
            );
            return false;
        }

        if (!locker.TryOccupy(this))
        {
            Debug.Log("Esta taquilla está ocupada.");
            return false;
        }

        currentLocker = locker;
        hidePoint = locker.HidePoint;
        exitPoint = locker.ExitPoint;

        IsHidden = true;
        IsTransitioning = true;

        if (playerController != null)
        {
            previousControllerEnabled = playerController.enabled;
            playerController.enabled = false;
        }

        if (rb != null)
        {
            previousKinematic = rb.isKinematic;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        SetAnimatorTrigger(playerAnimator, enterHideTrigger);
        currentLocker.PlayOpenAnimation();

        StartCoroutine(EnterHideRoutine());

        return true;
    }

    private IEnumerator EnterHideRoutine()
    {
        yield return new WaitForSeconds(
            Mathf.Max(0f, enterAnimationDuration)
        );

        if (hidePoint != null)
        {
            MovePlayerTo(hidePoint);
        }

        SetPlayerColliders(false);
        SetPlayerRenderers(false);

        if (currentLocker != null)
        {
            currentLocker.PlayCloseAnimation();
        }

        IsTransitioning = false;
    }

    public void TryExitHide()
    {
        if (!IsHidden || IsTransitioning)
            return;

        IsTransitioning = true;
        StartCoroutine(ExitHideRoutine());
    }

    private IEnumerator ExitHideRoutine()
    {
        if (currentLocker != null)
        {
            currentLocker.PlayOpenAnimation();
        }

        yield return new WaitForSeconds(
            Mathf.Max(0f, doorOpenDelay)
        );

        // Mostrar al jugador para reproducir la animación de salida.
        SetPlayerRenderers(true);
        SetAnimatorTrigger(playerAnimator, exitHideTrigger);

        yield return new WaitForSeconds(
            Mathf.Max(0f, exitAnimationDuration)
        );

        if (exitPoint != null)
        {
            MovePlayerTo(exitPoint);
        }

        SetPlayerColliders(true);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = previousKinematic;
        }

        if (playerController != null)
        {
            playerController.enabled = previousControllerEnabled;
        }

        if (currentLocker != null)
        {
            currentLocker.PlayCloseAnimation();
            currentLocker.Release(this);
        }

        currentLocker = null;
        hidePoint = null;
        exitPoint = null;

        IsHidden = false;
        IsTransitioning = false;
    }

    private void MovePlayerTo(Transform destination)
    {
        if (rb != null)
        {
            rb.position = destination.position;
            rb.rotation = destination.rotation;
        }
        else
        {
            transform.SetPositionAndRotation(
                destination.position,
                destination.rotation
            );
        }
    }

    private void SetPlayerColliders(bool enabledState)
    {
        for (int i = 0; i < playerColliders.Length; i++)
        {
            if (playerColliders[i] != null)
            {
                playerColliders[i].enabled = enabledState
                    ? originalColliderStates[i]
                    : false;
            }
        }
    }

    private void SetPlayerRenderers(bool visible)
    {
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] != null)
            {
                playerRenderers[i].enabled = visible
                    ? originalRendererStates[i]
                    : false;
            }
        }
    }

    private void SetAnimatorTrigger(
        Animator animator,
        string triggerName)
    {
        if (animator == null || string.IsNullOrEmpty(triggerName))
            return;

        foreach (AnimatorControllerParameter parameter
                 in animator.parameters)
        {
            if (parameter.name == triggerName &&
                parameter.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(triggerName);
                animator.SetTrigger(triggerName);
                return;
            }
        }

        Debug.LogWarning(
            "No existe el Trigger '" + triggerName +
            "' en el Animator."
        );
    }
}