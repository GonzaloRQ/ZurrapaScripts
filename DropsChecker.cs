using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DropsChecker : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private string userId;
    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        userId = auth.CurrentUser.UserId;
        db = FirebaseFirestore.DefaultInstance;
        Checker();
    }

    async void Checker()
    {
        DocumentReference docRef = db.Collection("users").Document(userId);
        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                int dropsToOpen = snapshot.GetValue<int>("dropsToOpen");
                int totalDrops = snapshot.GetValue<int>("totalDrop");

                if (dropsToOpen <= 0)
                {
                    SceneManager.LoadScene("Menu 1");
                }
                else
                {
                    SceneManager.LoadScene("Drops");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al actualizar los datos del usuario: " + ex.Message);
        }
    }
}
