using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TarjetaLogro : MonoBehaviour
{
    public string idLogroAsociado;

    [Header("Componentes Visuales")]
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoDescripcion;
    public Image fondoTarjeta;
    public GameObject iconoCandado;

    [Header("Configuración de Colores")]
    public Color colorBloqueado = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color colorDesbloqueado = new Color(0.15f, 0.4f, 0.15f, 0.9f);

    public void ActualizarVisual(Logro datosLogro)
    {
        if (datosLogro == null) return;

        if (!datosLogro.desbloqueado && datosLogro.esOculto)
        {
            textoTitulo.text = "???";
            textoDescripcion.text = "??????????????????????????????????";
            fondoTarjeta.color = colorBloqueado;

            if (iconoCandado != null) iconoCandado.SetActive(true);
        }

        else if (datosLogro.desbloqueado)
        {
            textoTitulo.text = datosLogro.titulo;
            textoDescripcion.text = datosLogro.descripcion;
            fondoTarjeta.color = colorDesbloqueado;

            if (iconoCandado != null) iconoCandado.SetActive(false);
        }

        else
        {
            textoTitulo.text = datosLogro.titulo;
            textoDescripcion.text = datosLogro.descripcion;
            fondoTarjeta.color = colorBloqueado;

            if (iconoCandado != null) iconoCandado.SetActive(true);
        }
    }
}