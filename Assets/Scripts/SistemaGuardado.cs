using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DatosGuardado
{
    public int monedas;
    public int golpesTotales;
    public List<string> armasCompradasNombres = new List<string>();
    public List<string> logrosDesbloqueadosIds = new List<string>();
}

public class SistemaGuardado : MonoBehaviour
{
    public static SistemaGuardado Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("DebeCargarPartida", 0) == 1)
        {
            Cargar();
            PlayerPrefs.SetInt("DebeCargarPartida", 0);
        }
    }

    public void Guardar()
    {
        DatosGuardado datos = new DatosGuardado();

        datos.monedas = GestorJuego.Instance.monedas;
        datos.golpesTotales = GestorJuego.Instance.golpesTotales;

        BotonTienda[] botonesTienda = Object.FindObjectsByType<BotonTienda>(FindObjectsSortMode.None);
        foreach (BotonTienda boton in botonesTienda)
        {
            if (boton.yaComprada && boton.armaQueVende != null)
            {
                datos.armasCompradasNombres.Add(boton.armaQueVende.name);
            }
        }

        foreach (Logro logro in GestorLogros.Instance.listaLogros)
        {
            if (logro.desbloqueado)
            {
                datos.logrosDesbloqueadosIds.Add(logro.id);
            }
        }

        string json = JsonUtility.ToJson(datos);
        PlayerPrefs.SetString("PartidaGuardada", json);
        PlayerPrefs.Save();

        Debug.Log("¡Partida guardada con éxito!");
    }

    public void Cargar()
    {
        if (!PlayerPrefs.HasKey("PartidaGuardada")) return;

        string json = PlayerPrefs.GetString("PartidaGuardada");
        DatosGuardado datos = JsonUtility.FromJson<DatosGuardado>(json);

        GestorJuego.Instance.monedas = datos.monedas;
        GestorJuego.Instance.golpesTotales = datos.golpesTotales;

        foreach (Logro logro in GestorLogros.Instance.listaLogros)
        {
            logro.desbloqueado = datos.logrosDesbloqueadosIds.Contains(logro.id);
        }

        BotonTienda[] botonesTienda = Object.FindObjectsByType<BotonTienda>(FindObjectsSortMode.None);
        foreach (BotonTienda boton in botonesTienda)
        {
            if (boton.armaQueVende != null && datos.armasCompradasNombres.Contains(boton.armaQueVende.name))
            {
                boton.yaComprada = true;
            }
        }

        FindFirstObjectByType<UIJuego>().ActualizarHUD();
        Debug.Log("¡Partida cargada con éxito!");
    }
}