using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using System;

public class CanvasMenu : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI userText, coinsText;
    public GameObject settings, menu, pwCanvas, music, sfx, master;
    public Animator sfxAnim, musicAnim, masterAnim, returnButton, burgerAnim, glitchyAnim;
    public Item[] items;
    public SkinManager skinManager;
    public Sprite noneIm;
    public Image pw;
    public Button pwBtn, skinsBtn;
    public GameObject pwSlot, skinsSlot;
    private FirebaseAuth auth;
    private FirebaseFirestore firestore;
    private string currentUserUsername; // Variable para almacenar el nombre de usuario actual

    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;
        pwSlot.SetActive(false);
        skinsSlot.SetActive(false);
        // Verificar si el usuario está autenticado
        if (auth.CurrentUser != null)
        {
            GetUserData();
            Debug.Log(auth.CurrentUser.UserId);
        }
        else
        {
          
        }

        menu.SetActive(true);
        settings.SetActive(false);
        LoadPw();
        LoadSkin();
    }

    async void GetUserData()
    {
        try
        {
            DocumentSnapshot snapshot = await firestore.Collection("users").Document(auth.CurrentUser.UserId).GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();

                // Obtener y mostrar el nombre de usuario
                if (userData.ContainsKey("username"))
                {
                    userText.text = userData["username"].ToString();
                }
                        
            }
           
        }
        catch (Exception e)
        {
            Debug.LogError("Error al obtener los datos del usuario: " + e);
        }
    }

    public void LoadPw()
    {
        PlayerPrefs.GetInt("currentPw", -1);

        if(PlayerPrefs.GetInt("currentPw", -1) == -1)
        {
            pw.sprite = noneIm;
        }
        else
        {
            pw.sprite = items[PlayerPrefs.GetInt("currentPw")].Sprite2D_UI;
        }
    }

    // Cargar skin
    public void LoadSkin()
    {
        skinManager.getSkin();
    }

    public void DropButton()
    {
        SceneManager.LoadScene("Drops");
    }

    // Cargar los slot de Power ups
    public void LoadPwSlot()
    {
        if (pwSlot.activeSelf)
        {
            pwSlot.SetActive(false); 
        }
        else
        {
            pwSlot.SetActive(true);
        }

        if(skinsSlot.activeSelf)
        {
            skinsSlot.SetActive(false);
        }
    } 

    // Cargar slots de skins
    public void LoadSkinSlot()
    {
        if (skinsSlot.activeSelf)
        {
            skinsSlot.SetActive(false);
            skinManager.getSkin();
        }
        else
        {
            skinsSlot.SetActive(true);
        }

        if(pwSlot.activeSelf)
        {
            pwSlot.SetActive(false);
        }
    }

    // Cargar escena del juego
    public void PlayButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShopButton()
    {
        pwCanvas.SetActive(false);
        skinsSlot.SetActive(false);
        menu.SetActive(false);
        glitchyAnim.SetBool("Glitch", true);
        Invoke("LoadShopScene", 3f);
    }

    // Cargar tienda
    void LoadShopScene()
    {
        SceneManager.LoadScene("Shop");
    }

    // Mostrar ajustes
    public void MostrarSettings()
    {
        settings.SetActive(true);
        music.SetActive(true);
        sfx.SetActive(true);
        master.SetActive(true);
        sfxAnim.SetBool("Opciones" , true);
        returnButton.SetBool("Opciones" , true);
        musicAnim.SetBool("Opciones" , true);
        masterAnim.SetBool("Opciones" , true);
        menu.SetActive(false);
    }

    public void Regresar()
    {
        music.SetActive(false);
        sfx.SetActive(false);
        master.SetActive(false);
        sfxAnim.SetBool("Opciones", false);
        returnButton.SetBool("Opciones", false);
        musicAnim.SetBool("Opciones", false);
        masterAnim.SetBool("Opciones", false);
        burgerAnim.SetBool("exit", true);
        StartCoroutine(Pausa());

      
    }
    public void RegresarGlobal(GameObject go)
    {
       go.SetActive(!go.active);
    }

    private IEnumerator Pausa()
    {
        Debug.Log("Antes de la pausa");

        yield return new WaitForSeconds(1.5f); // Pausa durante 3 segundos
        
        settings.SetActive(false);
        menu.SetActive(true);
        Debug.Log("Después de la pausa");
    }

}
