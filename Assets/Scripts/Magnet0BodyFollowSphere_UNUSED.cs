using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet0BodyFollowSphere : MonoBehaviour
{
    public Transform WalkingSphere;

    // Update is called once per frame
    void Update()
    {
        var walkingSpherePosition = WalkingSphere.transform.position;
        transform.position = new Vector3(walkingSpherePosition.x, walkingSpherePosition.y - (1.879114f * 0.05f), walkingSpherePosition.z);
    }
}
