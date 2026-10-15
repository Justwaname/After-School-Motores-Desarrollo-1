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

    // Detectar si el jugador está escondido en una taquilla
    private PlayerHideController playerHideController;

    // INICIALIZACIÓN
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

    // ACTUALIZACIÓN
    void Update()
    {
        if (target == null)
            return;

        // Si el jugador está escondido, detener al enemigo
        if (playerHideController != null &&
            playerHideController.IsHidden)
        {
            DetenerEnemigo();
            return;
        }

        // Si no está escondido, continuar con la IA normal
        Comportamiento_Enemigo();
    }

    // DETENER AL ENEMIGO
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

    // COMPORTAMIENTO DEL ENEMIGO
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
        // PERSEGUIR AL JUGADOR
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
        // ATACAR AL JUGADOR
        else
        {
            if (ani != null)
            {
                ani.SetBool("walk", false);
                ani.SetBool("run", false);
                ani.SetBool("attack", true);
            }

            atacando = true;
        }
    }

    // FINALIZAR ANIMACIÓN DE ATAQUE
    public void Final_Ani()
    {
        if (ani != null)
            ani.SetBool("attack", false);

        atacando = false;
    }
}

