using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DatosGuardado
{
    public int monedas;
    public int monedasTotalesGanadas;
    public int golpesTotales;
    public float danoTotal;

    public List<string> armasCompradasNombres = new List<string>();
    public List<string> logrosDesbloqueadosIds = new List<string>();

    public float posX, posY, posZ;
    public float rotX, rotY, rotZ;
    public float camRotX;
    public string armaEquipadaNombre;
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
        datos.monedasTotalesGanadas = GestorJuego.Instance.monedasTotalesGanadas;
        datos.golpesTotales = GestorJuego.Instance.golpesTotales;
        datos.danoTotal = GestorJuego.Instance.danoTotal;

        BotonTienda[] botonesTienda = Object.FindObjectsByType<BotonTienda>(FindObjectsInactive.Include, FindObjectsSortMode.None);

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

        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            datos.posX = jugador.transform.position.x;
            datos.posY = jugador.transform.position.y;
            datos.posZ = jugador.transform.position.z;

            datos.rotX = jugador.transform.eulerAngles.x;
            datos.rotY = jugador.transform.eulerAngles.y;
            datos.rotZ = jugador.transform.eulerAngles.z;

            Camera camara = jugador.GetComponentInChildren<Camera>();
            if (camara != null)
            {
                datos.camRotX = camara.transform.localEulerAngles.x;
            }
        }

        ControladorCombate combate = Object.FindFirstObjectByType<ControladorCombate>();
        if (combate != null && combate.armaActual != null)
        {
            datos.armaEquipadaNombre = combate.armaActual.name;
        }

        string json = JsonUtility.ToJson(datos);
        PlayerPrefs.SetString("PartidaGuardada", json);
        PlayerPrefs.Save();

        Debug.Log("¡Partida, posición y equipamiento guardados!");
    }

    public void Cargar()
    {
        if (!PlayerPrefs.HasKey("PartidaGuardada")) return;

        string json = PlayerPrefs.GetString("PartidaGuardada");
        DatosGuardado datos = JsonUtility.FromJson<DatosGuardado>(json);

        GestorJuego.Instance.monedas = datos.monedas;
        GestorJuego.Instance.monedasTotalesGanadas = datos.monedasTotalesGanadas;
        GestorJuego.Instance.golpesTotales = datos.golpesTotales;
        GestorJuego.Instance.danoTotal = datos.danoTotal;

        GestorJuego.Instance.RecalcularMetas();

        foreach (Logro logro in GestorLogros.Instance.listaLogros)
        {
            logro.desbloqueado = datos.logrosDesbloqueadosIds.Contains(logro.id);
        }
        if (GestorLogros.Instance != null) GestorLogros.Instance.GuardarLogrosPermanentes();

        BotonTienda[] botonesTienda = Object.FindObjectsByType<BotonTienda>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (BotonTienda boton in botonesTienda)
        {
            if (boton.armaQueVende != null && datos.armasCompradasNombres.Contains(boton.armaQueVende.name))
            {
                boton.yaComprada = true;
            }
        }

        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            CharacterController cc = jugador.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            jugador.transform.position = new Vector3(datos.posX, datos.posY, datos.posZ);

            jugador.transform.rotation = Quaternion.Euler(datos.rotX, datos.rotY, datos.rotZ);

            Camera camara = jugador.GetComponentInChildren<Camera>();
            if (camara != null)
            {
                camara.transform.localRotation = Quaternion.Euler(datos.camRotX, 0f, 0f);
            }

            if (cc != null) cc.enabled = true;
        }

        if (!string.IsNullOrEmpty(datos.armaEquipadaNombre))
        {
            ControladorCombate combate = Object.FindFirstObjectByType<ControladorCombate>();
            if (combate != null)
            {
                foreach (BotonTienda boton in botonesTienda)
                {
                    if (boton.armaQueVende != null && boton.armaQueVende.name == datos.armaEquipadaNombre)
                    {
                        combate.EquiparArma(boton.armaQueVende);
                        break;
                    }
                }
            }
        }

        FindFirstObjectByType<UIJuego>().ActualizarHUD();
        Debug.Log("¡Partida cargada con éxito!");
    }
}