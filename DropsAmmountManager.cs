using Firebase.Auth;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DropsAmmountManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore firestore;
    public GameObject ammountbck;
    public TextMeshProUGUI ammount;
    // Start is called before the first frame update

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;
    
        if (auth.CurrentUser != null)
        {
            GetDropData();
            Debug.Log(auth.CurrentUser.UserId);
        }
        else
        {
            Debug.LogWarning("No hay usuario autenticado.");
        }
    }

    // Obtener datos de drops
    async void GetDropData()
    {
        try
        {
            DocumentSnapshot snapshot = await firestore.Collection("users").Document(auth.CurrentUser.UserId).GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();
                
                if(snapshot.GetValue<int>("dropsToOpen") > 0)
                {
                    this.gameObject.GetComponent<Button>().interactable = true;
                    ammountbck.SetActive(true);
                    ammount.text = snapshot.GetValue<int>("dropsToOpen").ToString();
                }
            }
            else
            {
                Debug.LogWarning("El documento del usuario no existe.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al obtener los datos del usuario: " + e);
        }
    }


    
}
