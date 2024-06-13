using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Reflection;

public class CollectSlotManager : MonoBehaviour
{
    public Item[] items; // Array de Items que asignarás a cada slot
    public AmountLoader amountLoader;
    public Button buyButton;
    public Button[] otherButtons = new Button[2];
    public Button[] slots; // Array de botones (slots)
    public TMP_Text nombreText; // TextMeshPro para mostrar el nombre
    public TMP_Text descripcionText; // TextMeshPro para mostrar la descripción
    public TMP_Text rarezaText; // TextMeshPro para mostrar la descripción
    public TMP_Text precioText; // TextMeshPro para mostrar la descripción
    public GameObject parentObject; // GameObject para el modelo
    public GameObject modeloContainer; // GameObject para contener el modelo
    public Image imgSlot;
    public Sprite noneSprite;
    [SerializeField] public vendorsVoiceLines voiceLines;
    private GameObject currentModel;
    public bool isAuthenticated = false; // Flag para verificar la autenticación del usuario

    // Firebase variables
    FirebaseAuth auth;
    FirebaseFirestore db;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            
          // Si el usuario está autenticado, llamar a checkItems
          isAuthenticated = true;
          CheckItems();
                
        }
        else
        {
          Debug.LogError("No se puede inicializar Firebase: ");
        }
        
        nombreText.text = "";
        descripcionText.text = "";
        rarezaText.text = "";
        precioText.text = "";
        AssignItemsToSlots();
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
        HideAll();
        buyButton.gameObject.SetActive(true);

        // Mostrar el nombre y la descripción del item clickeado
        nombreText.text = items[index].Nombre;
        descripcionText.text = items[index].Type;
        rarezaText.text = "";
        precioText.text = items[index].Prize.ToString() + "c";
        voiceLines.sayDesc(items[index].Descripcion, items[index].DescripcionIng);
        
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => OnBuyButtonClicked(index));

        // Destruir el modelo actual si existe
        imgSlot.sprite = noneSprite;
        


        

        // Crear el modelo del item en la misma posición y con la misma rotación y escala que modeloContainer
        if (items[index].Modelo != null && parentObject != null && modeloContainer != null)
        {
            currentModel = Instantiate(items[index].Modelo, parentObject.transform.position, modeloContainer.transform.rotation, modeloContainer.transform);
          
        }
    }
    async void OnBuyButtonClicked(int index)
    {
        bool success = await amountLoader.ReduceMoney(items[index].Prize);

        if (success)
        {
            voiceLines.sayThx("Gracias, coleguita...", "Thanks, lil' man...");
            HideAll();

            // Verificar los items
            UpdateItems(index); 
        }
        else
        {
            voiceLines.sayDesc("te falta guita, chaval...", "Ya ain't got enough cash, kid...");
          
        }
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
                if (userData.ContainsKey("collectables"))
                {
                    Dictionary<string, object> collectablesMap = (Dictionary<string, object>)userData["collectables"];

                    for (int i = 0; i < items.Length; i++)
                    {
                        Debug.Log(items[i].Id.ToString());

                        // Verificar si el mapa de coleccionables contiene una entrada para el índice actual
                        if (collectablesMap.ContainsKey(items[i].Id.ToString()))
                        {
                            // Obtener el valor booleano correspondiente al índice i
                            bool isCollectable = (bool)collectablesMap[items[i].Id.ToString()];
                            
                            // Actualizar el estado del slot correspondiente
                            if (isCollectable && slots.Length > i)
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

    void HideAll()
    {
        otherButtons[0].gameObject.SetActive(false);
        otherButtons[1].gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
        nombreText.text = "";
        descripcionText.text = "";
        rarezaText.text = "";
        precioText.text = "";
        Transform t = modeloContainer.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            GameObject.Destroy(t.GetChild(i).gameObject);
        }
    }

    async void UpdateItems(int index)
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
                if (userData.ContainsKey("collectables"))
                {
                    Dictionary<string, object> collectablesMap = (Dictionary<string, object>)userData["collectables"];

                    // Verificar si el mapa de coleccionables contiene una entrada para el índice actual
                    if (collectablesMap.ContainsKey(items[index].Id.ToString()))
                    {
                        // Actualizar el estado del ítem correspondiente al índice
                        collectablesMap[items[index].Id.ToString()] = true;

                        // Actualizar el documento en Firestore
                        await db.Collection("users")
                            .Document(auth.CurrentUser.UserId)
                            .UpdateAsync(new Dictionary<string, object> { { "collectables", collectablesMap } });

                        Debug.Log("Item marcado como coleccionable: " + items[index].Id.ToString());
                        CheckItems();

                    }
                }
            }
        }
    }


}
