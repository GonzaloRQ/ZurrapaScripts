using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform objectToFollow;

    private void LateUpdate()
    {
        transform.position = objectToFollow.position;
    }
}
