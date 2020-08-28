using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public MatchValue matchValue;
    [SerializeField] InterpolationType interpolation = InterpolationType.SmootherStep;

    Board board;
    bool isMoving = false;

    public enum InterpolationType
    {
      Linear,
      EaseOut,
      EaseIn,
      SmoothStep,
      SmootherStep
    };

    public enum MatchValue
    {
        RedBlock,
        YellowBlock,
        BlueBlock,
        PurpleBlock,
        GreenBlock,
        Cat,
        Dog,
        Sheep,
        DarkCat,
        ZombieCat,
        BrownCat,
        DarkSheep
    };

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    Move((int)transform.position.x + 1, (int)transform.position.y, 0.5f);
        //}
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    Move((int)transform.position.x - 1, (int)transform.position.y, 0.5f);
        //}
    }

    public void Init(Board board)
    {
        this.board = board;
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

                if (board != null)
                {
                    board.PlaceCharacter(this, (int)destination.x, (int)destination.y);
                }
                break;
            }

            // track the total running time
            elapsedTime += Time.deltaTime;

            // calculate the lerp value
            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            switch(interpolation)
            {
                case InterpolationType.Linear:
                    break;
                case InterpolationType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case InterpolationType.EaseIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;
                case InterpolationType.SmoothStep:
                    t = t * t * (3 - 2 * t);
                    break;
                case InterpolationType.SmootherStep:
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            }

            transform.position = Vector2.Lerp(startPosition, destination, t);

            // wait until next frame
            yield return null;
        }

        isMoving = false;
    }
}
