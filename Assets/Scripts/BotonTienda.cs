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
        // 1. OBLIGAMOS A LA TARJETA A BUSCAR A SUS PROPIOS HIJOS POR SU NOMBRE EXACTO
        textoNombre = transform.Find("TextoNombre").GetComponent<TextMeshProUGUI>();
        imagenEspada = transform.Find("ImagenEspada").GetComponent<Image>();
        textoEstadisticas = transform.Find("TextoEstadisticas").GetComponent<TextMeshProUGUI>();

        botonCompra = transform.Find("BotonComprar").GetComponent<Button>();
        textoBotonCompra = botonCompra.transform.Find("TextoPrecio").GetComponent<TextMeshProUGUI>();
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