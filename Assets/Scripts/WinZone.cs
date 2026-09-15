using UnityEngine;
using TMPro; 

public class WinZone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            if (winText != null)
            {
                winText.text = "YOU ESCAPED THANKS FOR PLAYING!"; 
                winText.gameObject.SetActive(true);
            }

            
            Time.timeScale = 0f;
        }
    }
}