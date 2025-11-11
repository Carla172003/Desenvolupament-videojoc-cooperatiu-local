using UnityEngine;

public class ControladorObjecte : MonoBehaviour
{
    public string idObjecte; // Identificador únic per a l'objecte
    
    [Header("Punt de Col·locació Correcte")]
    public PuntColocacio puntCorrecte;
    public float snapDistance = 1f;
    public bool colocat = false;

    [Header("Opcions d'agafar")]
    public bool necessitaDosJugadors = false;
    [HideInInspector] public bool jugador1Preparat = false;
    [HideInInspector] public bool jugador2Preparat = false;

    public void IntentarColocar()
    {
        if (puntCorrecte == null || colocat) return;

        float distancia = Vector2.Distance(transform.position, puntCorrecte.transform.position);

        // Comprova si està dins de la distància per fer "snap"
        if (distancia <= snapDistance && !puntCorrecte.ocupat)
        {
            // Col·loca l'objecte al punt correcte
            transform.position = puntCorrecte.transform.position;
            transform.rotation = puntCorrecte.transform.rotation;
            transform.localScale = Vector3.one;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true;

            colocat = true;
            puntCorrecte.ocupat = true;

            // Eliminar components innecessaris i canviar capa
            SpriteRenderer[] rends = GetComponentsInChildren<SpriteRenderer>();
            foreach (var r in rends)
            {
                if (r != null)
                    r.sortingLayerName = "SiluetesEscenari";
            }

            Rigidbody2D[] rbs = GetComponentsInChildren<Rigidbody2D>();
            foreach (var rcomp in rbs)
            {
                if (rcomp != null)
                    Destroy(rcomp);
            }

            BoxCollider2D[] boxes = GetComponentsInChildren<BoxCollider2D>();
            foreach (var b in boxes)
            {
                if (b != null)
                    Destroy(b);
            }

            //Crides a GameManager per comprovar si s'ha guanyat
            FindObjectOfType<GameManager>().ComprovarVictoria();
        }
    }
}