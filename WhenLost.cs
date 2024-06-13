using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class WhenLost : MonoBehaviour
{
    public GameObject thisLost;

    private void Start()
    {
       thisLost.SetActive(false);
    }

    public void VolverAMenu()
    {
       
        SceneManager.LoadScene("Menu 1");

    }
    public void VolverAJugar()
    {
        SceneManager.LoadScene("Checker");
    }

}
