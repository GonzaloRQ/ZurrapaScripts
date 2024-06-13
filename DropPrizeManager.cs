using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.UI;

public class DropPrizeManager : MonoBehaviour
{

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private string userId;

    private int dropOpen;
    private int cAmmounts = 0;

    Dictionary<string, object> powerups = new Dictionary<string, object>();
    private int coins;
    private int rand1, rand2, rand3, rand4, rand5, randSkin;

    public Sprite[] coinsSprite;
    public Image[] pwImg;
    public Image prizeImg;
    private GameObject currentModel;
    public Item[] skins;
    public Item[] powerUpSprite;
    public GameObject modeloContainer; // GameObject para contener el modelo
    public TextMeshProUGUI amount, quees;
    public Animator qualityAnim;
    private bool animationFinished = true;
    private bool skinOwned;

    // Start is called before the first frame update
    async void Start()
    {
        Debug.ClearDeveloperConsole();
        auth = FirebaseAuth.DefaultInstance;
        userId = auth.CurrentUser.UserId;
        db = FirebaseFirestore.DefaultInstance;
        randSkin = GenerateSkinRandom();
        Debug.Log(userId);
        Debug.Log("SKIN NUMERO: " + randSkin);

        await CheckSkinDuplicated(skins[randSkin]);

        LoadPw();
        await LoadDropsStatus();

        Debug.Log(powerups.Values);
        rand1 = GenerateRandomNumber();
        rand2 = GenerateRandomNumber();
        rand3 = GenerateRandomNumber();
        rand4 = GenerateRandomNumber();
        rand5 = GenerateRandomNumber();

        // Recuperar la calidad final desde PlayerPrefs
        string finalQuality = PlayerPrefs.GetString("FinalQuality", "Special");

        // Obtener un valor aleatorio del 1 al 100
        int randomNumber = UnityEngine.Random.Range(1, 101);
        int randPrize;

        // Determinar qué animación reproducir basándose en la calidad final y el valor aleatorio
        if (finalQuality == "Special")
        {
            if (randomNumber <= 60)
            {
                pwImg[0].sprite = powerUpSprite[rand1].Sprite2D_UI;
                UpdatePowerUp(powerUpSprite[rand1]);

                quees.text = "Potenciadores";
                amount.text = "x1";
                qualityAnim.Play("SpecialPrizePower");

            }
            else
            {
                prizeImg.sprite = coinsSprite[0];
                quees.text = "Monedas";
                amount.text = "x5";
                cAmmounts = 5;
                qualityAnim.Play("SpecialPrizeCoins");

            }
        }
        else if (finalQuality == "Rare")
        {
            if (randomNumber <= 50)
            {
                pwImg[1].sprite = powerUpSprite[rand1].Sprite2D_UI;
                pwImg[2].sprite = powerUpSprite[rand2].Sprite2D_UI;
                pwImg[3].sprite = powerUpSprite[rand3].Sprite2D_UI;

                UpdatePowerUp(powerUpSprite[rand1]);
                UpdatePowerUp(powerUpSprite[rand2]);
                UpdatePowerUp(powerUpSprite[rand3]);
                quees.text = "Potenciadores";
                amount.text = "x3";
                qualityAnim.Play("RarePrizePower");
            }
            else
            {
                prizeImg.sprite = coinsSprite[1];
                quees.text = "Monedas";
                amount.text = "x10";
                cAmmounts = 10;
                qualityAnim.Play("RarePrizeCoins");
            }
        }
        else if (finalQuality == "Epic")
        {
            if (randomNumber <= 80)
            {
                pwImg[4].sprite = powerUpSprite[rand1].Sprite2D_UI;
                pwImg[5].sprite = powerUpSprite[rand2].Sprite2D_UI;
                pwImg[6].sprite = powerUpSprite[rand3].Sprite2D_UI;
                pwImg[7].sprite = powerUpSprite[rand4].Sprite2D_UI;

                UpdatePowerUp(powerUpSprite[rand1]);
                UpdatePowerUp(powerUpSprite[rand2]);
                UpdatePowerUp(powerUpSprite[rand3]);
                UpdatePowerUp(powerUpSprite[rand4]);

                quees.text = "Potenciadores";
                amount.text = "x4";
                qualityAnim.Play("EpicPrizesPower");
            }
            else if (randomNumber <= 99)
            {
                prizeImg.sprite = coinsSprite[2];
                quees.text = "Monedas";
                amount.text = "x25";
                cAmmounts = 25;
                qualityAnim.Play("EpicPrizesCoins");
            }
            else
            {
                if (skinOwned)
                {
                    prizeImg.sprite = coinsSprite[3];
                    quees.text = "Monedas";
                    amount.text = "x40";
                    cAmmounts = 40;
                    qualityAnim.Play("LegendaryPrizeCoin");
                    Debug.Log("TENIA SKIN");
                }
                else
                {
                    quees.text = "";
                    amount.text = "";
                    if (skins[randSkin].Modelo != null)
                    {
                        currentModel = Instantiate(skins[randSkin].Modelo, modeloContainer.transform.position, modeloContainer.transform.rotation, modeloContainer.transform);

                    }
                    UpdateSkin(skins[randSkin]);
                    StartCoroutine(PlaySkinShowerAnimation());
                }
            }
        }
        else if (finalQuality == "Legendary")
        {
            if (randomNumber <= 60)
            {
                pwImg[8].sprite = powerUpSprite[rand1].Sprite2D_UI;
                pwImg[9].sprite = powerUpSprite[rand2].Sprite2D_UI;
                pwImg[10].sprite = powerUpSprite[rand3].Sprite2D_UI;
                pwImg[11].sprite = powerUpSprite[rand4].Sprite2D_UI;
                pwImg[12].sprite = powerUpSprite[rand5].Sprite2D_UI;

                UpdatePowerUp(powerUpSprite[rand1]);
                UpdatePowerUp(powerUpSprite[rand2]);
                UpdatePowerUp(powerUpSprite[rand3]);
                UpdatePowerUp(powerUpSprite[rand4]);
                UpdatePowerUp(powerUpSprite[rand5]);

                quees.text = "Potenciadores";
                amount.text = "x5";
                qualityAnim.Play("LegendaryPrizePower");
            }
            else if (randomNumber <= 80)
            {
                prizeImg.sprite = coinsSprite[3];
                quees.text = "Monedas";
                amount.text = "x40";
                cAmmounts = 40;
                qualityAnim.Play("LegendaryPrizeCoin");


            }
            else
            {

                if (skinOwned)
                {
                    prizeImg.sprite = coinsSprite[3];
                    quees.text = "Monedas";
                    amount.text = "x40";
                    cAmmounts = 40;
                    Debug.Log("TENIA SKIN");
                    qualityAnim.Play("LegendaryPrizeCoin");
                }
                else
                {
                    quees.text = "";
                    amount.text = "";
                    if (skins[randSkin].Modelo != null)
                    {
                        currentModel = Instantiate(skins[randSkin].Modelo, modeloContainer.transform.position, modeloContainer.transform.rotation, modeloContainer.transform);

                    }
                    UpdateSkin(skins[randSkin]);
                    StartCoroutine(PlaySkinShowerAnimation());
                }

            }
        }
        else
        {
            Debug.LogError("La calidad final no es válida.");
        }

        UpdateCoins(cAmmounts);
    }

