/// <summary>
/// Contador estático de muertes en level_cielo durante una sesión.
/// Persiste entre recargas de escena (static), se resetea al salir al menú.
/// </summary>
public static class CieloScoreManager
{
    public static int Deaths { get; private set; }

    public static void RegisterDeath() => Deaths++;

    public static void Reset() => Deaths = 0;

    /// <summary>
    /// Calcula el rango final.
    /// Si el buff x2 está activo, las muertes cuentan la mitad (redondeado hacia abajo).
    /// </summary>
    public static string GetRank(float scoreMultiplier)
    {
        int effective = scoreMultiplier >= 2f ? Deaths / 2 : Deaths;

        if (effective == 0) return "S";
        if (effective == 1) return "A";
        if (effective == 2) return "B";
        if (effective == 3) return "C";
        return "D";
    }

    /// <summary>Devuelve el color asociado al rango.</summary>
    public static UnityEngine.Color GetRankColor(string rank)
    {
        switch (rank)
        {
            case "S": return new UnityEngine.Color(1.0f, 0.84f, 0.0f);   // dorado
            case "A": return new UnityEngine.Color(0.4f, 0.8f, 1.0f);   // celeste
            case "B": return new UnityEngine.Color(0.4f, 1.0f, 0.4f);   // verde
            case "C": return new UnityEngine.Color(1.0f, 0.7f, 0.2f);   // naranja
            default:  return new UnityEngine.Color(0.8f, 0.2f, 0.2f);   // rojo (D)
        }
    }
}
