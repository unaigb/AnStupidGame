using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BotonTienda : MonoBehaviour
{
    [Header("ARRASTRA AQUÍ EL SCRIPTABLE OBJECT")]
    public DatosArma armaQueVende;

    // VARIABLES PRIVADAS: El Inspector ya no puede cruzarlas por error
    private TextMeshProUGUI textoNombre;
    private Image imagenEspada;
    private TextMeshProUGUI textoEstadisticas;
    private Button botonCompra;
    private TextMeshProUGUI textoBotonCompra;

    public bool yaComprada = false;
    private ControladorCombate jugador;

    void Awake()
    {
        Transform tNombre = transform.Find("TextoNombre");
        if (tNombre != null) textoNombre = tNombre.GetComponent<TextMeshProUGUI>();
        else Debug.LogError($"[Tienda] Falta el objeto hijo 'TextoNombre' en el botón: {gameObject.name}", gameObject);

        Transform tImagen = transform.Find("ImagenEspada");
        if (tImagen != null) imagenEspada = tImagen.GetComponent<Image>();
        else Debug.LogError($"[Tienda] Falta el objeto hijo 'ImagenEspada' en el botón: {gameObject.name}", gameObject);

        Transform tEstadisticas = transform.Find("TextoEstadisticas");
        if (tEstadisticas != null) textoEstadisticas = tEstadisticas.GetComponent<TextMeshProUGUI>();
        else Debug.LogError($"[Tienda] Falta el objeto hijo 'TextoEstadisticas' en el botón: {gameObject.name}", gameObject);

        Transform tBoton = transform.Find("BotonComprar");
        if (tBoton != null)
        {
            botonCompra = tBoton.GetComponent<Button>();

            Transform tPrecio = botonCompra.transform.Find("TextoPrecio");
            if (tPrecio != null) textoBotonCompra = tPrecio.GetComponent<TextMeshProUGUI>();
            else Debug.LogError($"[Tienda] Falta el objeto hijo 'TextoPrecio' dentro de BotonComprar en: {gameObject.name}", gameObject);
        }
        else
        {
            Debug.LogError($"[Tienda] Falta el objeto hijo 'BotonComprar' en el botón: {gameObject.name}", gameObject);
        }
    }

    void Start()
    {
        jugador = FindFirstObjectByType<ControladorCombate>();

        if (armaQueVende != null)
        {
            if (GestorJuego.Instance == null) return;

            textoNombre.text = armaQueVende.nombreArma;
            if (armaQueVende.iconoTienda != null) imagenEspada.sprite = armaQueVende.iconoTienda;

            textoEstadisticas.text = $"Daño: {armaQueVende.dano}\nVelocidad: {armaQueVende.ataquesPorSegundo}/s\nCrítico: {armaQueVende.probabilidadCritico * 100}%";

            botonCompra.onClick.AddListener(AlPulsarBoton);

            if (armaQueVende.costeEnTienda == 0) yaComprada = true;
        }
        else
        {
            Debug.LogError($"¡ATENCIÓN! La tarjeta {gameObject.name} no tiene ningún ScriptableObject asignado.");
        }
    }

    void Update()
    {
        if (jugador == null || armaQueVende == null || textoBotonCompra == null) return;

        if (jugador.armaActual == armaQueVende)
        {
            textoBotonCompra.text = "Equipada";
            botonCompra.interactable = false;
        }
        else if (yaComprada)
        {
            textoBotonCompra.text = "Equipar";
            botonCompra.interactable = true;
        }
        else
        {
            textoBotonCompra.text = $"{armaQueVende.costeEnTienda} Monedas";
            botonCompra.interactable = true;
        }
    }

    void AlPulsarBoton()
    {
        if (yaComprada)
        {
            jugador.EquiparArma(armaQueVende);
        }
        else
        {
            if (GestorJuego.Instance.monedas >= armaQueVende.costeEnTienda)
            {
                GestorJuego.Instance.monedas -= armaQueVende.costeEnTienda;
                FindFirstObjectByType<UIJuego>().ActualizarHUD();

                yaComprada = true;
                jugador.EquiparArma(armaQueVende);

                GestorJuego.Instance.ReproducirSonidoCompra();

                if (GestorLogros.Instance != null)
                {
                    GestorLogros.Instance.RegistrarCompra();
                }
            }
            else
            {
                Debug.LogWarning("No tienes dinero suficiente.");

                GestorJuego.Instance.ReproducirSonidoError();
            }
        }
    }
}