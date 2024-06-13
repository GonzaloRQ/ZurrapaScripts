using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopCanvasManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject objectToDisable;
    public GameObject buyButton;
    [SerializeField] private Animator animator1;
    [SerializeField] private Animator animator2;

    // Start is called before the first frame update
    void Start()
    {
        buyButton.SetActive(false);
        // Generar un n�mero aleatorio entre 0 y 40 al inicio del juego
         int randomValue = Random.Range(0, 41);

         // Establecer el par�metro int del animator con el valor aleatorio generado
         animator.SetInteger("randomval", randomValue);
    }

    // Update is called once per frame
    public void MenuButton()
    {
        // Desactivar el GameObject
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }

        // Establecer la variable bool "out" en los dos animadores a true
        if (animator1 != null && animator2 != null)
        {
            animator1.Play("DoorShut");
        }

        // Llamar a la funci�n para cargar la escena despu�s de un retraso
        StartCoroutine(LoadMenuAfterAnimation());
    }

    IEnumerator LoadMenuAfterAnimation()
    {
        // Esperar hasta que la animaci�n del animator1 termine
        yield return new WaitForSeconds(0.8f);

        // Iniciar la animaci�n del animator2
        animator2.Play("shopExit");

        // Esperar hasta que la animaci�n del animator2 termine
        yield return new WaitForSeconds(3f);

        // Cargar la escena del men�
        SceneManager.LoadScene("Menu 1");
    }
}
