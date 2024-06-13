using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasInGame: MonoBehaviour
{
    
    public GameObject settings, HungerBar, data, lost;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        data.SetActive(false);
        HungerBar.SetActive(true);
        settings.SetActive(false);
        anim = GetComponent<Animator>();
    }
    public void VolverAMenu()
    {
        
        SceneManager.LoadScene("Menu 1");
    }
    public void MostrarSettings()
    {
        
        HungerBar.SetActive(false);
        settings.SetActive(true);
        anim.SetBool("Opciones" , true);
    }
    public void MostrarData()
    {
        lost.SetActive(false);
        HungerBar.SetActive(false);
        data.SetActive(true);
    }
    public void Regresar()
    {
       
        settings.SetActive(false);
        HungerBar.SetActive(true);
    }
    public void RegresarGlobal(GameObject go)
    {
       go.SetActive(!go.active);
    }



}
