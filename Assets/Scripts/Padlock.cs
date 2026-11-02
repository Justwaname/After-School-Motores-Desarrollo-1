using UnityEngine;

public class Padlock : MonoBehaviour
{
    public static Padlock Instance;

    [Header("Configuración")]
    [Range(1, 9)]
    [SerializeField] private int codeLength = 4;

    [HideInInspector]
    public string generatedCode;

    // Permite que PadlockUI conozca la longitud del código.
    public int CodeLength => codeLength;

    private void Awake()
    {
        // Comprobar Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GenerateRandomCode();
    }

    // GENERAR CÓDIGO ALEATORIO
    private void GenerateRandomCode()
    {
        codeLength = Mathf.Clamp(codeLength, 1, 9);

        int maxNumber = (int)Mathf.Pow(10, codeLength);

        int randomNum = Random.Range(0, maxNumber);

        // Mantener los ceros iniciales.
        generatedCode = randomNum.ToString("D" + codeLength);

        Debug.Log("Código secreto de la partida: " + generatedCode);
    }

    // VERIFICAR CÓDIGO
    public bool VerifyCode(string inputCode)
    {
        if (string.IsNullOrEmpty(inputCode))
            return false;

        return inputCode.Trim() == generatedCode;
    }
}