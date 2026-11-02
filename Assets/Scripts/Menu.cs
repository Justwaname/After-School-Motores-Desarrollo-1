using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
   
   public void jugar()
    {
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void volver()
    {
        SceneManager.LoadScene("menuinicial");
    }
    public void reiniciar()
    {

        SceneManager.LoadScene("SchoolTestScene");
    }

    public void salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
