using UnityEngine;
using UnityEngine.Audio; // Requerido para el Bloque F (Audio Mixer)
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

    [Header("--- CONFIGURACIÓN DE MOVIMIENTO Y STAMINA ---")]
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
    // 2. VARIABLES DE AUDIO INTERACTIVO (Agregado para el Bloque F)
    // =========================================================================
    [Header("--- CONFIGURACIÓN DE AUDIO INTERACTIVO (Bloque F) ---")]
    [SerializeField] private AudioMixer gameAudioMixer; // Controla parámetros globales del Mixer
    [SerializeField] private AudioSource footstepsAudioSource; // Controla el sonido de pasos
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.3f;

    // --- INICIALIZACIÓN ---
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponentInChildren<Animator>();
    }

    // --- LÓGICA PRINCIPAL (Actualización por frame) ---
    void Update()
    {
        // -----------------------------------------------------------------
        // BLOQUE DE INPUTS Y STAMINA 
        // -----------------------------------------------------------------
        inputWalk = playerInput.actions["Move"].ReadValue<Vector2>();
        inputSprint = playerInput.actions["Sprint"].ReadValue<float>();

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

        // -----------------------------------------------------------------
        // BLOQUE DEL ANIMATOR (Blend Tree 2D)
        // -----------------------------------------------------------------
        if (anim != null)
        {
            anim.SetFloat("MovX", inputWalk.x);
            anim.SetFloat("MovZ", inputWalk.y);

            float animationSpeed = inputWalk.magnitude * (currentSpeed / walkSpeed);
            anim.SetFloat("Speed", animationSpeed);
        }

        // -----------------------------------------------------------------
        // BLOQUE F: AUDIO INTERACTIVO (Llamada al método externo de audio)
        // Editar este método si se precisa ajustar o cambiar el sonido ante 
        // ante las acciones del jugador.
        // -----------------------------------------------------------------
        GestionarAudioInteractivo();
    }

    // =========================================================================
    // MÉTODO EXCLUSIVO DEL BLOQUE F (Audio Interactivo)
    // Controlar los dos parámetros exigidos:
    // 1. El Pitch del AudioSource según la velocidad de desplazamiento.
    // 2. Un parámetro dinámico dentro del AudioMixer.
    // =========================================================================
    void GestionarAudioInteractivo()
    {
        if (footstepsAudioSource != null)
        {
            // Parámetro 1: Modifica la velocidad/tono de los pasos si corre o camina
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
            // Parámetro 2: Modifica el volumen o un efecto del Mixer según el movimiento
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
}