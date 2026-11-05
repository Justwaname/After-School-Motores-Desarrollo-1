using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    [Header("Patrullaje por Pasillos")]
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    [SerializeField] private float waitTimeAtWaypoint = 2f;
    private float waitTimer = 0f;
    private bool waitingAtPoint = false;

    [Header("Animator y Componentes")]
    public Animator ani;
    private NavMeshAgent agent;
    [SerializeField] MusicManager musicManager;

    [Header("Jugador")]
    public GameObject target;
    public bool atacando;

    [Header("Distancias y Velocidades")]
    public float distanciaDeteccion = 9.5f;
    public float distanciaAtaque = 1.5f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer = 0f;

    [Header("Memoria de Persecución")]
    [SerializeField] private float loseTargetDelay = 6f;
    private float lossTimer = 0f;
    private bool isChasingLostTarget = false;

    [Header("Audio de pasos")]
    [SerializeField] private float walkFootstepInterval = 0.55f;
    [SerializeField] private float runFootstepInterval = 0.35f;

    private float footstepTimer;
    private PlayerHideController playerHideController;
    private bool wasHiddenLastFrame = false;

    // --- Control de pausa por UI ---
    private bool isPausedByUI = false;

    void Start()
    {
        ani = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player");

        if (target != null)
        {
            playerHideController = target.GetComponent<PlayerHideController>();
        }
        else
        {
            Debug.LogError("No se encontró al jugador. Debe llamarse Player.");
        }

        SiguientePuntoPatrulla();
    }

    void Update()
    {
        // Si la UI está abierta, congelamos todo el comportamiento del enemigo
        if (isPausedByUI)
            return;

        if (target == null)
            return;

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        bool isHiddenNow = (playerHideController != null && playerHideController.IsHidden);

        // Detectamos el preciso instante en que el jugador se esconde
        if (isHiddenNow && !wasHiddenLastFrame)
        {
            isChasingLostTarget = false;
            lossTimer = 0f;

            if (musicManager != null)
            {
                musicManager.SetLayerActive(0, false);
            }

            SiguientePuntoPatrulla();
        }

        wasHiddenLastFrame = isHiddenNow;

        if (isHiddenNow)
        {
            footstepTimer = 0f;
        }

        Comportamiento_Enemigo();
        ActualizarPasos();
    }

    // =========================================================================
    // MÉTODOS PARA PAUSAR / REANUDAR DESDE LA UI (Candado, Notas, etc.)
    // =========================================================================
    public void PauseEnemy()
    {
        isPausedByUI = true;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        if (ani != null)
        {
            ani.SetBool("walk", false);
            ani.SetBool("run", false);
            ani.SetBool("attack", false);
        }
    }

    public void ResumeEnemy()
    {
        isPausedByUI = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    private void DetenerEnemigo()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;

        if (ani != null)
        {
            ani.SetBool("walk", false);
            ani.SetBool("run", false);
            ani.SetBool("attack", false);
        }

        atacando = false;
        isChasingLostTarget = false;
    }

    public void Comportamiento_Enemigo()
    {
        if (target == null || isPausedByUI)
            return;

        bool jugadorInvisible = (playerHideController != null && playerHideController.IsHidden);

        float distancia = Vector3.Distance(transform.position, target.transform.position);

        if (atacando)
            return;

        // Intencificación de música por distancia
        float distanceToIntensify = 8f; // Distancia a la que la música se intensifica
        if (distancia <= distanceToIntensify)
        {
            musicManager.SetLayerActive(2, true);
            musicManager.SetLayerActive(4, true);
        }
        else
        {
            musicManager.SetLayerActive(2, false);
            musicManager.SetLayerActive(4, false);
        }
        // 1. ATAQUE (Solo si no está escondido)
        if (!jugadorInvisible && distancia <= distanciaAtaque)
        {
            lossTimer = 0f;
            isChasingLostTarget = false;

            if (agent.isOnNavMesh)
                agent.isStopped = true;

            Vector3 direccionJugador = (target.transform.position - transform.position).normalized;
            direccionJugador.y = 0;
            if (direccionJugador != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direccionJugador);
            }

            if (attackTimer <= 0)
            {
                AtacarJugador();
                attackTimer = attackCooldown;
            }
            return;
        }

        // 2. PERSECUCIÓN DIRECTA (Solo si no está escondido)
        if (!jugadorInvisible && distancia <= distanciaDeteccion)
        {
            lossTimer = loseTargetDelay;
            isChasingLostTarget = true;
            waitingAtPoint = false;

            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.speed = Mathf.MoveTowards(agent.speed, runSpeed, Time.deltaTime);
                agent.SetDestination(target.transform.position);
                musicManager.SetLayerActive(0, true);
            }

            if (ani != null)
            {
                ani.SetBool("walk", false);
                ani.SetBool("run", true);
                ani.SetBool("attack", false);
            }
            return;
        }

        // 3. PERSECUCIÓN POR MEMORIA
        if (isChasingLostTarget && !jugadorInvisible)
        {
            lossTimer -= Time.deltaTime;

            if (lossTimer > 0)
            {
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.speed = Mathf.MoveTowards(agent.speed, runSpeed, Time.deltaTime);
                    agent.SetDestination(target.transform.position);
                }

                if (ani != null)
                {
                    ani.SetBool("walk", false);
                    ani.SetBool("run", true);
                    ani.SetBool("attack", false);
                }
                return;
            }
            else
            {
                isChasingLostTarget = false;

                if (musicManager != null)
                {
                    musicManager.SetLayerActive(0, false);
                }

                SiguientePuntoPatrulla();
            }
        }

        // 4. PATRULLAJE POR PASILLOS
        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        if (agent.isOnNavMesh)
            agent.isStopped = false;

        agent.speed = walkSpeed;

        if (waitingAtPoint)
        {
            if (ani != null)
                ani.SetBool("walk", false);

            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                waitingAtPoint = false;
                SiguientePuntoPatrulla();
            }
        }
        else
        {
            if (ani != null)
                ani.SetBool("walk", agent.velocity.sqrMagnitude > 0.1f);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                waitingAtPoint = true;
                waitTimer = 0f;
            }
        }
    }

    public bool EstaEnPeriodoDeGracia(float segundosMinimos)
    {
        if (!isChasingLostTarget) return false;
        float tiempoTranscurrido = loseTargetDelay - lossTimer;
        return tiempoTranscurrido < segundosMinimos;
    }

    private void SiguientePuntoPatrulla()
    {
        if (waypoints.Length == 0 || waypoints[currentWaypointIndex] == null) return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    private void AtacarJugador()
    {
        atacando = true;

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.EnemyAttack,
                transform.position
            );
        }

        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage();
        }

        if (ani != null)
        {
            ani.SetBool("walk", false);
            ani.SetBool("run", false);
            ani.SetBool("attack", true);
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

        float interval = running ? runFootstepInterval : walkFootstepInterval;

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