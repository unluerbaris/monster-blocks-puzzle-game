using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    float gravitySpeed = 0.25f;
    float xSpeed = 32f;

    void Update()
    {
        transform.position -= transform.up * gravitySpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += transform.right * xSpeed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position -= transform.right * xSpeed * Time.deltaTime;
        }
    }
}
