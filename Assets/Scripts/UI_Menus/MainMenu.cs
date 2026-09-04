using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Carga una escena usando el nombre que indiquemos
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Cierra el juego.
    public void ExitGame()
    {
        // Si estamos ejecutando el juego dentro del Editor de Unity<Application.Quit() no funciona.Por eso detenemos manualmente el Play Mode. :)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        // Si estamos en una build real del juego cerramos la aplicación.
#else
        Application.Quit();
#endif
    }
}