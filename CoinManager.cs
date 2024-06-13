using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public ParticleSystem coinCollected;
    public static int coins;
    void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            coins++;
            ScoreManager.SetCoins();
            this.gameObject.SetActive(false);
          
        }
    }

    public static int getCoins()
    {
        return coins;
    }
}
