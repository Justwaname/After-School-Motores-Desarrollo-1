using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
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
    private bool staminaDepleted = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }
    void Update()
    {
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
    }
    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(
            inputWalk.normalized.x,
            0,
            inputWalk.normalized.y
        );

        rb.linearVelocity = new Vector3(
            movement.x * currentSpeed,
            -1,
            movement.z * currentSpeed
        );

        // Hace que el frente del personaje (eje Z) mire hacia donde se mueve
        if (movement != Vector3.zero)
        {
            transform.forward = movement;
        }
    }
}