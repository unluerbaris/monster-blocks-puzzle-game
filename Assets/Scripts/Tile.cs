using System.Collections;
using UnityEngine;

public enum TileType
{
    Normal,
    Obstacle,
    Breakable
}

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    Board board;
    SpriteRenderer spriteRenderer;

    public TileType tileType = TileType.Normal;
    public int breakableValue = 0;
    [SerializeField] Sprite[] breakableSprites;
    public Color normalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;
        this.board = board;

        if (tileType == TileType.Breakable)
        {
            if (breakableSprites[breakableValue] != null)
            {
                spriteRenderer.sprite = breakableSprites[breakableValue];
            }
        }
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
        board.ReleaseTile();
    }

    public void BreakTile()
    {
        if (tileType != TileType.Breakable)
        {
            return;
        }

        StartCoroutine(BreakTileRoutine());
    }

    IEnumerator BreakTileRoutine()
    {
        breakableValue--;
        breakableValue = Mathf.Clamp(breakableValue, 0, breakableValue);

        yield return new WaitForSeconds(0.25f);

        if (breakableSprites[breakableValue] != null)
        {
            spriteRenderer.sprite = breakableSprites[breakableValue];
        }

        if (breakableValue == 0)
        {
            tileType = TileType.Normal;
            spriteRenderer.color = normalColor;
        }
    }
}
