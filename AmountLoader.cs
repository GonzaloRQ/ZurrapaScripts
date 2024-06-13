using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AmountLoader : MonoBehaviour
{
    private Animator moneyAnim;
    private FirebaseAuth auth;
    private FirebaseFirestore firestore;
    public TextMeshProUGUI amount;
    private int currentCoins;
    [SerializeField] private TextMeshProUGUI changer;
    // Start is called before the first frame update
    void Start()
    {
        moneyAnim = GetComponent<Animator>();
        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;
        if (auth.CurrentUser != null)
        {
            getData();
        }
        else
        {
            Debug.LogWarning("No hay usuario autenticado.");
        }
    }

    async void getData()
    {
        try
        {
            DocumentSnapshot snapshot = await firestore.Collection("users").Document(auth.CurrentUser.UserId).GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();

                if (userData.ContainsKey("coins"))
                {
                    currentCoins = Convert.ToInt32(userData["coins"]); // Convertir a entero
                    amount.text = currentCoins.ToString(); // Convertir a cadena y mostrar en TextMeshPro
                }
                else
                {
                    Debug.LogWarning("El documento del usuario no contiene el campo 'coins'.");
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

    public async Task<bool> ReduceMoney(int amountToReduce)
    {
        changer.text = "-"+amountToReduce.ToString();
        try
        {
            DocumentSnapshot snapshot = await firestore.Collection("users").Document(auth.CurrentUser.UserId).GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();

                if (userData.ContainsKey("coins"))
                {
                    int coins = Convert.ToInt32(userData["coins"]); // Convertir a entero

                    if (amountToReduce <= coins)
                    {
                        int newAmount = coins - amountToReduce; // Calcular la nueva cantidad de monedas
                                                                // Actualizar la cantidad de monedas en Firestore
                        Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "coins", newAmount }
                    };
                        await firestore.Collection("users").Document(auth.CurrentUser.UserId).UpdateAsync(updates);

                        // Iniciar la animación de reducción de monedas
                        StartCoroutine(AnimateCoinReduction(coins, newAmount));

                        return true; // La reducción de dinero se realizó con éxito
                    }
                    else
                    {
                        Debug.LogWarning("No tienes suficiente dinero para comprar.");
                        return false; // No hay suficiente dinero para comprar
                    }
                }
                else
                {
                    Debug.LogWarning("El documento del usuario no contiene el campo 'coins'.");
                    return false; // El campo 'coins' no existe en el documento del usuario
                }
            }
            else
            {
                Debug.LogWarning("El documento del usuario no existe.");
                return false; // El documento del usuario no existe
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al obtener los datos del usuario: " + e);
            return false; // Error al obtener los datos del usuario
        }
    }


    private IEnumerator AnimateCoinReduction(int startAmount, int endAmount)
    {
        moneyAnim.Play("moneyLoss");
        float duration = 1.0f; // Duración de la animación en segundos
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentAmount = (int)Mathf.Lerp(startAmount, endAmount, elapsed / duration);
            amount.text = currentAmount.ToString();
            yield return null;
        }

        // Asegurarse de que la cantidad final sea exacta
        amount.text = endAmount.ToString();
        currentCoins = endAmount;
        moneyAnim.Play("moneyLossReturn");
    }

    public async void increaseMoney(int amountToAdd)
    {
        try
        {
            DocumentSnapshot snapshot = await firestore.Collection("users").Document(auth.CurrentUser.UserId).GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();

                if (userData.ContainsKey("coins"))
                {
                    int coins = Convert.ToInt32(userData["coins"]); // Convertir a entero
                    int newAmount = coins + amountToAdd; // Calcular la nueva cantidad de monedas

                    if (newAmount > Int32.MaxValue)
                    {
                        newAmount = Int32.MaxValue;
                    }

                    // Actualizar la cantidad de monedas en Firestore
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "coins", newAmount }
                    };
                    await firestore.Collection("users").Document(auth.CurrentUser.UserId).UpdateAsync(updates);

                    // Iniciar la animación de incremento de monedas
                    StartCoroutine(AnimateCoinIncrease(coins, newAmount));
                }
                else
                {
                    Debug.LogWarning("El documento del usuario no contiene el campo 'coins'.");
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

    private IEnumerator AnimateCoinIncrease(int startAmount, int endAmount)
    {
        float duration = 1.0f; // Duración de la animación en segundos
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentAmount = (int)Mathf.Lerp(startAmount, endAmount, elapsed / duration);
            amount.text = currentAmount.ToString();
            yield return null;
        }

        // Asegurarse de que la cantidad final sea exacta
        amount.text = endAmount.ToString();
        currentCoins = endAmount;
    }
}