    IEnumerator PlaySkinShowerAnimation()
    {
        qualityAnim.Play("SkinsShower");
        animationFinished = false;
        yield return new WaitForSeconds(5f); // Espera a que acabe la animación
        animationFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (animationFinished && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(DelayBeforeSceneChange());
        }
    }
    IEnumerator DelayBeforeSceneChange()
    {
        yield return new WaitForSeconds(0.4f); // Espera 0.4 segundos
        CheckAndLoadScene();
    }
    async void CheckAndLoadScene()
    {
        await LoadDropsStatus();
        if (dropOpen > 0)
        {
            SceneManager.LoadScene("Drops");
        }
        else
        {
            SceneManager.LoadScene("Menu 1");
        }
    }

    int GenerateRandomNumber()
    {
        // Crear una instancia de la clase Random
        System.Random rnd = new System.Random();

        // Generar un número aleatorio en el rango de 0 a 2 (incluyendo el 0 pero excluyendo el 3)
        int randomNumber = rnd.Next(0, 6);

        return randomNumber;
    }

    int GenerateSkinRandom()
    {
        // Crear una instancia de la clase Random
        System.Random rnd = new System.Random();

        // Generar un número aleatorio en el rango de 0 a 2 (incluyendo el 0 pero excluyendo el 3)
        int randomNumber = rnd.Next(0, 2);

        return randomNumber;
    }

