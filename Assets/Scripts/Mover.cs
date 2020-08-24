using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    float gravitySpeed = 2f; //0.25f;
    float xSpeed = 0.5f;
    bool canMove = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider is BoxCollider2D)
        {
            gravitySpeed = 0f;
            canMove = false;
            GameObject.FindWithTag("Spawner").GetComponent<Spawner>().SpawnObject();
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is PolygonCollider2D)
        {
            canMove = false;
        }
    }

    void Update()
    {
        transform.position -= transform.up * gravitySpeed * Time.deltaTime;

        if (!canMove) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x + xSpeed, transform.position.y), 1);
            Debug.Log($"New x position: {transform.position.x}");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x - xSpeed, transform.position.y), 1);
            Debug.Log($"New x position: {transform.position.x}");
        }
    }
}
