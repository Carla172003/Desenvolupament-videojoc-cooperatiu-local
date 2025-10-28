using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgafarObjecte : MonoBehaviour
{

    public GameObject puntAgafar;
    private GameObject objecteAgafat;
    private KeyCode specialKey;

    void Start()
    {
        assignarTecles();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (objecteAgafat != null)
        {
            if (Input.GetKey(specialKey))
            {
                objecteAgafat.GetComponent<Rigidbody2D>().gravityScale = 1;
                objecteAgafat.GetComponent<Rigidbody2D>().isKinematic = false;
                objecteAgafat.gameObject.transform.SetParent(null);
                objecteAgafat = null;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Objecte"))
        {
            if (Input.GetKey(specialKey) && objecteAgafat == null)
            {
                other.GetComponent<Rigidbody2D>().gravityScale = 0;
                other.GetComponent<Rigidbody2D>().isKinematic = true;
                other.gameObject.transform.position = puntAgafar.transform.position;
                other.gameObject.transform.SetParent(puntAgafar.gameObject.transform);
                objecteAgafat = other.gameObject;
            }
            
        }
    }

    private void assignarTecles() 
    {
        // Asignar tecles segons el tag
        if (CompareTag("Ma1"))
        {
            specialKey = KeyCode.E;
        }
        else if (CompareTag("Ma2"))
        {
            specialKey = KeyCode.O;
        }
    }
}