    async Task CheckSkinDuplicated(Item skin)
    {
        DocumentReference docRef = db.Collection("users").Document(userId);
        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                // Obtener el mapa de coleccionables del usuario
                Dictionary<string, object> userData = snapshot.ToDictionary();
                if (userData.ContainsKey("skins"))
                {
                    Dictionary<string, object> collectablesMap = (Dictionary<string, object>)userData["skins"];

                    if (collectablesMap.ContainsKey(skin.Id.ToString()))
                    {
                        bool checkSkinBool = (bool)collectablesMap[skin.Id.ToString()];
                        Debug.Log("Tiene la skin : " + checkSkinBool);
                        skinOwned = checkSkinBool;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al obtener el documento del usuario: " + e);
        }
    }

    async void LoadPw()
    {
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
                    // Obtener el mapa de power-ups de la base de datos
                    Dictionary<string, object> powerupsData = userData["powerups"] as Dictionary<string, object>;

                    // Si el mapa de powerups de la base de datos es nulo, que inicialice como un nuevo diccionario
                    if (powerupsData == null)
                    {
                        powerupsData = new Dictionary<string, object>();
                    }

                    // Agregar todos los powerups al diccionario local incluso si su valor es cero (reincorporar)
                    foreach (var entry in powerupsData)
                    {
                        string key = entry.Key;
                        object value = entry.Value;

                        if (!powerups.ContainsKey(key))
                        {
                            powerups.Add(key, value);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al obtener el documento del usuario: " + e);
        }
    }

    async Task LoadDropsStatus()
    {
        try
        {
            DocumentSnapshot snapshot = await db.Collection("users").Document(userId).GetSnapshotAsync();

            if (snapshot.Exists)
            {
                dropOpen = snapshot.GetValue<int>("dropsToOpen");
                Debug.Log(dropOpen);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading user data: " + ex.Message);
        }
    }

    async void UpdateSkin(Item skin)
    {
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
                    if (collectablesMap.ContainsKey(skin.Id.ToString()))
                    {
                        // Actualizar el estado del ítem correspondiente al índice
                        collectablesMap[skin.Id.ToString()] = true;

                        // Actualizar el documento en Firestore
                        await db.Collection("users")
                            .Document(auth.CurrentUser.UserId)
                            .UpdateAsync(new Dictionary<string, object> { { "skins", collectablesMap } });

                        Debug.Log("Item marcado como coleccionable: " + skin.Id.ToString());
                    }
                }
            }
        }
    }

    async void UpdateCoins(int ammount)
    {
        DocumentReference docRef = db.Collection("users").Document(userId);
        try
        {
            DocumentSnapshot snapshot = await db.Collection("users").Document(userId).GetSnapshotAsync();

            if (snapshot.Exists)
            {
                int actualCoins = snapshot.GetValue<int>("coins");
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "coins", actualCoins + ammount }
                };
                await docRef.UpdateAsync(updates);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error updating user data: " + ex.Message);
        }
    }

    async void UpdatePowerUp(Item item)
    {
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
                    if (powerups != null && powerups.ContainsKey(item.Id.ToString()))
                    {
                        // Incrementar el contador del powerup correspondiente
                        int currentCount = Convert.ToInt32(powerups[item.Id.ToString()]);

                        Debug.Log("se tiene: " + currentCount);
                        powerups[item.Id.ToString()] = currentCount + 1;

                        Debug.Log(item.Id + ": " + powerups[item.Id.ToString()]);
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
                    Debug.Log("Powerup" + item.Id.ToString() + "actualizado correctamente con el valor: " + powerups[item.Id.ToString()]);
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
}
