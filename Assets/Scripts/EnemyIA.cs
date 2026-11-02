using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Patrullaje")]
    public int rutina;
    public float cronometro;
    public Animator ani;
    public Quaternion angulo;
    public float grado;

    [Header("Jugador")]
    public GameObject target;
    public bool atacando;

    [Header("Distancias")]
    public float distanciaDeteccion = 5f;
    public float distanciaAtaque = 1f;

    [Header("Audio de pasos")]
    [SerializeField] private float walkFootstepInterval = 0.55f;
    [SerializeField] private float runFootstepInterval = 0.35f;

    private float footstepTimer;
    private PlayerHideController playerHideController;

    void Start()
    {
        ani = GetComponent<Animator>();
        target = GameObject.Find("Player");

        if (target != null)
        {
            playerHideController =
                target.GetComponent<PlayerHideController>();
        }
        else
        {
            Debug.LogError(
                "No se encontró al jugador. Debe llamarse Player."
            );
        }
    }

    void Update()
    {
        if (target == null)
            return;

        if (playerHideController != null &&
            playerHideController.IsHidden)
        {
            DetenerEnemigo();
            footstepTimer = 0f;
            return;
        }

        Comportamiento_Enemigo();
        ActualizarPasos();
    }

    private void DetenerEnemigo()
    {
        if (ani != null)
        {
            ani.SetBool("walk", false);
            ani.SetBool("run", false);
            ani.SetBool("attack", false);
        }

        atacando = false;
    }

    public void Comportamiento_Enemigo()
    {
        if (target == null)
            return;

        float distancia = Vector3.Distance(
            transform.position,
            target.transform.position
        );

        // PATRULLAJE
        if (distancia > distanciaDeteccion)
        {
            if (ani != null)
                ani.SetBool("run", false);

            cronometro += Time.deltaTime;

            if (cronometro >= 4f)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0f;
            }

            switch (rutina)
            {
                case 0:
                    if (ani != null)
                        ani.SetBool("walk", false);
                    break;

                case 1:
                    grado = Random.Range(0f, 360f);
                    angulo = Quaternion.Euler(0f, grado, 0f);
                    rutina = 2;
                    break;

                case 2:
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        angulo,
                        90f * Time.deltaTime
                    );

                    transform.Translate(
                        Vector3.forward * Time.deltaTime,
                        Space.Self
                    );

                    if (ani != null)
                        ani.SetBool("walk", true);
                    break;
            }
        }
        // PERSECUCIÓN
        else if (distancia > distanciaAtaque)
        {
            Vector3 direccion =
                target.transform.position - transform.position;

            direccion.y = 0f;

            if (direccion.sqrMagnitude > 0.001f)
            {
                Quaternion rotacion =
                    Quaternion.LookRotation(direccion);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    rotacion,
                    180f * Time.deltaTime
                );
            }

            if (ani != null)
            {
                ani.SetBool("walk", false);
                ani.SetBool("run", true);
                ani.SetBool("attack", false);
            }

            atacando = false;

            transform.Translate(
                Vector3.forward * 2f * Time.deltaTime,
                Space.Self
            );
        }
        // ATAQUE
        else
        {
            if (!atacando &&
                HorrorSFXManager.Instance != null)
            {
                HorrorSFXManager.Instance.PlaySFX(
                    HorrorSFXEvent.EnemyAttack,
                    transform.position
                );
            }

            if (ani != null)
            {
                ani.SetBool("walk", false);
                ani.SetBool("run", false);
                ani.SetBool("attack", true);
            }

            atacando = true;
        }
    }

    private void ActualizarPasos()
    {
        if (ani == null)
            return;

        bool walking = ani.GetBool("walk");
        bool running = ani.GetBool("run");

        if (!walking && !running)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer += Time.deltaTime;

        float interval = running
            ? runFootstepInterval
            : walkFootstepInterval;

        if (footstepTimer >= interval)
        {
            if (HorrorSFXManager.Instance != null)
            {
                HorrorSFXManager.Instance.PlaySFX(
                    HorrorSFXEvent.EnemyFootsteps,
                    transform.position
                );
            }

            footstepTimer = 0f;
        }
    }

    public void Final_Ani()
    {
        if (ani != null)
            ani.SetBool("attack", false);

        atacando = false;
    }
}

