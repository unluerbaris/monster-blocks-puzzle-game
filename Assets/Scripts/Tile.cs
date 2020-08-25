using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] int xIndex;
    [SerializeField] int yIndex;

    Board board;

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;
        this.board = board;
    }
}
