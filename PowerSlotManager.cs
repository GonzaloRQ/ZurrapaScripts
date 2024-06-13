using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PowerSlotManager : MonoBehaviour
{
    public Item[] items; // Array de Items que asignarás a cada slot
    public Button[] slots; // Array de botones (slots)
    public TMP_Text nombreText; // TextMeshPro para mostrar el nombre
    public TMP_Text descripcionText; // TextMeshPro para mostrar la descripción
    public TMP_Text precioText; // TextMeshPro para mostrar la descripción
    public GameObject parentObject; // GameObject para el modelo
    public GameObject modeloContainer; // GameObject para contener el modelo

    private GameObject currentModel;

    void Start()
    {
        nombreText.text = "";
        descripcionText.text = "";
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
        // Mostrar el nombre y la descripción del item clickeado
        nombreText.text = items[index].Nombre;
        descripcionText.text = items[index].Descripcion;
        precioText.text = items[index].Prize.ToString() + "c";

        // Destruir el modelo actual si existe
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        // Crear el modelo del item en la misma posición y con la misma rotación y escala que modeloContainer
        if (items[index].Modelo != null && parentObject != null && modeloContainer != null)
        {
            currentModel = Instantiate(items[index].Modelo, parentObject.transform.position, modeloContainer.transform.rotation, modeloContainer.transform);
          
        }
    }

   
}
