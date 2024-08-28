using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    float moveSpeed = 5f;
    public UDPClient client;
    private Vector3 lastDirection = Vector3.zero;
    public void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h,0,v).normalized;
        
        transform.position += dir * moveSpeed * Time.deltaTime;
        if (dir != lastDirection)
        {
            client.SendData(new JsonData { direction = dir + "" });
            lastDirection = dir;
        }
    }
}
