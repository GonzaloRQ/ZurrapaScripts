using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicsController : MonoBehaviour
{
    public PlayableDirector timeline1;
    // Start is called before the first frame update
    void Start()
    {
        timeline1.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
