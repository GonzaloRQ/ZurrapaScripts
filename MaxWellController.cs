using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxWellController : MonoBehaviour
{
    public GameObject maxwell;
    public Animator maxwellAnim;
    // Start is called before the first frame update
    void Start()
    {
        maxwell.SetActive(true);
        int valor = Random.Range(0, 50);
        maxwellAnim.SetBool("Found", false);
        if (valor == 25)
        {
            maxwell.SetActive(true);
        }
        else
        {
            maxwell.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            maxwellAnim.SetBool("Found", true);
        }
    }
}
