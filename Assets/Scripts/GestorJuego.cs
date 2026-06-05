using UnityEngine;

public class GestorJuego : MonoBehaviour
{
    public static GestorJuego Instance;

    [Header("Estadísticas del Jugador")]
    public int monedas = 0;
    public int monedasTotalesGanadas = 0;
    public int golpesTotales = 0;
    public float danoTotal = 0f;

    [Header("Sonidos de Interfaz")]
    public AudioSource altavozUI;
    public AudioClip sonidoCompra;
    public AudioClip sonidoError;

    [Range(0f, 1f)]
    public float volumenError = 0.3f;

    [Header("Estado del Juego")]
    public bool tiendaAbierta = false;
    public bool logrosAbierto = false;

    [Header("Estado de la Partida")]
    public bool jugadorMuerto = false;

    private int metaGolpes = 10;
    private float metaDano = 10f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int RegistrarImpacto(float cantidadDano)
    {
        golpesTotales++;
        danoTotal += cantidadDano;
        int monedasGanadas = 0;

        if (golpesTotales >= metaGolpes)
        {
            monedasGanadas += 5;
            metaGolpes += 10;
        }

        if (danoTotal >= metaDano)
        {
            monedasGanadas += 5;
            metaDano += 10f;
        }

        monedas += monedasGanadas;
        UIJuego ui = FindFirstObjectByType<UIJuego>();
        if (ui != null) ui.ActualizarHUD();

        if (GestorLogros.Instance != null) GestorLogros.Instance.ComprobarMonedas();

        return monedasGanadas;
    }

    public void AnadirMonedas(int cantidad)
    {
        monedas += cantidad;
        monedasTotalesGanadas += cantidad;
        Debug.Log($"Has ganado {cantidad} monedas. Total: {monedas}");
        FindAnyObjectByType<UIJuego>().ActualizarHUD();
        if (GestorLogros.Instance != null) GestorLogros.Instance.ComprobarMonedas();
    }

    public void DerrotaPorArmaTroll()
    {
        Debug.LogWarning("¡EL ARMA TROLL HA REBOTADO! Has muerto.");
        golpesTotales = 0;
        danoTotal = 0f;
        monedas = 0;
        Morir("troll");
    }

    public void ReproducirSonidoCompra()
    {
        if (altavozUI != null && sonidoCompra != null)
        {
            altavozUI.PlayOneShot(sonidoCompra);
        }
    }

    public void ReproducirSonidoError()
    {
        if (altavozUI != null && sonidoError != null)
        {
            altavozUI.PlayOneShot(sonidoError, volumenError);
        }
    }

    public void Morir(string causa)
    {
        if (jugadorMuerto) return;
        jugadorMuerto = true;

        if (GestorLogros.Instance != null)
        {
            if (causa == "troll") GestorLogros.Instance.DesbloquearLogro("gambler");
        }

        UIJuego ui = FindFirstObjectByType<UIJuego>();
        if (ui != null)
        {
            ui.MostrarPantallaMuerte(causa);
        }
    }

    public void RecalcularMetas()
    {
        metaGolpes = ((golpesTotales / 10) + 1) * 10;
    }
}