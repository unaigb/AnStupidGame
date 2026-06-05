using UnityEngine;

[CreateAssetMenu(fileName = "NuevaArma", menuName = "Juego/Arma")]
public class DatosArma : ScriptableObject
{
    public string nombreArma;

    [Tooltip("El icono en 2D que se verá en la tienda")]
    public Sprite iconoTienda;
    public GameObject prefabModelo;

    [Header("Estadísticas Base")]
    public float dano = 1f;
    public float ataquesPorSegundo = 1f; // 1 = 1 golpe por segundo, 2 = 2 golpes/s

    [Header("Críticos")]
    public float probabilidadCritico = 0.05f; // 5% por defecto
    public float multiplicadorCritico = 3f; // El daño se multiplica por 3

    [Header("Economía")]
    public int costeEnTienda = 50;

    [Header("Arma Troll (Derrota)")]
    public bool esArmaTroll = false;
    public float probabilidadReboteMortal = 0.05f; // 5% de matarte al golpear
}