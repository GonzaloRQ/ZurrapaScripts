using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;

public class SlotManager : MonoBehaviour
{
    public Item[] items; // Array de Items que asignarás a cada slot
    public AmountLoader amountLoader;
    public Button[] slots; // Array de botones (slots)
    public Button[] otherButtons = new Button[2]; // Array de botones (slots)
    public TMP_Text nombreText; // TextMeshPro para mostrar el nombre
    public TMP_Text rarezaText; // TextMeshPro para mostrar la descripción
    public TMP_Text descripcionText; // TextMeshPro para mostrar la descripción
    public TMP_Text precioText; // TextMeshPro para mostrar la descripción
    public Button buyButton;
    public Image powerUpImg;
    public Sprite none;
    public GameObject parentObject; // GameObject para el modelo
    public GameObject modeloContainer; // GameObject para contener el modelo
    [SerializeField] public vendorsVoiceLines voiceLines;
    private GameObject currentModel;

    // Firebase variables
    FirebaseAuth auth;
    FirebaseFirestore db;


    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        nombreText.text = "";
        descripcionText.text = "";
        rarezaText.text = "";
        precioText.text = "";
        AssignItemsToSlots();
        powerUpImg.sprite = none;
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
        voiceLines.sayDesc(items[index].Descripcion, items[index].DescripcionIng);
        // Mostrar el nombre y la descripción del item clickeado
        nombreText.text = items[index].Nombre;
        descripcionText.text = items[index].Type;
        rarezaText.text = "";
        precioText.text = items[index].Prize.ToString() + "c";
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => OnBuyButtonClicked(index));
        // Destruir el modelo actual si existe

        /*
        Transform t = modeloContainer.transform;
            for(int i = 0; i < t.childCount; i++) 
            {
                Destroy(t.GetChild(i).gameObject);
            }
        */
        // Crear el modelo del item en la misma posición y con la misma rotación y escala que modeloContainer
        if (items[index].Modelo == null)
        {
            powerUpImg.sprite = items[index].Sprite2D_UI;
          
        }
    }
    async void OnBuyButtonClicked(int index)
    {
        bool success = await amountLoader.ReduceMoney(items[index].Prize);

        if (success)
        {
            voiceLines.sayThx("Gracias, coleguita...", "Thanks, lil' man...");
            // Verificar los items
            BuySlot(items[index]);
            powerUpImg.GetComponent<Animator>().Play("itembuy");
        }
        else
        {
            voiceLines.sayDesc("te falta guita, chaval...", "Ya ain't got enough cash, kid...");
            powerUpImg.GetComponent<Animator>().Play("itemcantbuy");
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

    async void BuySlot(Item item)
    {
        // Verificar el usuario actualmente autenticado
        if (auth.CurrentUser != null)
        {
            string userId = auth.CurrentUser.UserId;
            // Obtener el documento del usuario actual
            DocumentReference docRef = db.Collection("users").Document(userId);
            try
            {
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    // Obtener los powerups del documento
                    Dictionary<string, object> userData = snapshot.ToDictionary();
                    if (userData.ContainsKey("powerups"))
                    {
                        Dictionary<string, object> powerups = userData["powerups"] as Dictionary<string, object>;

                        if (powerups != null && powerups.ContainsKey(item.Id.ToString()))
                        {
                            // Incrementar el contador del powerup correspondiente
                            int currentCount = Convert.ToInt32(powerups[item.Id.ToString()]);
                            powerups[item.Id.ToString()] = currentCount + 1;
                        }
                        else
                        {
                            // Si el powerup no existe, agregarlo con valor 1
                            powerups[item.Id.ToString()] = 1;
                        }
                        // Actualizar el documento del usuario con el nuevo valor del powerup
                        Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "powerups", powerups }
                    };
                        await docRef.UpdateAsync(updates);
                        Debug.Log("Powerup actualizado correctamente");
                    }
                    else
                    {
                        Debug.LogError("El campo 'powerups' no existe en el documento del usuario.");
                    }
                }
                else
                {
                    Debug.LogError("El documento del usuario no existe.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error al obtener el documento del usuario: " + e);
            }
        }
        else
        {
            Debug.LogError("No hay usuario autenticado.");
        }
    }

}
