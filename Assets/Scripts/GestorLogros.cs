using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Logro
{
    public string id;
    public string titulo;
    [TextArea] public string descripcion;
    public bool desbloqueado;
    public Sprite icono;
    public bool esOculto;
}

public class GestorLogros : MonoBehaviour
{
    public static GestorLogros Instance;

    [Header("Lista de Logros")]
    public List<Logro> listaLogros = new List<Logro>();

    [Header("Sonido de Logro")]
    public AudioSource altavozLogros;
    public AudioClip sonidoLogroDesbloqueado;

    [Header("Contadores Ocultos")]
    private int rachaCriticos = 0;
    private float tiempoSinAtacar = 0f;
    private bool logroManosQuietasConseguido = false;

    void Start()
    {
        CargarLogrosPermanentes();
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!logroManosQuietasConseguido)
        {
            tiempoSinAtacar += Time.deltaTime;
            if (tiempoSinAtacar >= 60f)
            {
                DesbloquearLogro("manos_quietas");
                logroManosQuietasConseguido = true;
            }
        }
    }

    public void DesbloquearLogro(string idLogro)
    {
        Logro logro = listaLogros.Find(l => l.id == idLogro);

        if (logro != null && !logro.desbloqueado)
        {
            logro.desbloqueado = true;

            if (altavozLogros != null && sonidoLogroDesbloqueado != null)
            {
                altavozLogros.PlayOneShot(sonidoLogroDesbloqueado);
            }

            if (NotificacionLogroUI.Instance != null)
            {
                NotificacionLogroUI.Instance.EncolarLogro(logro);
            }

            GuardarLogrosPermanentes();

            Debug.Log($"LOGRO DESBLOQUEADO: {logro.titulo} - {logro.descripcion}");

            ComprobarPlatino();
        }
    }

    public void RegistrarEspadazoAire()
    {
        tiempoSinAtacar = 0f;
    }

    public void RegistrarImpactoReal(bool fueCritico)
    {
        DesbloquearLogro("primer_golpe");

        if (GestorJuego.Instance.golpesTotales >= 100) DesbloquearLogro("100_golpes");
        if (GestorJuego.Instance.golpesTotales >= 500) DesbloquearLogro("500_golpes");
        if (GestorJuego.Instance.golpesTotales >= 1000) DesbloquearLogro("1000_golpes");
        if (GestorJuego.Instance.golpesTotales >= 5000) DesbloquearLogro("5000_golpes");
        if (GestorJuego.Instance.golpesTotales >= 10000) DesbloquearLogro("10000_golpes");

        if (fueCritico)
        {
            DesbloquearLogro("primer_critico");
            rachaCriticos++;

            if (rachaCriticos >= 3) DesbloquearLogro("trebol_hojas");
            if (rachaCriticos >= 10) DesbloquearLogro("diosa_fortuna");
        }
        else
        {
            rachaCriticos = 0;
        }
    }

    public void ComprobarMonedas()
    {
        if (GestorJuego.Instance.monedasTotalesGanadas >= 1000000)
        {
            DesbloquearLogro("millonario");
        }
    }

    private void ComprobarPlatino()
    {
        bool faltanLogros = false;
        foreach (Logro l in listaLogros)
        {
            if (l.id != "platino" && !l.desbloqueado)
            {
                faltanLogros = true;
                break;
            }
        }

        if (!faltanLogros)
        {
            DesbloquearLogro("platino");
        }
    }

    public void RegistrarCompra()
    {
        DesbloquearLogro("primera_compra");

        BotonTienda[] todosLosBotones = Object.FindObjectsByType<BotonTienda>(FindObjectsSortMode.None);

        bool todoComprado = true;
        int armasReales = 0;

        foreach (BotonTienda boton in todosLosBotones)
        {
            if (boton.armaQueVende != null)
            {
                armasReales++;
                if (!boton.yaComprada)
                {
                    todoComprado = false;
                    break;
                }
            }
        }

        if (todoComprado && armasReales > 0)
        {
            DesbloquearLogro("tienda_completa");
        }
    }

    public void ActualizarInterfazLogros()
    {
        TarjetaLogro[] tarjetas = Object.FindObjectsByType<TarjetaLogro>(FindObjectsSortMode.None);

        foreach (TarjetaLogro tarjeta in tarjetas)
        {
            Logro datos = listaLogros.Find(l => l.id == tarjeta.idLogroAsociado);

            if (datos != null)
            {
                tarjeta.ActualizarVisual(datos);
            }
        }
    }

    public void GuardarLogrosPermanentes()
    {
        foreach (Logro logro in listaLogros)
        {
            PlayerPrefs.SetInt("LogroPermanente_" + logro.id, logro.desbloqueado ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void CargarLogrosPermanentes()
    {
        foreach (Logro logro in listaLogros)
        {
            if (PlayerPrefs.HasKey("LogroPermanente_" + logro.id))
            {
                logro.desbloqueado = PlayerPrefs.GetInt("LogroPermanente_" + logro.id) == 1;
            }
        }
    }
}