using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LogController : MonoBehaviour
{
    [SerializeField]
    private bool _enableLogging = true;

    private void Awake()
    {
        Debug.unityLogger.logEnabled = _enableLogging;
    }
}
