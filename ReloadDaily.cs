using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;

public class ReloadDaily : MonoBehaviour
{
    /* ###########
     * #   API   #
     * ###########
     */
    #region Singleton class: WorldTimeAPI

    public static ReloadDaily Instance;
    private FirebaseFirestore db2;
    private FirebaseAuth auth;
    private string userId;

    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        userId = auth.CurrentUser.UserId;
        db2 = FirebaseFirestore.DefaultInstance;
        Debug.Log(userId);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #endregion

    // JSON container
    struct TimeData
    {
        public string datetime;
    }

    const string API_URL = "http://worldtimeapi.org/api/ip";
    private const string LastUpdateKey = "LastUpdateDate";

    [HideInInspector] public bool IsTimeLoaded = false;

    private DateTime _currentDateTime = DateTime.Now;
    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        StartCoroutine(GetRealDateTimeFromAPI());
    }

    public DateTime GetCurrentDateTime()
    {
        return _currentDateTime.AddSeconds(Time.realtimeSinceStartup);
    }

    IEnumerator GetRealDateTimeFromAPI()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(API_URL);
        Debug.Log("Getting real datetime...");

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("Error: " + webRequest.error);
        }
        else
        {
            TimeData timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);
            _currentDateTime = ParseDateTime(timeData.datetime);
            IsTimeLoaded = true;
            Debug.Log("Success.");

            // Comprueba si son las 10 a. m. o más tarde y aún no está actualizado hoy
            if (_currentDateTime.Hour >= 10 && !IsUpdatedToday())
            {
                UpdateUserFirestore(userId);
            }
        }

        DateTime ParseDateTime(string datetime)
        {
            string date = Regex.Match(datetime, @"^\d{4}-\d{2}-\d{2}").Value;
            string time = Regex.Match(datetime, @"\d{2}:\d{2}:\d{2}").Value;
            return DateTime.Parse(string.Format("{0} {1}", date, time));
        }

        bool IsUpdatedToday()
        {
            string lastUpdate = PlayerPrefs.GetString(LastUpdateKey, string.Empty);
            if (DateTime.TryParse(lastUpdate, out DateTime lastUpdateDate))
            {
                return lastUpdateDate.Date == _currentDateTime.Date;
            }
            return false;
        }

        void SetLastUpdateDate()
        {
            PlayerPrefs.SetString(LastUpdateKey, _currentDateTime.ToString());
            PlayerPrefs.Save();
        }

        void UpdateUserFirestore(string userId)
        {
            DocumentReference docRef = db.Collection("users").Document(userId);
            docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "todayDrop", 0 },
            { "todayScore", 0 }
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User fields updated successfully.");
                SetLastUpdateDate(); // Establece la fecha de la última actualización solo en caso de una actualización exitosa
            }
            else
            {
                Debug.LogError("Error updating user fields: " + task.Exception);
            }
        });
        }
    }
}
