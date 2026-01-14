using UnityEngine;

/// <summary>
/// Estratègia de validació per objectes de tipus Vestimenta.
/// Comprova que tots els NPCs estiguin vestits correctament.
/// </summary>
public class ValidacioVestimenta : IValidacioEstrategia
{
    public bool Validar()
    {
        // Buscar tots els NPCs de l'escena
        ControladorNPC[] npcs = Object.FindObjectsOfType<ControladorNPC>();
        
        foreach (ControladorNPC npc in npcs)
        {
            if (!npc.estaVestit)
            {
                return false;
            }
        }
        
        return true;
    }
}
