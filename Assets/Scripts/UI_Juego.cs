using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIJuego : MonoBehaviour
{
    [Header("Componentes HUD")]
    public TextMeshProUGUI textoMonedas;
    public TextMeshProUGUI textoDano;
    public TextMeshProUGUI textoGolpes;

    [Header("Componentes Tienda")]
    public GameObject panelTienda;
    private bool tiendaAbierta = false;

    [Header("Componentes Logros")]
    public GameObject panelLogros;
    private bool logrosAbierto = false;

    [Header("Componentes Muerte")]
    public GameObject panelMuerte;
    public TextMeshProUGUI textoCausaMuerte;

    [Header("Componentes Pausa")]
    public GameObject panelPausa;
    private bool juegoPausado = false;

    private FirstPersonController fpsController;

    private StarterAssetsInputs fpsInputs;

    void Start()
    {
        if (panelTienda != null) panelTienda.SetActive(false);
        if (panelLogros != null) panelLogros.SetActive(false);
        if (panelMuerte != null) panelMuerte.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);

        fpsController = FindFirstObjectByType<FirstPersonController>();
        fpsInputs = FindFirstObjectByType<StarterAssetsInputs>();

        ActualizarHUD();
        Time.timeScale = 1f;

        ManejarEstadoCursorYControles();
    }

    void Update()
    {
        if (GestorJuego.Instance != null && GestorJuego.Instance.jugadorMuerto) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePausa();
        }

        if (!juegoPausado)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) ToggleTienda();
            if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame) ToggleLogros();
        }
    }

    public void ActualizarHUD()
    {
        if (textoMonedas != null)
            textoMonedas.text = $"{GestorJuego.Instance.monedas}";

        if (textoDano != null)
            textoDano.text = $"{System.Math.Round(GestorJuego.Instance.danoTotal, 1)}";

        if (textoGolpes != null)
            textoGolpes.text = $"{GestorJuego.Instance.golpesTotales}";
    }

    public void TogglePausa()
    {
        if (tiendaAbierta) { CerrarTiendaSilencioso(); ManejarEstadoCursorYControles(); return; }
        if (logrosAbierto) { CerrarLogrosSilencioso(); ManejarEstadoCursorYControles(); return; }

        juegoPausado = !juegoPausado;
        if (panelPausa != null) panelPausa.SetActive(juegoPausado);

        Time.timeScale = juegoPausado ? 0f : 1f;

        ManejarEstadoCursorYControles();
    }

    public void BotonPausaGuardar()
    {
        if (SistemaGuardado.Instance != null)
        {
            SistemaGuardado.Instance.Guardar();
        }
    }

    public void BotonPausaSalirAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void ToggleTienda()
    {
        if (logrosAbierto) CerrarLogrosSilencioso();

        tiendaAbierta = !tiendaAbierta;
        panelTienda.SetActive(tiendaAbierta);

        if (GestorJuego.Instance != null) GestorJuego.Instance.tiendaAbierta = tiendaAbierta;

        ManejarEstadoCursorYControles();
    }

    public void ToggleLogros()
    {
        if (tiendaAbierta) CerrarTiendaSilencioso();

        logrosAbierto = !logrosAbierto;
        panelLogros.SetActive(logrosAbierto);

        if (GestorJuego.Instance != null) GestorJuego.Instance.logrosAbierto = logrosAbierto;

        if (logrosAbierto && GestorLogros.Instance != null)
        {
            GestorLogros.Instance.ActualizarInterfazLogros();
        }

        ManejarEstadoCursorYControles();
    }

    private void CerrarTiendaSilencioso()
    {
        tiendaAbierta = false;
        panelTienda.SetActive(false);
        if (GestorJuego.Instance != null) GestorJuego.Instance.tiendaAbierta = false;
    }

    private void CerrarLogrosSilencioso()
    {
        logrosAbierto = false;
        panelLogros.SetActive(false);
        if (GestorJuego.Instance != null) GestorJuego.Instance.logrosAbierto = false;
    }

    private void ManejarEstadoCursorYControles()
    {
        if (tiendaAbierta || logrosAbierto || juegoPausado || (GestorJuego.Instance != null && GestorJuego.Instance.jugadorMuerto))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (fpsController != null) fpsController.enabled = false;

            if (fpsInputs != null)
            {
                fpsInputs.cursorLocked = false;
                fpsInputs.cursorInputForLook = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (fpsController != null) fpsController.enabled = true;

            if (fpsInputs != null)
            {
                fpsInputs.cursorLocked = true;
                fpsInputs.cursorInputForLook = true;
            }
        }
    }

    public void MostrarPantallaMuerte(string causa)
    {
        CerrarTiendaSilencioso();
        CerrarLogrosSilencioso();

        if (panelMuerte != null) panelMuerte.SetActive(true);

        if (textoCausaMuerte != null)
        {
            if (causa == "caida") textoCausaMuerte.text = "Te has caído al vacío de la isla flotante.";
            if (causa == "troll") textoCausaMuerte.text = "El poder inestable del Arma Alien te ha desintegrado.";
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (fpsController != null) fpsController.enabled = false;
    }

    public void ReiniciarPartida()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}