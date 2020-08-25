using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] int xIndex;
    [SerializeField] int yIndex;

    bool isMoving = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move((int)transform.position.x + 1, (int)transform.position.y, 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move((int)transform.position.x - 1, (int)transform.position.y, 0.5f);
        }
    }

    public void SetCoord(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void Move(int destinationX, int destinationY, float timeToMove)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveRoutine(new Vector2(destinationX, destinationY), timeToMove));
        } 
    }

    IEnumerator MoveRoutine(Vector2 destination, float timeToMove)
    {
        Vector2 startPosition = transform.position;
        bool reachedDestination = false;
        float elapsedTime = 0f;

        isMoving = true;

        while (!reachedDestination)
        {
            // if we are close enough to destination
            if (Vector2.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;
                transform.position = destination;
                SetCoord((int)destination.x, (int)destination.y);
                break;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            transform.position = Vector2.Lerp(startPosition, destination, t);

            // wait until next frame
            yield return null;
        }

        isMoving = false;
    }
}
