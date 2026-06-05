using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    public Button botonContinuar;
    public string nombreEscenaJuego = "EscenaJuego";

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (botonContinuar != null)
        {
            botonContinuar.interactable = PlayerPrefs.HasKey("PartidaGuardada");
        }
    }

    public void NuevaPartida()
    {
        PlayerPrefs.SetInt("DebeCargarPartida", 0);
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void Continuar()
    {
        PlayerPrefs.SetInt("DebeCargarPartida", 1);
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }
}