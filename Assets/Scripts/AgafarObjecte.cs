using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgafarObjecte : MonoBehaviour
{
    public GameObject puntAgafar; // Mano o punto donde se sujeta
    public float separacioDelSol = 0.05f; // Pequeña separación del suelo
    public float offsetHoritzontal = 0f;  // Si quieres moverlo un poco a los lados
    private GameObject objecteAgafat;
    private KeyCode specialKey;
    private Animator animator;
    [SerializeField] private bool teObjecte;
    private GameObject objecteProper;

    void Start()
    {
        AssignarTecles();
        animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(specialKey))
        {
            if (teObjecte)
                DeixarObjecte();
            else if (objecteProper != null)
                Agafar(objecteProper);
        }

        // Mantener el objeto a ras de suelo si lo estamos sujetando
        if (teObjecte && objecteAgafat != null)
        {
            PosicionarARasDeSol(objecteAgafat);
        }

        animator.SetBool("teObjecte", teObjecte);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Objecte"))
            objecteProper = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Objecte") && objecteProper == other.gameObject)
            objecteProper = null;
    }

    private void Agafar(GameObject obj)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = true;

        obj.transform.SetParent(puntAgafar.transform);
        PosicionarARasDeSol(obj);

        objecteAgafat = obj;
        teObjecte = true;
    }

    private void DeixarObjecte()
    {
        if (objecteAgafat == null) return;

        Rigidbody2D rb = objecteAgafat.GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
        rb.isKinematic = false;

        objecteAgafat.transform.SetParent(null);

        ObjecteColocable colocable = objecteAgafat.GetComponent<ObjecteColocable>();
        if (colocable != null)
        {
            colocable.IntentarColocar();
        }

        objecteAgafat = null;
        teObjecte = false;
    }

    private void PosicionarARasDeSol(GameObject obj)
    {
        Collider2D col = obj.GetComponent<Collider2D>();
        if (col == null) return;

        // Altura del collider
        float altura = col.bounds.size.y;

        // Y actual del borde inferior del objeto
        float yBaseActual = col.bounds.min.y;

        // Diferencia que hay entre donde está su base y donde debería estar
        float diferencia = puntAgafar.transform.position.y - yBaseActual;

        // Mueve el objeto entero esa diferencia hacia arriba o abajo
        obj.transform.position += new Vector3(0, diferencia + separacioDelSol, 0);
    }

    private void AssignarTecles()
    {
        if (CompareTag("Ma1"))
            specialKey = KeyCode.E;
        else if (CompareTag("Ma2"))
            specialKey = KeyCode.O;
    }
}
