using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] public Slider hungerBar;
    [SerializeField] public TextMeshProUGUI scoreMesh;
    [SerializeField] public Animator animator;
    [SerializeField] public ParticleSystem run;
    [SerializeField] public static float hungerScore;
    [SerializeField] public static int score;
    public int isGassed;
    public float hungerMin;
    public static float decreaseSpeed;
    public Camera cam;
    public float initialFOV = 90f;
    public float maxFOV = 135f;
    public float duration = 0.5f;
    public GameObject thisLost, gamebar, settings;
    public static bool lost = false;
    public static int coins;
    public Animator danger;
    public static bool isSpeed;

    public static bool GetLost()
    {
        return lost;
    }

    public static int GetScore()
    {
        return score;
    }

    public int GetGassed()
    {
        return isGassed;
    }

    void Start()
    {
        isSpeed = false;
        isGassed = 0;
        coins = 0;
        decreaseSpeed = 0.0004f;
        lost = false;
        hungerBar = GetComponent<Slider>();
        hungerScore = hungerBar.value;
        score = 0;
    }

    public static void CheckScore()
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        Rigidbody rb = playerMovement.rb;

        if (GetScore() >= 250 && GetScore() < 500)
        {
            decreaseSpeed = 0.00042f;
        }
        if (GetScore() >= 500 && GetScore() < 750)
        {
            decreaseSpeed = 0.00046f;
        }
        if (GetScore() >= 750 && GetScore() < 1000)
        {
            decreaseSpeed = 0.00058f;
        }
        else if (GetScore() >= 1000 && GetScore() < 1500)
        {
            decreaseSpeed = 0.00063f;
        }
        else if (GetScore() >= 1500 && GetScore() < 2000)
        {
            decreaseSpeed = 0.00076f;
        }
        else if (GetScore() >= 2000 && GetScore() < 2500)
        {   
            decreaseSpeed = 0.0008f;
        }
        else if (GetScore() >= 2500 && GetScore() < 3000)
        {
            decreaseSpeed = 0.0009f;
        }
        else if (GetScore() >= 3000)
        {
            decreaseSpeed = 0.0012f;
        }

        Debug.Log(decreaseSpeed);
    }
    
    void Update()
    {
        if (GetGassed() == 0)
        {
            hungerScore -= decreaseSpeed;
            hungerBar.value = hungerScore;
        }
        else
        {
            hungerBar.value = hungerScore;
        }

        if (hungerBar.value <= 0f)
        {
            hungerBar.fillRect.gameObject.SetActive(false);
            hungerScore = 0;
        }
        if (hungerBar.value <= 0.5f && hungerBar.value > 0)
        {
            danger.SetBool("inDanger", true);
        }
        else
        {
            danger.SetBool("inDanger", false);
        }
        scoreMesh.text = score.ToString();

        if (hungerScore == 0)
        {
            lost = true;
            thisLost.SetActive(true);
            gamebar.SetActive(false);
            settings.SetActive(false);
        }
    }

    public static void GetPoints(GameObject other)
    {
       
        ScoreManager instance = FindObjectOfType<ScoreManager>();
        if (!isSpeed)
        {
            instance.StartCoroutine(instance.SpeedEffect());
            isSpeed = false;
        }
        if (other.gameObject.CompareTag("High"))
        {
            hungerScore += 0.35f;
            score += 100;
        }
        else if (other.gameObject.CompareTag("Mid"))
        {
            hungerScore += 0.15f;
            score += 50;
        }
        else if (other.gameObject.CompareTag("Low") || other.gameObject.CompareTag("pizzaSlice"))
        {
            hungerScore += 0.05f;
            score += 25;
        }

        if (hungerScore > 1)
        {
            hungerScore = 1;
        }
        CheckScore();

    }

    public static void SetCoins()
    {
        coins++;
    }

    public void SetGassed(int status)
    {
        isGassed = status;
    }

    public static int GetCoins()
    {
        return coins;
    }

    private IEnumerator SpeedEffect()
    {
        float startTime = Time.time;
        float endTime = startTime + duration;
        run.Play();
        while (Time.time < endTime)
        {
            isSpeed = true;
            float t = (Time.time - startTime) / duration;
            float currentFOV = Mathf.Lerp(initialFOV, maxFOV, t);
            if (animator.gameObject.activeSelf)
            {
                animator.speed = 5f;
            }
            Camera.main.fieldOfView = currentFOV;
            yield return null;
        }

        startTime = Time.time;
        endTime = startTime + 0.25f;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / 0.25f;
            float currentFOV = Mathf.Lerp(maxFOV, initialFOV, t);
            Camera.main.fieldOfView = currentFOV;
            if (GetScore() < 2500)
            {
                if (animator.gameObject.activeSelf)
                {
                    animator.speed = 1;
                }
            }
            else
            {
                if (animator.gameObject.activeSelf)
                {
                    animator.speed = 2;
                }
            }
            isSpeed = false;
            yield return null;
        }
        Camera.main.fieldOfView = initialFOV;
        isSpeed = false;
        run.Stop();
    }
}
