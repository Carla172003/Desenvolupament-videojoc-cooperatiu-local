using UnityEngine;
using System.Collections;

/// <summary>
/// Controlador dels personatges no jugables (NPC).
/// Gestiona el comportament de vagareig aleatori i l'estat de vestit.
/// Els NPCs poden ser vestits amb objectes específics per completar els objectius del nivell.
/// </summary>
public class ControladorNPC : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 1.5f;
    public float moveDistance = 1f;
    public float minIdleTime = 1.5f;
    public float maxIdleTime = 4f;

    [HideInInspector]
    public Animator animator;

    public bool estaVestit { get; private set; } = false;


    private bool miraDreta = true; // true = derecha, false = izquierda

    /// <summary>
    /// Inicialitza el NPC. Obté l'animator i comença la rutina de vagareig.
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", false);
        StartCoroutine(WanderRoutine());
    }

    /// <summary>
    /// Estableix l'estat de vestit del NPC.
    /// Actualitza l'animator per mostrar l'animació corresponent.
    /// </summary>
    /// <param name="puesto">True si el NPC està vestit, false si no.</param>
    public void SetVestidoPuesto(bool puesto)
    {
        estaVestit = puesto;

        if (animator != null)
        {
            animator.SetBool("estaVestit", puesto);
        }
    }

    /// <summary>
    /// Corutina que gestiona el comportament de vagareig del NPC.
    /// Alterna entre períodes d'inactivitat i moviment en direcció aleatòria.
    /// </summary>
    /// <returns>IEnumerator per a la corutina.</returns>
    IEnumerator WanderRoutine()
    {
        while (true)
        {
            // --- IDLE ---
            animator.SetBool("isWalking", false);
            yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));

            // Decide dirección
            int direction = Random.value < 0.5f ? -1 : 1;

            // Girar solo si hace falta
            if ((direction == 1 && !miraDreta) || (direction == -1 && miraDreta))
                Girar();

            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + Vector3.right * direction * moveDistance;

            // --- WALK ---
            animator.SetBool("isWalking", true);
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            animator.SetBool("isWalking", false);
        }
    }

    /// <summary>
    /// Gira el sprite del NPC horitzontalment invertint l'escala X.
    /// </summary>
    private void Girar()
    {
        miraDreta = !miraDreta;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}

