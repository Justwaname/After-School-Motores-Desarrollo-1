using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinZone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private float delayBeforeLoad = 2f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            triggered = true;

            if (winText != null)
            {
                winText.text = "¡ESCAPASTE, GRACIAS POR JUGAR!";
                winText.gameObject.SetActive(true);
            }

            Time.timeScale = 0f;

            StartCoroutine(LoadNextSceneRoutine());
        }
    }

    private IEnumerator LoadNextSceneRoutine()
    {
        yield return new WaitForSecondsRealtime(delayBeforeLoad);

        Time.timeScale = 1f;

        SceneManager.LoadScene("FinalChase");
    }
}