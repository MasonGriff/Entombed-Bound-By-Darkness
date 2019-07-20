using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject platform;
    public float moveSpeed;
    public Transform currentPoint;
    public Transform[] points;
    public int pointSelection;
    public float waitTime;
    public float waitTimer;

	void Start ()
    {
        currentPoint = points[pointSelection];
	}
	
	void FixedUpdate ()
    {
        platform.transform.position = Vector3.MoveTowards(platform.transform.position, currentPoint.position, Time.deltaTime * moveSpeed);

        if (platform.transform.position == currentPoint.position)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waitTimer = 0;
                pointSelection++;

                if (pointSelection == points.Length)
                {
                    pointSelection = 0;
                }

                currentPoint = points[pointSelection];
            }
        }
	}
}
