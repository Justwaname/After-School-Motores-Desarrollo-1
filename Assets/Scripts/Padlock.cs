using UnityEngine;

public class Padlock : MonoBehaviour
{
    public static Padlock Instance;

    [SerializeField] private int codeLength = 4;
    [HideInInspector] public string generatedCode;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        GenerateRandomCode();
    }

    void GenerateRandomCode()
    {
        int randomNum = Random.Range(0, (int)Mathf.Pow(10, codeLength));

        generatedCode = randomNum.ToString("D" + codeLength);

        Debug.Log("Código secreto de la partida: " + generatedCode);
    }

    public bool VerifyCode(string inputCode)
    {
        return inputCode == generatedCode;
    }
}
