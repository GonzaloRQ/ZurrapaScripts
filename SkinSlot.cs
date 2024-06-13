using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinSlot : MonoBehaviour
{
    public CanvasMenu canvasMenu;
    public Item[] items; // Array de Items que asignarás a cada slot
    public Button[] slots; // Array de botones (slots)
    public Button noneButton; // Botón de nada
    public Image selected; // Imagen para indicar el slot seleccionado
    public TMP_Text descripcionText; // Texto para la descripción del ítem
    FirebaseAuth auth;
    FirebaseFirestore db;

    private int selectedItemId = 0; // ID del ítem seleccionado

    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            // Si el usuario está autenticado, llamar a checkItems
            CheckItems();
        }
        else
        {
            Debug.LogError("No se puede inicializar Firebase: ");
        }
        CheckCurrentSkin();
        descripcionText.text = "";
        AssignItemsToSlots();
        noneButton.onClick.AddListener(OnNoneButtonClicked); // Añadir listener para el botón noneButton
    }

    void AssignItemsToSlots()
    {
        if (items.Length != slots.Length)
        {
            Debug.LogError("El número de items no coincide con el número de slots.");
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            int index = i; // Capturamos el valor actual de i
            slots[i].onClick.AddListener(() => OnSlotClicked(index)); // Asignamos el evento onClick a cada botón

            // Cambiamos la imagen del botón al sprite del item correspondiente
            Image buttonImage = slots[i].GetComponent<Image>();
            buttonImage.sprite = items[i].Sprite2D_UI;
        }
    }

    void OnSlotClicked(int index)
    {  
        descripcionText.text = items[index].Nombre + ":\n" + items[index].Descripcion;
        selected.transform.position = slots[index].transform.position;
        selectedItemId = items[index].Id; // Guardar el ID del ítem seleccionado
        PlayerPrefs.SetInt("currentSkin", selectedItemId); // Guardar el ID del ítem seleccionado en PlayerPrefs
        PlayerPrefs.Save(); // Asegurarse de que PlayerPrefs se guarde
        Debug.Log(PlayerPrefs.GetInt("currentSkin").ToString());       
    }

    async void CheckItems()
    {
        // Verificar el usuario actualmente autenticado
        if (auth.CurrentUser != null)
        {
            // Obtener el documento del usuario actual
            DocumentSnapshot snapshot = await db.Collection("users")
                .Document(auth.CurrentUser.UserId)
                .GetSnapshotAsync();

            if (snapshot.Exists)
            {
                // Obtener el mapa de coleccionables del usuario
                Dictionary<string, object> userData = snapshot.ToDictionary();
                if (userData.ContainsKey("skins"))
                {
                    Dictionary<string, object> collectablesMap = (Dictionary<string, object>)userData["skins"];

                    for (int i = 0; i < items.Length; i++)
                    {
                        // Verificar si el mapa de coleccionables contiene una entrada para el índice actual
                        if (collectablesMap.ContainsKey(items[i].Id.ToString()))
                        {
                            // Obtener el valor booleano correspondiente al índice i
                            bool isCollectable = (bool)collectablesMap[items[i].Id.ToString()];

                            // Actualizar el estado del slot correspondiente
                            if (!isCollectable && slots.Length > i)
                            {
                                // Si el elemento no es coleccionable, hacer el slot no interactuable
                                slots[i].interactable = false;
                            }
                        }
                    }
                }
            }
        }
    }
    void OnNoneButtonClicked()
    {
        selected.transform.position = noneButton.transform.position;
        Debug.Log(PlayerPrefs.GetInt("currentSkin").ToString());
        PlayerPrefs.SetInt("currentSkin", 0); 
        PlayerPrefs.Save(); // Asegurarse de que PlayerPrefs se guarde
        descripcionText.text = "No seleccionar nada.";
    }

    void CheckCurrentSkin()
    {
        int currentSkin = PlayerPrefs.GetInt("currentSkin", 0);
        for(int i = 0; i < items.Length; i++) 
        {
            if (items[i].Id == currentSkin)
            {
                selected.transform.position = slots[i].transform.position;
            }
        }
    }
}
