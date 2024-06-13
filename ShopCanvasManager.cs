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
        // Generar un número aleatorio entre 0 y 40 al inicio del juego
         int randomValue = Random.Range(0, 41);

         // Establecer el parámetro int del animator con el valor aleatorio generado
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

        // Llamar a la función para cargar la escena después de un retraso
        StartCoroutine(LoadMenuAfterAnimation());
    }

    IEnumerator LoadMenuAfterAnimation()
    {
        // Esperar hasta que la animación del animator1 termine
        yield return new WaitForSeconds(0.8f);

        // Iniciar la animación del animator2
        animator2.Play("shopExit");

        // Esperar hasta que la animación del animator2 termine
        yield return new WaitForSeconds(3f);

        // Cargar la escena del menú
        SceneManager.LoadScene("Menu 1");
    }
}
