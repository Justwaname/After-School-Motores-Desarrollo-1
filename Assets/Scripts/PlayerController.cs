using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // =========================================================================
    // 1. VARIABLES PRINCIPALES (Movimiento, Inputs y Animaciones)
    // =========================================================================
    private Rigidbody rb;
    private PlayerInput playerInput;
    private Animator anim;
    private Vector2 inputWalk;
    private float inputSprint;

    private float inputInventory;
    [SerializeField] private GameObject InvPanel;

    [Header("--- MOVIMIENTO Y STAMINA ---")]
    [SerializeField] float currentSpeed;
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float runSpeed = 12f;
    [SerializeField] float maxStamina = 7f;
    [SerializeField] float stamina = 7f;
    [SerializeField] float staminaDrain = 2f;
    [SerializeField] float staminaRecovery = 1.5f;
    [SerializeField] float timeStoppedRunning = 0f;
    [SerializeField] float recoveryDelay = 0.7f;
    [SerializeField] private float rotationSpeed = 15f;
    private bool staminaDepleted = false;

    // =========================================================================
    // 2. VARIABLES DE AUDIO INTERACTIVO (Bloque F)
    // =========================================================================
    [Header("--- CONFIGURACIÓN DE AUDIO INTERACTIVO (Bloque F) ---")]
    [SerializeField] private AudioMixer gameAudioMixer;
    [SerializeField] private AudioSource footstepsAudioSource;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.3f;

    // --- INICIALIZACIÓN ---
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponentInChildren<Animator>();
    }

    // --- LÓGICA PRINCIPAL ---
    void Update()
    {
        inputWalk = playerInput.actions["Move"].ReadValue<Vector2>();
        inputSprint = playerInput.actions["Sprint"].ReadValue<float>();
        inputInventory = playerInput.actions["Inventory"].ReadValue<float>();

        // --- ABRIR/CERRAR INVENTARIO ---
        if (playerInput != null && playerInput.actions["Inventory"].WasPressedThisFrame())
        {
            AltInventory();
        }

        // --- ACCIONES DE TECLADO (Q, E, J, K) CON LOS NOMBRES EXACTOS DEL ANIMATOR ---
        if (Keyboard.current != null && anim != null)
        {
            // Q: Abrir puerta
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                anim.SetTrigger("TriggerOpening");
            }

            // E: Tocar el botón
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                anim.SetTrigger("TriggerInteract");
            }

            // J: Escalar
            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                anim.SetTrigger("TriggerBracedHangToCrouch");
            }

            // K: Bajar
            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                anim.SetTrigger("TriggerClimbingDown");
            }
        }

        bool wantsToSprint = inputSprint > 0;

        if (stamina <= 0)
        {
            stamina = 0;
            staminaDepleted = true;
        }

        if (wantsToSprint && !staminaDepleted)
        {
            currentSpeed = runSpeed;
            stamina -= staminaDrain * Time.deltaTime;
            stamina = Mathf.Max(stamina, 0f);
            timeStoppedRunning = 0f;
        }
        else
        {
            currentSpeed = walkSpeed;
            timeStoppedRunning = Mathf.Min(
                timeStoppedRunning + Time.deltaTime,
                recoveryDelay
            );
        }

        if (timeStoppedRunning >= recoveryDelay)
        {
            stamina += staminaRecovery * Time.deltaTime;
            stamina = Mathf.Min(stamina, maxStamina);
        }

        if (stamina >= maxStamina)
        {
            stamina = maxStamina;
            staminaDepleted = false;
        }

        // --- ANIMATOR (Blend Tree 2D) ---
        if (anim != null)
        {
            anim.SetFloat("MovX", inputWalk.x);
            anim.SetFloat("MovZ", inputWalk.y);

            float animationSpeed = inputWalk.magnitude * (currentSpeed / walkSpeed);
            anim.SetFloat("Speed", animationSpeed);
        }

        // --- AUDIO INTERACTIVO (Bloque F) ---
        GestionarAudioInteractivo();
    }

    // =========================================================================
    // MÉTODO EXCLUSIVO DEL BLOQUE F (Audio Interactivo)
    // =========================================================================
    void GestionarAudioInteractivo()
    {
        if (footstepsAudioSource != null)
        {
            if (inputWalk.sqrMagnitude > 0.01f)
            {
                footstepsAudioSource.pitch = (currentSpeed > walkSpeed) ? maxPitch : minPitch;
                if (!footstepsAudioSource.isPlaying) footstepsAudioSource.Play();
            }
            else
            {
                if (footstepsAudioSource.isPlaying) footstepsAudioSource.Stop();
            }
        }

        if (gameAudioMixer != null)
        {
            float targetVolume = (inputWalk.sqrMagnitude > 0.01f) ? 0f : -10f;
            gameAudioMixer.SetFloat("MasterVolume", targetVolume);
        }
    }

    // =========================================================================
    // 3. FÍSICAS Y MOVIMIENTO (Rigidbody y Rotación Suave)
    // =========================================================================
    private void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(inputWalk.x, 0f, inputWalk.y).normalized;

        rb.linearVelocity = new Vector3(
          inputWalk.normalized.x * currentSpeed,
          rb.linearVelocity.y,
          inputWalk.normalized.y * currentSpeed
        );

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    // --- METODO P/ ABRIR/CERRAR INVENTARIO (Con protección contra nulos) ---
    private void AltInventory()
    {
        if (InvPanel != null)
        {
            InvPanel.SetActive(!InvPanel.activeSelf);
        }
    }
}