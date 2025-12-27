using UnityEngine;

public class ControladorObjecte : MonoBehaviour
    {
        public string idObjecte; // Identificador únic per a l'objecte
        
        [Header("Punt de Col·locació Correcte")]
        public float snapDistance = 1f;
        public bool colocat = false;

        [Header("Opcions d'agafar")]
        [HideInInspector] public bool estaAgafat = false;


        public void IntentarColocar()
    {
        if (colocat) return;

        // Buscar tots els punts de col·locació
        PuntColocacio[] punts = FindObjectsOfType<PuntColocacio>();

        PuntColocacio puntTrobat = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (var punt in punts)
        {
            // ID ha de coincidir i el punt ha d'estar lliure
            if (punt.ocupat) continue;
            if (punt.idCorrecte != idObjecte) continue;

            float distancia = Vector2.Distance(transform.position, punt.transform.position);

            // Ha d'estar dins del rang i ser el més proper
            if (distancia <= snapDistance && distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                puntTrobat = punt;
            }
        }

        // No hi ha cap punt vàlid
        if (puntTrobat == null) return;

        // 🔒 SNAP
        transform.position = puntTrobat.transform.position;
        transform.rotation = puntTrobat.transform.rotation;
        transform.localScale = Vector3.one;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.isKinematic = true;

        colocat = true;
        puntTrobat.ocupat = true;

        // Canviar capa
        SpriteRenderer[] rends = GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in rends)
            r.sortingLayerName = "Decoracions";

        // Eliminar física
        foreach (var r in GetComponentsInChildren<Rigidbody2D>())
            Destroy(r);

        foreach (var c in GetComponentsInChildren<Collider2D>())
            Destroy(c);

        // Punts
        FindObjectOfType<ControladorPuntuacio>()?.SumarPunts(100);

        // Encendre llum si és focus
        ControladorFocus focus = GetComponent<ControladorFocus>();
        if (focus != null)
        {
            focus.ConfigurarDesdePunt(puntTrobat);
            focus.EncenderLuz();
        }


        // Comprovar victòria
        FindObjectOfType<GameManager>()?.ComprovarVictoria();
    }


    private void OnDrawGizmos()
    {
        // Obté el collider (si existeix)
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        // Color del gizmo (groc)
        Gizmos.color = Color.yellow;

        // Calcular posició i mides amb offset i escala
        Vector3 pos = box.transform.TransformPoint(box.offset);
        Vector3 size = Vector3.Scale(box.size, box.transform.lossyScale);

        // Dibuixa el rectangle
        Gizmos.DrawWireCube(pos, size);
    }

}