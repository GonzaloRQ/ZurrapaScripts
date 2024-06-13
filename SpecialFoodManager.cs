using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialFoodManager : MonoBehaviour
{
    float velocidadJugador;
    float velocidadOriginal;
    public ParticleSystem coinCollected;
    public int pizzaCount;
    public float tiempoRestanteBoost = 0f;
    public float duracionBoost = 10f;
    public Animator animator;
    public PlayerMovement pm;
    public new GameObject light;
    public GameObject particle;
    public GameObject pizzaroll, rollParty;
    public GameObject roll1, roll2, roll3, roll4, roll5, roll6;
    public bool caseGass, spiced;
    public GameObject pizzaTime;
    float velocidadAnimacion;
    [SerializeField] public Image slider;
    public Sprite originalFill;
    [SerializeField] public Sprite gassImg;
    [SerializeField] public ScoreManager scoreManager;

    private void Start()
    {
        caseGass = false;
        spiced = false;
        rollParty.SetActive(false);
        pizzaTime.SetActive(false);
        pizzaroll.SetActive(false);
        light.SetActive(false);
        particle.SetActive(false);
        pizzaCount = -1;
        originalFill = slider.sprite;
    }

    private void Update()
    {
        if (tiempoRestanteBoost > 0f)
        {
            tiempoRestanteBoost -= Time.deltaTime;
            if (tiempoRestanteBoost <= 0f)
            {
                if (!caseGass && spiced)
                {
                    pm.SetSpeed(velocidadOriginal); // Restaurar la velocidad original
                    light.SetActive(false);
                    particle.SetActive(false);
                    animator.speed /= 2f;
                    spiced = false;
                }
            }
            else
            {
                if (caseGass)
                {
                    slider.sprite = gassImg;
                }
            }
        }
        else
        {
            if(caseGass)
            {
                // Restaura el color original de la barra de relleno
                slider.sprite = originalFill;
                scoreManager.isGassed = 0;
                caseGass = false;
            }

           
            
        }
    }

    void OnTriggerEnter(Collider other) // Al entrar en contacto con un trigger.
    {
        if (other.gameObject.CompareTag("spicy") && !spiced && !caseGass)
        {
            if (tiempoRestanteBoost <= 0f)
            {
                velocidadOriginal = pm.GetSpeed(); // Guardar la velocidad original
                pm.SetSpeed(velocidadOriginal * 2f);
                tiempoRestanteBoost = duracionBoost;
                light.SetActive(true);
                particle.SetActive(true);
                spiced = true;
            }
        }
        if (other.gameObject.CompareTag("gass") && !caseGass && !spiced)
        {
            tiempoRestanteBoost = duracionBoost;
            caseGass = true;
            scoreManager.isGassed = 1;
            Debug.Log(scoreManager.GetGassed());
            Debug.Log("La barra de relleno cambió de color al entrar en contacto con 'gass'.");
        }
        if (other.gameObject.CompareTag("Coin"))
        {
            coinCollected.Play();
        }
        if (other.gameObject.CompareTag("pizzaSlice"))
        {
            if (!pizzaTime.activeSelf)
            {
                pizzaroll.SetActive(false);
                roll1.SetActive(false);
                roll2.SetActive(false);
                roll3.SetActive(false);
                roll4.SetActive(false);
                roll5.SetActive(false);
                roll6.SetActive(false);
                rollParty.SetActive(false);
                pizzaCount++;
                if (pizzaCount == 0)
                {
                    pizzaroll.SetActive(true);
                    roll1.SetActive(true);
                }
                else if (pizzaCount == 1)
                {
                    pizzaroll.SetActive(true);
                    roll2.SetActive(true);
                }
                else if (pizzaCount == 2)
                {
                    pizzaroll.SetActive(true);
                    roll3.SetActive(true);
                }
                else if (pizzaCount == 3)
                {
                    pizzaroll.SetActive(true);
                    roll4.SetActive(true);
                }
                else if (pizzaCount == 4)
                {
                    pizzaroll.SetActive(true);
                    roll5.SetActive(true);
                }
                else if (pizzaCount == 5)
                {
                    rollParty.SetActive(true);
                    pizzaTime.SetActive(true);
                    pizzaCount = -1;
                }
            }
        }
    }
}
