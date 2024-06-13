using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    [SerializeField] public GameObject eat;
    void OnTriggerEnter(Collider other) //Al entrar en contacto con un trigger.
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            this.gameObject.SetActive(false);
            ScoreManager.GetPoints(this.gameObject);
        }
    }
}
