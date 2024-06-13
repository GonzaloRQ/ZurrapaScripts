using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("Firebase")]
    public Firebase.DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    // Variables de inicio de sesi�n
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;

    // Variables de registro
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    private FirebaseFirestore db;

    void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;

        // Verifica que todas las dependencias necesarias para Firebase est�n presentes en el sistema
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Si est�n disponibles, inicializa Firebase
                InitializeFirebase();
                if (auth.CurrentUser.UserId != null)
                {
                    SceneManager.LoadScene("Menu 1");
                }
            }
            else
            {
                Debug.LogError("No se pudieron resolver todas las dependencias de Firebase: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Configurando Firebase Auth");
        // Establece el objeto de instancia de autenticaci�n
        auth = FirebaseAuth.DefaultInstance;
    }

    // Funci�n para el bot�n de inicio de sesi�n
    public void LoginButton()
    {
        // Llama a la corrutina de inicio de sesi�n pasando el email y la contrase�a
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    // Funci�n para el bot�n de registro
    public void RegisterButton()
    {
        // Llama a la corrutina de registro pasando el email, la contrase�a y el nombre de usuario
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator Login(string _email, string _password)
    {
        // Llama a la funci�n de inicio de sesi�n de Firebase Auth pasando el email y la contrase�a
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        // Espera hasta que la tarea se complete
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            // Si hay errores, manejarlos
            Debug.LogWarning(message: $"Error en la tarea de inicio de sesi�n: {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Inicio de sesi�n fallido!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Falta el email";
                    break;
                case AuthError.MissingPassword:
                    message = "Falta la contrase�a";
                    break;
                case AuthError.WrongPassword:
                    message = "Contrase�a incorrecta";
                    break;
                case AuthError.InvalidEmail:
                    message = "Email inv�lido";
                    break;
                case AuthError.UserNotFound:
                    message = "La cuenta no existe";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            // El usuario ha iniciado sesi�n
            // Ahora obtener el resultado
            user = LoginTask.Result.User;
            Debug.LogFormat("Usuario ha iniciado sesi�n exitosamente: {0} ({1})", user.DisplayName, user.Email);
            warningLoginText.text = "";
            SceneManager.LoadScene("Menu 1");
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            // Si el campo de nombre de usuario est� vac�o, mostrar una advertencia
            warningRegisterText.text = "Falta el nombre de usuario";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            // Si las contrase�as no coinciden, mostrar una advertencia
            warningRegisterText.text = "Las contrase�as no coinciden!";
        }
        else
        {
            // Llama a la funci�n de registro de Firebase Auth pasando el email y la contrase�a
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            // Espera hasta que la tarea se complete
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                // Si hay errores, manejarlos
                Debug.LogWarning(message: $"Error en la tarea de registro: {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Registro fallido!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Falta el email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Falta la contrase�a";
                        break;
                    case AuthError.WeakPassword:
                        message = "Contrase�a d�bil";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "El email ya est� en uso";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                // El usuario ha sido creado
                // Ahora obtener el resultado
                user = RegisterTask.Result.User;

                if (user != null)
                {
                    // Crear un perfil de usuario y establecer el nombre de usuario
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    // Llama a la funci�n de Firebase Auth para actualizar el perfil de usuario pasando el perfil con el nombre de usuario
                    Task ProfileTask = user.UpdateUserProfileAsync(profile);
                    // Espera hasta que la tarea se complete
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        // Si hay errores, manejarlos
                        Debug.LogWarning(message: $"Error en la tarea de actualizaci�n del perfil: {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Fallo al establecer el nombre de usuario!";
                    }
                    else
                    {
                        // El nombre de usuario ha sido establecido
                        // Ahora regresar a la pantalla de inicio de sesi�n

                        createUser(_email, _username);
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }

    void createUser(string _email, string _username)
    {
        DocumentReference docRef = db.Collection("users").Document(user.UserId);

        // Generar un ID �nico
        string uniqueId = "#" + GenerateRandomString(9);

        Dictionary<string, object> userData = new Dictionary<string, object>
                        {
                            { "id", uniqueId },
                            { "email", _email },
                            { "username", _username },
                            { "coins", 0 },
                            { "highscore", 0 },
                            { "todayScore", 0 },
                            { "todayDrop", 0 },
                            { "totalDrop", 0 },
                            { "dropsToOpen", 0 },
                            { "powerups", new Dictionary<string, int>
                            {
                                { "0", 0 },
                                { "1", 0 },
                                { "2", 0 }
                            }
                            },
                            { "collectables", new Dictionary<string, bool>
                                {
                                    { "0", false },
                                    { "1", false },
                                    { "2", false },
                                    { "3", false },
                                    { "4", false },
                                    { "5", false },
                                    { "6", false },
                                    { "7", false },
                                    { "8", false },
                                    { "9", false },
                                    { "10", false }
                                }
                            },
                            { "skins", new Dictionary<string, bool>
                                {
                                    { "0", true },
                                    { "1", false },
                                    { "2", false },
                                    { "3", false },
                                    { "4", false }
                                }
                            }
                        };

        docRef.SetAsync(userData).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al guardar datos en Firestore: " + task.Exception);
            }
            else
            {
                Debug.Log("Datos guardados en Firestore correctamente.");
            }
        });
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Excluyendo 'O' y 'L'
        var random = new System.Random(); // Especificar namespace completo
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
