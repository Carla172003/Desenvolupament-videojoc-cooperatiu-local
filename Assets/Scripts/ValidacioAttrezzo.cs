using UnityEngine;

/// <summary>
/// Estratègia de validació per objectes de tipus Attrezzo.
/// Comprova que tots els punts de col·locació per a objectes Attrezzo estiguin ocupats.
/// </summary>
public class ValidacioAttrezzo : IValidacioEstrategia
{
    public bool Validar()
    {
        // Buscar tots els punts de col·locació
        PuntColocacio[] punts = Object.FindObjectsOfType<PuntColocacio>();
        
        foreach (PuntColocacio punt in punts)
        {
            // Només validar punts que NO tenen spotlight (els attrezos no necessiten llum)
            // i que NO corresponen a vestimenta (els NPCs tenen el seu propi sistema)
            if (punt.spotlight == null && !EsPuntVestimenta(punt))
            {
                if (!punt.ocupat)
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    /// <summary>
    /// Comprova si un punt de col·locació correspon a vestimenta.
    /// Els punts de vestimenta són aquells que coincideixen amb un NPC.
    /// </summary>
    private bool EsPuntVestimenta(PuntColocacio punt)
    {
        ControladorNPC[] npcs = Object.FindObjectsOfType<ControladorNPC>();
        foreach (ControladorNPC npc in npcs)
        {
            InteraccioNPC interaccio = npc.GetComponent<InteraccioNPC>();
            if (interaccio != null && interaccio.idNPC == punt.idCorrecte)
            {
                return true;
            }
        }
        return false;
    }
}
