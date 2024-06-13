using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Auth;
using System;

public class HighsCoreAnim : MonoBehaviour
{
    public Animator highScoreAnimator;
    [SerializeField] public TextMeshProUGUI highScoreText;
    [SerializeField] public TextMeshProUGUI coinsText;
    [SerializeField] public TextMeshProUGUI todayScoreText;
    [SerializeField] public Slider scoreSlider;
    [SerializeField] public Animator drop1, drop2,drop3;

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private string userId;
    private int currentCoins;
    private int currentScore;
    private int currentHighScore;
    private int todayScore;
    private int todayDrop;
    private int totalDrop;

    // Start is called before the first frame update
    void OnEnable()
    {
        auth = FirebaseAuth.DefaultInstance;
        userId = auth.CurrentUser.UserId;
        db = FirebaseFirestore.DefaultInstance;
        Debug.Log(userId);
        LoadUserData();
       
        UpdateScores();
    }

    // Update is called once per frame
    void Update()
    {
        highScoreText.text = PlayerPrefs.GetFloat("HighScore", ScoreManager.GetScore()).ToString();


    }

    async void LoadUserData()
    {
        try
        {
            DocumentSnapshot snapshot = await db.Collection("users").Document(userId).GetSnapshotAsync();

            if (snapshot.Exists)
            {
                currentHighScore = snapshot.GetValue<int>("highscore");
                todayScore = snapshot.GetValue<int>("todayScore");
                currentCoins = snapshot.GetValue<int>("coins");
                todayDrop = snapshot.GetValue<int>("todayDrop");
                Debug.Log(todayDrop);
                totalDrop = snapshot.GetValue<int>("dropsToOpen");
                Debug.Log(todayScore + "\n" + currentHighScore);

                if(ScoreManager.GetScore() > currentHighScore)
                {
                    PlayerPrefs.SetFloat("HighScore", ScoreManager.GetScore());
                }
                else
                {
                    PlayerPrefs.SetFloat("HighScore", currentHighScore);
                }
                scoreSlider.value = todayScore;

            
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading user data: " + ex.Message);
        }
    }


    public void UpdateScores()
    {
        currentScore = ScoreManager.GetScore();
        Debug.Log(currentScore);
        StartCoroutine(UpdateTodayScore());
        coinsText.text = "x" + ScoreManager.GetCoins().ToString();

        SaveUserData();
    }

   

    IEnumerator UpdateTodayScore()
    {
      
        yield return new WaitForSeconds(1.5f);

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            int intValue = Mathf.RoundToInt(Mathf.Lerp(todayScore, todayScore + ScoreManager.GetScore(), t));
            todayScoreText.text = intValue.ToString();
            scoreSlider.value = intValue;

            if (todayDrop >= 3)
            {
                drop1.gameObject.SetActive(false);
                drop2.gameObject.SetActive(false);
                drop3.gameObject.SetActive(false);
            }
            if (todayDrop < 3)
            {
                if (scoreSlider.value >= 5000)
                {
                    drop1.Play("dropOut");
                }
                if (scoreSlider.value >= 10000)
                {
                    drop2.Play("dropOut");
                }
                if (scoreSlider.value >= 15000)
                {
                    drop3.Play("dropOut");
                }
            }
            yield return null;
        }
        Debug.Log(todayScore+ScoreManager.GetScore());
        todayScoreText.text = Mathf.RoundToInt(todayScore + ScoreManager.GetScore()).ToString();
        scoreSlider.value = Mathf.RoundToInt(todayScore + ScoreManager.GetScore());

       
    }

    public int CheckDropIncrements()
    {
        if (todayDrop < 3)
        {
            if (todayDrop == 0 && (todayScore + ScoreManager.GetScore()) >= 5000)
            {
               
                todayDrop++;
                totalDrop++;
            }
            if (todayDrop == 1 && (todayScore + ScoreManager.GetScore()) >= 10000)
            {

                todayDrop++;
                totalDrop++;
            }
            if (todayDrop == 2 && (todayScore + ScoreManager.GetScore()) >= 15000)
            {

                todayDrop++;
                totalDrop++;
            }
        }
        Debug.Log (todayDrop);
        return todayDrop;
    }

    async void SaveUserData()
    {
        try
        {
            DocumentSnapshot snapshot = await db.Collection("users").Document(userId).GetSnapshotAsync();

            if (snapshot.Exists)
            {
                int dbHighScore = snapshot.GetValue<int>("highscore");
                Debug.Log(dbHighScore);

               if(currentHighScore > (int)PlayerPrefs.GetFloat("HighScore", ScoreManager.GetScore()))
               {
                    highScoreAnimator.Play("HighScore");     
               }

                Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "highscore", (int)PlayerPrefs.GetFloat("HighScore",ScoreManager.GetScore()) },
                { "todayScore", todayScore + ScoreManager.GetScore() },
                { "coins", currentCoins + ScoreManager.GetCoins() },
                { "todayDrop", CheckDropIncrements() },
                { "dropsToOpen", totalDrop }
            };

                await db.Collection("users").Document(userId).UpdateAsync(userData);
                Debug.Log("User data updated successfully." + currentHighScore + todayScore);
            }
            else
            {
               
                Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "highscore",  (int)PlayerPrefs.GetFloat("HighScore",ScoreManager.GetScore()) },
                { "todayScore", todayScore },
                { "coins", currentCoins + ScoreManager.GetCoins() },
                { "todayDrop", todayDrop },
                { "dropsToOpen", totalDrop }
            };

                await db.Collection("users").Document(userId).SetAsync(userData);
                Debug.Log("User data created successfully." + currentHighScore + todayScore);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error updating user data: " + ex.Message);
        }
    }


}
