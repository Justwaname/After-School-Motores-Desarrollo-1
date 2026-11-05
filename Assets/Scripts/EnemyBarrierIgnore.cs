using UnityEngine;

public class EnemyBarrierIgnore : MonoBehaviour
{
    private void Start()
    {
        Collider barrierCollider = GetComponent<Collider>();

        // Buscamos al enemigo en la escena automáticamente
        EnemyIA enemy = Object.FindFirstObjectByType<EnemyIA>();
        if (enemy != null)
        {
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (barrierCollider != null && enemyCollider != null)
            {
                // Hacemos que este muro ignore las colisiones exclusivamente con el enemigo
                Physics.IgnoreCollision(barrierCollider, enemyCollider, true);
            }
        }
    }
}
