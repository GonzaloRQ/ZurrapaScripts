using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOnMenu : MonoBehaviour
{
    public GameObject target; 
    public float velocidad = 5.0f;  

    private Vector3 giro;  

    void Start()
    {
        
        giro = transform.position - target.transform.position;
    }

    void LateUpdate()
    {
        // Calculamos la rotaci�n de la c�mara en funci�n del tiempo transcurrido y la velocidad
        float angle = Time.time * velocidad;

        // Calculamos la nueva posici�n de la c�mara en base a la rotaci�n y la distancia inicial
        Vector3 newPosition = target.transform.position + Quaternion.Euler(0, angle, 0) * giro;

        // Movemos la c�mara a la nueva posici�n
        transform.position = newPosition;

        // Hacemos que la c�mara mire al objeto
        transform.LookAt(target.transform);
    }
}
