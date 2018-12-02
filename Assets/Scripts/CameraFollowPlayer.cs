using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform Player;
    public float FollowSpeed = 0.05f;
    public Vector3 Offset;

    public float minY;
    public float maxY;

    public Vector2 minPos;
    public Vector2 maxPos;
	
	// Update is called once per frame
	void Update()
    {
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Clamp(Player.position.x + Offset.x, minPos.x, maxPos.x);
        newPos.y = Mathf.Clamp(Player.position.y + Offset.y, minPos.y, maxPos.y);
        newPos = Vector3.Slerp(transform.position, newPos, FollowSpeed);

        transform.position = newPos;
	}
}
