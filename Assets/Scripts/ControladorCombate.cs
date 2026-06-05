using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorCombate : MonoBehaviour
{
    [Header("Arma Equipada")]
    public DatosArma armaActual;

    [Header("Visuales del Arma")]
    [Tooltip("Arrastra aquí el objeto vacío que crearemos como hijo de la cámara")]
    public Transform puntoAnclajeArma;
    private GameObject modeloArmaInstanciado;

    [Header("Configuración de Raycast")]
    public float distanciaAtaque = 3f;

    [Header("Efectos y Animaciones")]
    public Animator animadorArma;
    public ParticleSystem particulasCritico;
    public ParticleSystem particulasMonedas;

    [Header("Sonidos")]
    public AudioSource altavozJugador;
    public AudioClip sonidoEspadazoAire;
    public AudioClip sonidoImpacto;
    public AudioClip sonidoCritico;
    public AudioClip sonidoMoneda;

    private float tiempoUltimoAtaque;
    private Camera camaraPrincipal;

    void Start()
    {
        camaraPrincipal = GetComponent<Camera>();
        ActualizarModeloArma();
    }

    // Este método destruye el arma vieja y genera la nueva en la mano
    public void ActualizarModeloArma()
    {
        // 1. Si ya teníamos un arma visual en la mano, la borramos
        if (modeloArmaInstanciado != null)
        {
            Destroy(modeloArmaInstanciado);
        }

        // 2. Si tenemos un arma seleccionada y tiene un modelo 3D asignado...
        if (armaActual != null && armaActual.prefabModelo != null && puntoAnclajeArma != null)
        {
            // Instanciamos el modelo 3D como HIJO de nuestro punto de anclaje
            modeloArmaInstanciado = Instantiate(armaActual.prefabModelo, puntoAnclajeArma);

            // Reseteamos su posición y rotación locales para empezar desde el centro del anclaje
            modeloArmaInstanciado.transform.localPosition = Vector3.zero;
            modeloArmaInstanciado.transform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        if (armaActual == null) return;

        float tiempoEntreAtaques = 1f / armaActual.ataquesPorSegundo;

        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
        {
            Atacar();
        }
    }

    void Atacar()
    {
        if (GestorJuego.Instance.tiendaAbierta ||
        GestorJuego.Instance.logrosAbierto ||
        GestorJuego.Instance.jugadorMuerto ||
        Time.timeScale == 0f)
        {
            return;
        }

        if (GestorLogros.Instance != null) GestorLogros.Instance.RegistrarEspadazoAire();

        tiempoUltimoAtaque = Time.time;

        // 1. Animación del arma (la lanzamos siempre que intentamos atacar)
        if (animadorArma != null) animadorArma.SetTrigger("Atacar");
        if (altavozJugador != null && sonidoEspadazoAire != null) altavozJugador.PlayOneShot(sonidoEspadazoAire);

        // 2. Comprobación del arma Troll
        if (armaActual.esArmaTroll)
        {
            if (Random.value <= armaActual.probabilidadReboteMortal)
            {
                GestorJuego.Instance.DerrotaPorArmaTroll();
                return;
            }
        }

        // 3. Cálculos de daño
        float danoFinal = armaActual.dano;
        bool esCritico = Random.value <= armaActual.probabilidadCritico;

        if (esCritico)
        {
            danoFinal *= armaActual.multiplicadorCritico;
            Debug.Log("¡GOLPE CRÍTICO!");
        }

        // 4. Raycast para detectar el golpe
        Ray rayo = new Ray(camaraPrincipal.transform.position, camaraPrincipal.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, distanciaAtaque))
        {
            Dummy dummy = hit.collider.GetComponent<Dummy>();

            if (dummy != null)
            {
                dummy.RecibirGolpe(danoFinal);

                if (GestorLogros.Instance != null) GestorLogros.Instance.RegistrarImpactoReal(esCritico);

                if (esCritico)
                {
                    if (particulasCritico != null)
                    {
                        particulasCritico.transform.position = hit.point;
                        particulasCritico.Play();
                    }
                    if (altavozJugador != null && sonidoCritico != null) altavozJugador.PlayOneShot(sonidoCritico);
                }
                else
                {
                    if (altavozJugador != null && sonidoImpacto != null) altavozJugador.PlayOneShot(sonidoImpacto);
                }

                int monedasSoltadas = GestorJuego.Instance.RegistrarImpacto(danoFinal);

                if (monedasSoltadas > 0)
                {
                    if (particulasMonedas != null)
                    {
                        particulasMonedas.transform.position = hit.point;
                        particulasMonedas.Play();
                    }
                    if (altavozJugador != null && sonidoMoneda != null) altavozJugador.PlayOneShot(sonidoMoneda);
                }
            }
        }
    }

    public void EquiparArma(DatosArma nuevaArma)
    {
        armaActual = nuevaArma;
        ActualizarModeloArma(); // Esto destruye la vieja y pone el modelo 3D de la nueva
        Debug.Log("Has equipado: " + nuevaArma.nombreArma);
    }
}