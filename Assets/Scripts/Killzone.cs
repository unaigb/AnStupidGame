using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GestorJuego.Instance != null)
            {
                GestorJuego.Instance.Morir("caida");
            }
        }
    }
}