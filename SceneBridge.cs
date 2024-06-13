using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBridge : MonoBehaviour
{
    IEnumerator Start()
    {
        // Espera un segundo
        yield return new WaitForSeconds(1f);

        // Carga la escena "Game"
        SceneManager.LoadScene("Game");
    }
}
