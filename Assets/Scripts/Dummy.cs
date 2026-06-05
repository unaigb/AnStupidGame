using UnityEngine;

public class Dummy : MonoBehaviour
{
    [Header("Configuración de Drops")]
    [Range(0f, 1f)]
    public float probabilidadDropMoneda = 0.3f;
    public int monedasPorDrop = 5;

    private Animator animador;

    void Start()
    {
        animador = GetComponentInChildren<Animator>();

        if (animador == null)
        {
            Debug.LogError("¡ERROR! El Dummy no encuentra el Animator. Revisa la jerarquía.");
        }
    }

    public void RecibirGolpe(float dano)
    {
        if (animador != null)
        {
            animador.SetTrigger("RecibirGolpe");
        }

        if (Random.value <= probabilidadDropMoneda)
        {
            GestorJuego.Instance.AnadirMonedas(monedasPorDrop);
        }
    }
}