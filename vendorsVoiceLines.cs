using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using System.Globalization;

public class vendorsVoiceLines : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public RawImage speechBubble1;
    public RawImage speechBubble2;

    // Diálogos en español
    private List<string> spanishDialogues = new List<string>();

    // Diálogos en inglés u otro idioma
    private List<string> englishDialogues = new List<string>();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSource2;
    [SerializeField] private AudioClip clip;
    [SerializeField] private Animator vendorSittingAnim;
    private bool isSpeaking;
    private string currentDialogue;
    private float letterSpeed = 0.03f; // Tiempo de espera entre letras
    private int lastDialogueIndex = -1; // Índice del último diálogo reproducido

    IEnumerator Start()
    {
        // Cargar los diálogos desde el archivo CSV
        yield return LoadDialoguesFromCSV();

        audioSource = GetComponent<AudioSource>();
        isSpeaking = false;
        textMesh.gameObject.SetActive(false);
        speechBubble1.gameObject.SetActive(false);
        speechBubble2.gameObject.SetActive(false);
        Invoke("PlaySecondAudio", 1.5f);
        InvokeRepeating("StartSpeaking", 5.4f, Random.Range(10f, 30f)); // Comienza a hablar después de 5 segundos y repite cada 10-30 segundos
    }

    IEnumerator LoadDialoguesFromCSV()
    {
        // Obtener el idioma del dispositivo
        string deviceLanguage = Application.systemLanguage.ToString();

        // Construir la ruta al archivo CSV en la carpeta Resources
        string filePath = "dialogues";

        // Cargar el archivo CSV desde la carpeta Resources
        TextAsset csvFile = Resources.Load<TextAsset>(filePath);

        // Leer el contenido del archivo CSV
        string csvText = csvFile.text;

        // Dividir el texto del archivo CSV en líneas
        string[] lines = csvText.Split('\n');

        foreach (string line in lines)
        {
            // Dividir la línea en columnas
            string[] columns = line.Split(';');

            // Agregar el diálogo en español a la lista
            spanishDialogues.Add(columns[0].Trim());
            Debug.Log("Current line: " + line);
            // Agregar el diálogo en inglés u otro idioma a la lista
            englishDialogues.Add(columns[1].Trim());
        }

        // Determinar qué diálogos utilizar según el idioma del dispositivo
        List<string> dialoguesToUse = (deviceLanguage == "Spanish") ? spanishDialogues : englishDialogues;


        yield return null; // Opcional, simplemente para cumplir con IEnumerator
    }


    void StartSpeaking()
    {
        if (!isSpeaking) // Verificar si el personaje no está hablando actualmente
        {
            isSpeaking = true;
            int randomIndex;
            List<string> dialoguesToUse = (Application.systemLanguage == SystemLanguage.Spanish) ? spanishDialogues : englishDialogues;
            do
            {
                randomIndex = Random.Range(0, dialoguesToUse.Count);
            } while (randomIndex == lastDialogueIndex); // Repetir si el nuevo índice es igual al último índice
            lastDialogueIndex = randomIndex; // Actualizar el último índice reproducido
            currentDialogue = dialoguesToUse[randomIndex];
            textMesh.text = "";
            textMesh.gameObject.SetActive(true);
            speechBubble1.gameObject.SetActive(true);
            speechBubble2.gameObject.SetActive(true);
            StartCoroutine(DisplayDialogue());
        }
    }


    IEnumerator DisplayDialogue()
    {
        foreach (char currentChar in currentDialogue)
        {
            // Reproducir el sonido en el momento de escribir el caracter, excepto para coma y punto
            if (currentChar != ',' && currentChar != '.')
            {
                PlayRandomSound();
            }

            if (currentChar == '?')
            {
                PlayQuestionSound();
                yield return new WaitForSeconds(0.3f);
            }

            textMesh.text += currentChar;
            yield return new WaitForSeconds(letterSpeed);

            if (currentChar == ',' || currentChar == '.')
            {
                yield return new WaitForSeconds((currentChar == ',') ? 0.2f : 0.3f); // Pausa después de coma o punto
            }
        }

        yield return new WaitForSeconds(5f); // Tiempo que el texto permanece visible
        EndSpeaking();
    }

    IEnumerator DisplayDesc(string text)
    {
        foreach (char currentChar in text)
        {
            // Reproducir el sonido en el momento de escribir el caracter, excepto para coma y punto
            if (currentChar != ',' && currentChar != '.')
            {
                PlayRandomSound();
            }

            if (currentChar == '?')
            {
                PlayQuestionSound();
                yield return new WaitForSeconds(0.15f);
            }

            textMesh.text += currentChar;
            yield return new WaitForSeconds(letterSpeed-0.02f);

            if (currentChar == ',' || currentChar == '.')
            {
                yield return new WaitForSeconds((currentChar == ',') ? 0.1f : 0.15f); // Pausa después de coma o punto
            }
        }

        yield return new WaitForSeconds(15f); // Tiempo que el texto permanece visible
        EndSpeaking();
    }


    void PlayRandomSound()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(clip);
    }

    void PlayQuestionSound()
    {
        audioSource.pitch = 1.3f;
        audioSource.PlayOneShot(clip);
    }

    void EndSpeaking()
    {
        isSpeaking = false;
        textMesh.gameObject.SetActive(false);
        speechBubble1.gameObject.SetActive(false);
        speechBubble2.gameObject.SetActive(false);
    }

    void PlaySecondAudio()
    {
        // Reproduce el segundo audio
        audioSource2.Play();
    }

    public void sayDesc(string esText, string engText)
    {
        StopAllCoroutines();
        EndSpeaking();
        StopAllCoroutines();
        textMesh.text = "";
        audioSource.Stop();

        if (!isSpeaking) // Verificar si el personaje no está hablando actualmente
        {
            isSpeaking = true;
            int randomIndex;
            string lang = (Application.systemLanguage == SystemLanguage.Spanish) ? esText : engText;
            textMesh.text = "";
            textMesh.gameObject.SetActive(true);
            speechBubble1.gameObject.SetActive(true);
            speechBubble2.gameObject.SetActive(true);
            StartCoroutine(DisplayDesc(lang));
        }

    }

    public void sayThx(string esText, string engText)
    {
        StopAllCoroutines();
        EndSpeaking();
        StopAllCoroutines();
        textMesh.text = "";
        audioSource.Stop();

        if (!isSpeaking) // Verificar si el personaje no está hablando actualmente
        {
            vendorSittingAnim.Play("vendorThx");
            isSpeaking = true;
            int randomIndex;
            string lang = (Application.systemLanguage == SystemLanguage.Spanish) ? esText : engText;
            textMesh.text = "";
            textMesh.gameObject.SetActive(true);
            speechBubble1.gameObject.SetActive(true);
            speechBubble2.gameObject.SetActive(true);
            StartCoroutine(DisplayDesc(lang));
        }

    }
    public void ClickSpeak()
    {
        StopAllCoroutines();
        EndSpeaking();
        StopAllCoroutines();
        textMesh.text = "";
        audioSource.Stop();
        StartSpeaking();
    }
}
