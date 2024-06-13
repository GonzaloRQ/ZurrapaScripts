using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopSwitcher : MonoBehaviour
{
    [SerializeField] public GameObject powerUpSlot, skinsSlot, collectiblesSlot;
    // Start is called before the first frame update
    public void ShowPowerUps()
    {
        powerUpSlot.SetActive(true);
        skinsSlot.SetActive(false);
        collectiblesSlot.SetActive(false);
    }
    public void ShowCollectibles()
    {
        powerUpSlot.SetActive(false);
        skinsSlot.SetActive(false);
        collectiblesSlot.SetActive(true);
    }

    public void ShowSkins()
    {
        powerUpSlot.SetActive(false);
        skinsSlot.SetActive(true);
        collectiblesSlot.SetActive(false);
    }
}
