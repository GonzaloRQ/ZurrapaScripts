using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{

    public Item[] skins;
    public void getSkin()
    {
        // Desactivar todos los skins
        foreach (Item skin in skins)
        {
            skin.gameObject.SetActive(false);

            if (skin.Id == PlayerPrefs.GetInt("currentSkin", 0))
            {
                skin.gameObject.SetActive(true);
            }
        }
    }
}
