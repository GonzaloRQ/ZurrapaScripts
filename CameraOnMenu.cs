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
        // Calculamos la rotación de la cámara en función del tiempo transcurrido y la velocidad
        float angle = Time.time * velocidad;

        // Calculamos la nueva posición de la cámara en base a la rotación y la distancia inicial
        Vector3 newPosition = target.transform.position + Quaternion.Euler(0, angle, 0) * giro;

        // Movemos la cámara a la nueva posición
        transform.position = newPosition;

        // Hacemos que la cámara mire al objeto
        transform.LookAt(target.transform);
    }
}
