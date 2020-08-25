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

    private void OnMouseDown()
    {
        if (board != null)
        {
            board.ClickTile(this);
        }
    }

    private void OnMouseEnter()
    {
        board.DragToTile(this);
    }

    private void OnMouseUp()
    {
        Debug.Log("Mouse up");
        board.ReleaseTile();
    }
}
