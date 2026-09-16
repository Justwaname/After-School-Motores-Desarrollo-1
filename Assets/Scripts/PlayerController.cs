using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    private Animator anim; // <-- Referencia al Animator
    private Vector2 inputWalk;
    private float inputSprint;

    [SerializeField] float currentSpeed;
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float runSpeed = 12f;
    [SerializeField] float maxStamina = 7f;
    [SerializeField] float stamina = 7f;
    [SerializeField] float staminaDrain = 2f;
    [SerializeField] float staminaRecovery = 1.5f;
    [SerializeField] float timeStoppedRunning = 0f;
    [SerializeField] float recoveryDelay = 0.7f;
    [SerializeField] private float rotationSpeed = 15f; // Rotación suave
    private bool staminaDepleted = false;

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

        bool wantsToSprint = inputSprint > 0;

        // --- CONTROL DE STAMINA ---
        if (stamina <= 0)
        {
            stamina = 0;
            staminaDepleted = true;
        }

        // --- CORRER ---
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

        // --- RECUPERACIÓN DE STAMINA ---
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

        // --- LÓGICA DEL ANIMATOR ---
        if (anim != null)
        {
            float animationSpeed = inputWalk.magnitude * (currentSpeed / walkSpeed);
            anim.SetFloat("Speed", animationSpeed);
        }
    }

    // --- MOVIMIENTO Y ROTACIÓN ---
    private void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(inputWalk.x, 0f, inputWalk.y).normalized;
        Vector3 targetVelocity = moveDirection * (inputWalk.magnitude > 0.01f ? currentSpeed : 0f);
        rb.linearVelocity = new Vector3(inputWalk.normalized.x * currentSpeed, -1, inputWalk.normalized.y * currentSpeed);

        // Gira el personaje hacia la dirección del movimiento ---
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}