using UnityEngine;
using System.Collections;

public class ControladorNPC : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 1.5f;
    public float moveDistance = 1f;
    public float minIdleTime = 1.5f;
    public float maxIdleTime = 4f;

    private Animator animator;
    private bool miraDreta = true; // true = derecha, false = izquierda

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", false);
        StartCoroutine(WanderRoutine());
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            // --- IDLE ---
            animator.SetBool("isWalking", false);
            yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));

            // Decide dirección (-1 izquierda, 1 derecha)
            int direction = Random.value < 0.5f ? -1 : 1;

            // Girar solo si hace falta
            if ((direction == 1 && !miraDreta) || (direction == -1 && miraDreta))
            {
                Girar();
            }

            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + Vector3.right * direction * moveDistance;

            // --- WALK ---
            animator.SetBool("isWalking", true);

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    speed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetPos;
            animator.SetBool("isWalking", false);
        }
    }

    private void Girar()
    {
        miraDreta = !miraDreta;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}
