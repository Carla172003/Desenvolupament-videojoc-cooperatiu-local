using UnityEngine;

/// <summary>
/// Estratègia de validació per objectes de tipus Llums (focus).
/// Comprova que tots els punts amb spotlight estiguin ocupats i amb el color correcte.
/// </summary>
public class ValidacioLlums : IValidacioEstrategia
{
    public bool Validar()
    {
        // Buscar tots els punts de col·locació
        PuntColocacio[] punts = Object.FindObjectsOfType<PuntColocacio>();
        
        foreach (PuntColocacio punt in punts)
        {
            // Només validar punts que tenen spotlight (objectes focus/llums)
            if (punt.spotlight != null)
            {
                if (!punt.ocupat || !punt.ColorEsCorrecto())
                {
                    return false;
                }
            }
        }
        
        return true;
    }
}
