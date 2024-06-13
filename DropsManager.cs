using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;
using Firebase.Auth;
using Firebase.Firestore;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;
using UnityEngine.PlayerLoop;

public class DropsManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private string userId;

    [SerializeField] private Animator animator;
    [SerializeField] private Animator a_QualityAnimator;
    [SerializeField] private Animator a_CamAnimator;

    [SerializeField] private Sprite sp_specialImageBG;
    [SerializeField] private Sprite sp_rareImageBG;
    [SerializeField] private Sprite sp_epicImageBG;
    [SerializeField] private Sprite sp_legendaryImageBG;
    [SerializeField] private Sprite sp_specialImageTxt;
    [SerializeField] private Sprite sp_rareImageTxt;
    [SerializeField] private Sprite sp_epicImageTxt;
    [SerializeField] private Sprite sp_legendaryImageTxt;
    [SerializeField] private Sprite sp_tryLost;

    [SerializeField] private Image img_canvasImage;
    [SerializeField] private Image img_qualityImage;
    [SerializeField] private Image[] img_tries;
    [SerializeField] private Image img_portal;

    [SerializeField] private RawImage r_img_qualityImageClr;

    [SerializeField] private Color c_specialColor;
    [SerializeField] private Color c_rareColor;
    [SerializeField] private Color c_epicColor;
    [SerializeField] private Color c_legendaryColor;
    [SerializeField] private Color c_specialPColor;
    [SerializeField] private Color c_rarePColor;
    [SerializeField] private Color c_epicPColor;
    [SerializeField] private Color c_legendaryPColor;

    [SerializeField] private GameObject go_particles;
    [SerializeField] private Light l_light;

    [SerializeField] private AudioSource a_touch;
    [SerializeField] private AudioSource a_open;
    [SerializeField] private AudioSource a_quality;
    [SerializeField] private AudioSource a_wowSound;

    [SerializeField] private ParticleSystem ps_qualityWow;
    [SerializeField] private ParticleSystem ps_touchSparkle;
    [SerializeField] private ParticleSystem ps_legendary;

    [SerializeField] private GameObject realUI;
    [SerializeField] private GameObject tries;
    [SerializeField] private GameObject golden;
    [SerializeField] private GameObject normal1, normal2;




    private string currentQuality = "Special";
    private int touchesRemaining = 5;
    private int triesRemaining = 0;
    private int touchCount = 0;


    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        userId = auth.CurrentUser.UserId;
        db = FirebaseFirestore.DefaultInstance;

        ReduceDropsRemaining();

        golden.SetActive(false);
        ps_legendary.Stop();
        go_particles.SetActive(false);
        animator.SetBool("touchable", false);
        a_QualityAnimator.SetBool("newQuality", false);
        animator.SetBool("dropOpen", false);
        // Establecer la imagen inicial en el canvas
        UpdateCanvasImage(currentQuality);
    }

    void Update()
    {


        if (touchesRemaining >= 0 && Input.GetMouseButtonDown(0))
        {

            touchesRemaining--;

            if (triesRemaining < 4)
            {
                img_tries[triesRemaining].sprite = sp_tryLost;
                triesRemaining++;
            }

            if (touchesRemaining > 0)
            {

                a_touch.Play();
                string newQuality = CalculateNewQuality();

                //  newQuality = "Special";
                // newQuality = "Rare";
                // newQuality = "Epic";
                // newQuality = "Legendary";

                if (newQuality != currentQuality)
                {
                    touchCount++;
                    currentQuality = newQuality;
                    a_QualityAnimator.Play("newQuality");
                    a_CamAnimator.SetInteger("touchVal", touchCount);
                }
                else
                {
                    a_QualityAnimator.Play("sparklebgAnimation");
                }
                if (newQuality != "Legendary")
                {
                    animator.SetBool("touchable", false);
                    //  animator.SetBool("touchable", true);
                    animator.Play("droptouch");

                    ps_touchSparkle.Play();
                }
                else
                {
                    tries.SetActive(false);
                    triesRemaining = 5;
                    animator.Play("dropLegendary");
                    a_CamAnimator.SetInteger("touchVal", 4);
                    a_QualityAnimator.Play("legendarybgAnimation");
                    normal1.SetActive(false);
                    normal2.SetActive(false);
                    golden.SetActive(true);
                    ps_legendary.Play();
                }

                if (newQuality == "Epic" || newQuality == "Legendary")
                {
                    go_particles.SetActive(true);
                }



                UpdateCanvasImage(newQuality);
                UpdateQualityImageColor(newQuality); // Actualizar el color de la imagen según la calidad


            }

            if (touchesRemaining <= 1)
            {
                a_CamAnimator.SetInteger("touchVal", 4);
                tries.SetActive(false);
            }

            if (touchesRemaining == 0 && Input.GetMouseButtonDown(0))
            {
                realUI.SetActive(false);
                animator.Play("dropOpen");
                a_QualityAnimator.Play("openbgAnimations");
                a_quality.Stop();
                a_open.Play();
                StartCoroutine(Again());
                Debug.Log("Se acabaron los toques. No se puede hacer nada más.");
            }
        }

    }

    string CalculateNewQuality()
    {
        int rand = UnityEngine.Random.Range(0, 251);
        Debug.Log(rand);
        switch (currentQuality)
        {
            case "Special":
                if (rand <= 185)
                {
                    a_quality.pitch = 1;
                    a_wowSound.pitch = 1;
                    return "Special";
                }
                else if (rand > 185 && rand < 245)
                {
                    a_quality.pitch = 1.2f;
                    a_wowSound.pitch = 1.2f;
                    ps_qualityWow.Play();
                    a_wowSound.Play();
                    return "Rare";
                }
                else if (rand >= 245 && rand <= 249)
                {
                    a_quality.pitch = 1.3f;
                    a_wowSound.pitch = 1.3f;
                    ps_qualityWow.Play();
                    a_wowSound.Play();
                    return "Epic";
                }
                else
                {
                    a_quality.pitch = 1.4f;
                    a_wowSound.pitch = 1.4f;
                    ps_qualityWow.Play();
                    a_wowSound.Play();
                    return "Legendary";
                }

            case "Rare":
                if (rand <= 210)
                {
                    return "Rare";
                }
                else if (rand > 210 && rand < 250)
                {
                    a_quality.pitch = 1.3f;
                    a_wowSound.pitch = 1.3f;
                    ps_qualityWow.Play();
                    a_wowSound.Play();
                    return "Epic";
                }
                else
                {
                    a_quality.pitch = 1.4f;
                    a_wowSound.pitch = 1.4f;
                    ps_qualityWow.Play();
                    a_wowSound.Play();
                    return "Legendary";
                }

            case "Epic":
                if (rand <= 243)
                {
                    return "Epic";
                }
                else
                {
                    a_quality.pitch = 1.4f;
                    a_wowSound.pitch = 1.4f;
                    ps_qualityWow.Play();
                    a_wowSound.Play();
                    return "Legendary";
                }

            case "Legendary":
                return "Legendary";

            default:
                return currentQuality;
        }
    }


    void UpdateCanvasImage(string newQuality)
    {
        switch (newQuality)
        {
            case "Special":
                img_portal.color = c_specialPColor;
                img_qualityImage.sprite = sp_specialImageTxt;
                img_canvasImage.sprite = sp_specialImageBG;
                break;

            case "Rare":
                img_portal.color = c_rarePColor;
                img_qualityImage.sprite = sp_rareImageTxt;
                img_canvasImage.sprite = sp_rareImageBG;
                break;

            case "Epic":
                img_portal.color = c_epicPColor;
                img_qualityImage.sprite = sp_epicImageTxt;
                img_canvasImage.sprite = sp_epicImageBG;
                break;

            case "Legendary":
                img_portal.color = c_legendaryPColor;
                img_qualityImage.sprite = sp_legendaryImageTxt;
                touchesRemaining = 1;
                img_canvasImage.sprite = sp_legendaryImageBG;
                break;

            default:
                break;
        }

        currentQuality = newQuality;
    }

    void UpdateQualityImageColor(string quality)
    {
        switch (quality)
        {
            case "Special":
                l_light.color = c_specialPColor;
                r_img_qualityImageClr.color = c_specialColor;
                break;

            case "Rare":
                l_light.color = c_rarePColor;
                r_img_qualityImageClr.color = c_rareColor;
                break;

            case "Epic":
                l_light.color = c_epicPColor;
                r_img_qualityImageClr.color = c_epicColor;
                break;

            case "Legendary":
                l_light.color = c_legendaryPColor;
                r_img_qualityImageClr.color = c_legendaryColor;
                break;

            default:
                break;
        }
    }



    IEnumerator Again()
    {
        yield return new WaitForSeconds(1f);

        // Guardar la calidad final en PlayerPrefs para que esté disponible en la siguiente escena
        PlayerPrefs.SetString("FinalQuality", currentQuality);
        // Cargar la escena "DropPrize" y pasar la calidad final como un parámetro
        SceneManager.LoadScene("Prizes", LoadSceneMode.Single);


    }

    async void ReduceDropsRemaining()
    {
        DocumentReference docRef = db.Collection("users").Document(userId);
        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                int dropsToOpen = snapshot.GetValue<int>("dropsToOpen");
                int totalDrops = snapshot.GetValue<int>("totalDrop");

                    dropsToOpen--;
                    totalDrops++;
                
                // Decrementar dropsToOpen y aumentar totalDrops

                // Crear un diccionario con los nuevos valores
                Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "dropsToOpen", dropsToOpen },
                { "totalDrop", totalDrops }
            };

                // Actualizar el documento del usuario con los nuevos valores
                await docRef.UpdateAsync(userData);

                Debug.Log("Datos actualizados correctamente.");
            }
            else
            {
                Debug.LogWarning("El documento del usuario no existe.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al actualizar los datos del usuario: " + ex.Message);
        }
    }
}










