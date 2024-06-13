using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpMenuSlot : MonoBehaviour
{
    public CanvasMenu canvasMenu;
    public Item[] items; // Array de Items que asignarás a cada slot
    public Button[] slots; // Array de botones (slots)
    public TMP_Text[] values; // Array de textos para mostrar cantidades
    public Button noneButton; // Botón de nada
    public Image selected; // Imagen para indicar el slot seleccionado
    public TMP_Text descripcionText; // Texto para la descripción del ítem
    FirebaseAuth auth;
    FirebaseFirestore db;

    private int selectedItemId = -1; // ID del ítem seleccionado

    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            // Si el usuario está autenticado, llamar a checkItems
            checkItems();
        }
        else
        {
            Debug.LogError("No se puede inicializar Firebase: ");
        }
       
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
        PlayerPrefs.SetInt("currentPw", selectedItemId); // Guardar el ID del ítem seleccionado en PlayerPrefs
        PlayerPrefs.Save(); // Asegurarse de que PlayerPrefs se guarde
        Debug.Log(PlayerPrefs.GetInt("currentPw").ToString());
        canvasMenu.LoadPw();
    }

    async void checkItems()
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
                if (userData.ContainsKey("powerups"))
                {
                    Dictionary<string, object> collectablesMap = (Dictionary<string, object>)userData["powerups"];

                    for (int i = 0; i < items.Length; i++)
                    {
                        string itemId = items[i].Id.ToString();
                        // Verificar si el mapa de coleccionables contiene una entrada para el índice actual
                        if (collectablesMap.ContainsKey(itemId))
                        {
                            // Obtener el valor correspondiente al índice i
                            int value = System.Convert.ToInt32(collectablesMap[itemId]);
                            values[i].text = "x" + value.ToString();
                            // Actualizar el estado del slot correspondiente
                            if (value == 0 && slots.Length > i)
                            {
                                // Si el valor es 0, hacer el slot no interactuable
                                slots[i].interactable = false;
                                values[i].text = "";
                            }
                        }
                        else
                        {
                            // Si no existe la entrada, también hacemos el slot no interactuable
                            slots[i].interactable = false;
                        }
                    }
                }
            }
        }
    }
  
    void OnNoneButtonClicked()
    {
        selected.transform.position = noneButton.transform.position;
        Debug.Log(PlayerPrefs.GetInt("currentPw").ToString());
        PlayerPrefs.SetInt("currentPw", -1); // Establecer PlayerPrefs a -1
        PlayerPrefs.Save(); // Asegurarse de que PlayerPrefs se guarde
        descripcionText.text = "No seleccionar nada.";
        canvasMenu.LoadPw();
    }
}
