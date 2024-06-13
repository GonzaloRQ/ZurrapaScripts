using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;

public class SkinsSlotManager : MonoBehaviour
{
    public Item[] items; // Array de Items que asignarás a cada slot
    public Button[] slots; // Array de botones (slots)
    public AmountLoader amountLoader;
    public Button[] otherButtons = new Button[2];
    public TMP_Text nombreText; // TextMeshPro para mostrar el nombre
    public TMP_Text descripcionText; // TextMeshPro para mostrar la descripción
    public TMP_Text rarezaText; // TextMeshPro para mostrar la descripción
    public TMP_Text precioText; // TextMeshPro para mostrar la descripción
    public Button buyButton;
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
    void OnSlotClicked(int index)
    {
        HideAll();
        otherButtons[0].gameObject.SetActive(false);
        otherButtons[1].gameObject.SetActive(false);
        buyButton.gameObject.SetActive(true);
        voiceLines.sayDesc(items[index].Descripcion, items[index].DescripcionIng);
        // Mostrar el nombre y la descripción del item clickeado
        nombreText.text = items[index].Nombre;
        descripcionText.text = items[index].Type;
        rarezaText.text = items[index].Rarity;
        precioText.text = items[index].Prize.ToString() + "c";
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => OnBuyButtonClicked(index));

        switch (items[index].Rarity)
        {
            case "Legendario":
                rarezaText.color = Color.yellow;
                break;

            case "Especial":
                rarezaText.color = Color.green;
                break;

            case "Raro":
                rarezaText.color = Color.blue;
                break;

            case "Epic":
                rarezaText.color = Color.magenta;
                break;

            default:
                rarezaText.color = Color.white;
                break;
        }

        // Destruir el modelo actual si existe
        imgSlot.sprite = noneSprite;
        Transform t = modeloContainer.transform;
            for (int i = 0; i < t.childCount; i++)
            {
                GameObject.Destroy(t.GetChild(i).gameObject);
            }

        

        // Crear el modelo del item en la misma posición y con la misma rotación y escala que modeloContainer
        if (items[index].Modelo != null && parentObject != null && modeloContainer != null)
        {
            currentModel = Instantiate(items[index].Modelo, parentObject.transform.position, modeloContainer.transform.rotation, modeloContainer.transform);
          
        }
    }
    void HideAll()
    {
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
                if (userData.ContainsKey("skins"))
                {
                    Dictionary<string, object> collectablesMap = (Dictionary<string, object>)userData["skins"];

                    // Verificar si el mapa de coleccionables contiene una entrada para el índice actual
                    if (collectablesMap.ContainsKey(items[index].Id.ToString()))
                    {
                        // Actualizar el estado del ítem correspondiente al índice
                        collectablesMap[items[index].Id.ToString()] = true;

                        // Actualizar el documento en Firestore
                        await db.Collection("users")
                            .Document(auth.CurrentUser.UserId)
                            .UpdateAsync(new Dictionary<string, object> { { "skins", collectablesMap } });

                        Debug.Log("Item marcado como coleccionable: " + items[index].Id.ToString());
                        CheckItems();

                    }
                }
            }
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

}
