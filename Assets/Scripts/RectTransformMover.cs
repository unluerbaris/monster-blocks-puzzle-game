using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectTransformMover : MonoBehaviour
{
    [SerializeField] Vector2 startPosition;
    [SerializeField] Vector2 onScreenPosition;
    [SerializeField] Vector2 endPosition;
    [SerializeField] float timeToMove = 1f;

    RectTransform rectTransform;
    bool isMoving = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Move(Vector2 startPos, Vector2 endPos, float timeToMove)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveRoutine(startPos, endPos, timeToMove));
        }
    }

    IEnumerator MoveRoutine(Vector2 startPos, Vector2 endPos, float timeToMove)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startPos;
        }

        bool reachedDestination = false;
        float elapsedTime = 0f;
        isMoving = true;

        while (!reachedDestination)
        {
            if (Vector2.Distance(rectTransform.anchoredPosition, endPos) < 0.01f)
            {
                reachedDestination = true;
                break;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);
            t = t * t * t * (t * (t * 6 - 15) + 10); // Smoother step formula

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            }

            yield return null;
        }

        isMoving = false;
    }

    public void MoveOn()
    {
        Move(startPosition, onScreenPosition, timeToMove);
    }

    public void MoveOff()
    {
        Move(onScreenPosition, endPosition, timeToMove);
    }
}
