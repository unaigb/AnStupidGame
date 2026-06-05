using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotificacionLogroUI : MonoBehaviour
{
    public static NotificacionLogroUI Instance;

    [Header("Componentes Visuales")]
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoDescripcion;
    public Image imagenIcono;

    [Header("Configuración de Movimiento")]
    public RectTransform panelNotificacion;
    public float velocidadSeparacion = 0.5f;
    public float tiempoEnPantalla = 3f;

    private Vector2 posicionEscondido;
    private Vector2 posicionVisible;
    private bool estaMostrandoLogro = false;

    private Queue<Logro> colaLogros = new Queue<Logro>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        posicionVisible = new Vector2(0, -30f);
        posicionEscondido = new Vector2(0, 150f);

        panelNotificacion.anchoredPosition = posicionEscondido;
    }

    public void EncolarLogro(Logro nuevoLogro)
    {
        colaLogros.Enqueue(nuevoLogro);

        if (!estaMostrandoLogro)
        {
            StartCoroutine(ProcesarColaNotificaciones());
        }
    }

    private IEnumerator ProcesarColaNotificaciones()
    {
        estaMostrandoLogro = true;

        while (colaLogros.Count > 0)
        {
            Logro logroActual = colaLogros.Dequeue();

            textoTitulo.text = logroActual.titulo;
            textoDescripcion.text = logroActual.descripcion;
            if (imagenIcono != null && logroActual.icono != null)
            {
                imagenIcono.sprite = logroActual.icono;
            }

            float tiempo = 0;
            while (tiempo < velocidadSeparacion)
            {
                tiempo += Time.deltaTime;
                panelNotificacion.anchoredPosition = Vector2.Lerp(posicionEscondido, posicionVisible, tiempo / velocidadSeparacion);
                yield return null;
            }
            panelNotificacion.anchoredPosition = posicionVisible;

            yield return new WaitForSeconds(tiempoEnPantalla);

            tiempo = 0;
            while (tiempo < velocidadSeparacion)
            {
                tiempo += Time.deltaTime;
                panelNotificacion.anchoredPosition = Vector2.Lerp(posicionVisible, posicionEscondido, tiempo / velocidadSeparacion);
                yield return null;
            }
            panelNotificacion.anchoredPosition = posicionEscondido;

            yield return new WaitForSeconds(0.2f);
        }

        estaMostrandoLogro = false;
    }
}