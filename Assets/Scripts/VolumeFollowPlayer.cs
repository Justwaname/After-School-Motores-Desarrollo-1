using UnityEngine;

public class VolumeFollowPlayer : MonoBehaviour
{
    public Transform playerTransform;

    void Update()
    {
        if (playerTransform != null)
        {
            // Mantiene la posición de escucha de volúmenes en el jugador
            Shader.SetGlobalVector("_WorldSpaceCameraPos", playerTransform.position);
        }
    }
}
