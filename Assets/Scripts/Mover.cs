using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    float gravitySpeed = 0.25f;
    float xSpeed = 0.5f;

    void Update()
    {
        transform.position -= transform.up * gravitySpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x + xSpeed, transform.position.y), 1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x - xSpeed, transform.position.y), 1);
        }
    }
}
