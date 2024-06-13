using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform[] waypoints;
    int waypointIndex;
    Vector3 tarjet;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        UpdateDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position,tarjet) < 1)
        {
            IterateWayPointIndex();
            UpdateDestination();
        }
    }
    void UpdateDestination()
    {
        tarjet = waypoints[waypointIndex].position;
        agent.SetDestination(tarjet);
    }

    void IterateWayPointIndex()
    {
        waypointIndex++;
        if(waypointIndex == waypoints.Length) 
        {
            waypointIndex = 0;
        }
    }
}
