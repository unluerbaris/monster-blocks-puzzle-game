using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int borderSize = 1;
    public float swapTime = 0.5f;
    [SerializeField] int fillYOffset = 10;
    [SerializeField] float fillMoveTime = 0.5f;

    [SerializeField] GameObject tileNormalPrefab;
    [SerializeField] GameObject tileObstaclePrefab;
    [SerializeField] GameObject[] blockPrefabs;

    [SerializeField] GameObject adjacentBombPrefab;
    [SerializeField] GameObject columnBombPrefab;
    [SerializeField] GameObject rowBombPrefab;

    GameObject clickedTileBomb;
    GameObject targetTileBomb;

    bool playerInputEnabled = true;

    [SerializeField] StartingObject[] startingTiles;
    [SerializeField] StartingObject[] startingGameBlocks;
    [SerializeField] int scoreMultiplier = 0;
    public bool isRefilling = false;

    Tile[,] tiles;
    Block[,] blocks;
    Tile clickedTile;
    Tile targetTile;

    [System.Serializable]
    public class StartingObject
    {
        public GameObject prefab;
        public int x;
        public int y;
    }

    private void Start()
    {
        tiles = new Tile[width, height];
        blocks = new Block[width, height];
        // HighlightMatches();
    }

    // Call SetupBoard() method from GameManager
    public void SetupBoard()
    {
        SetupTiles();
        SetupGameBlocks();
        SetupCamera();
        FillBoard(fillYOffset, fillMoveTime);
    }

    private void SetupTiles()
    {
        foreach (StartingObject startingTile in startingTiles)
        {
            if (startingTile != null)
            {
                MakeTile(startingTile.prefab, startingTile.x, startingTile.y);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] == null)
                {
                    MakeTile(tileNormalPrefab, i, j);
                }
            }
        }
    }

    private void MakeTile(GameObject tilePrefab, int x, int y)
    {
        if (tilePrefab != null && IsWithinBounds(x, y))
        {
            GameObject tile = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity) as GameObject;
            tile.name = $"Tile({x}, {y})";
            tiles[x, y] = tile.GetComponent<Tile>();
            tile.transform.parent = transform;
            tiles[x, y].Init(x, y, this);
        }
    }

    private Block MakeGameBlock(GameObject prefab, int x, int y, int falseYOffset, float moveTime)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            prefab.GetComponent<Block>().Init(this);
            PlaceBlock(prefab.GetComponent<Block>(), x, y);

            if (falseYOffset != 0)
            {
                prefab.transform.position = new Vector3(x, y + falseYOffset, 0);
                prefab.GetComponent<Block>().Move(x, y, moveTime);
            }

            prefab.transform.parent = transform;
            return prefab.GetComponent<Block>();
        }
        return null;
    }

    GameObject MakeBomb(GameObject prefab, int x, int y)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            GameObject bomb = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
            bomb.GetComponent<Bomb>().Init(this);
            bomb.GetComponent<Bomb>().SetCoord(x, y);
            bomb.transform.parent = transform;
            return bomb;
        }
        return null;
    }

    private void SetupGameBlocks()
    {
        foreach (StartingObject startingObject in startingGameBlocks)
        {
            if (startingObject != null)
            {
                GameObject piece = Instantiate(startingObject.prefab, new Vector2(startingObject.x, startingObject.y), Quaternion.identity) as GameObject;
                MakeGameBlock(piece, startingObject.x, startingObject.y, fillYOffset, fillMoveTime);
            }
        }
    }

    private void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1)/2f, (float)(height - 1)/2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;

        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
    }

    private GameObject GetRandomBlock()
    {
        int randomIndex = Random.Range(0, blockPrefabs.Length);

        if (blockPrefabs[randomIndex] == null)
        {
            Debug.LogWarning($"Index {randomIndex} does not contain a valid prefab!");
        }
        return blockPrefabs[randomIndex];
    }

    public void PlaceBlock(Block block, int x, int y)
    {
        if (block == null)
        {
            Debug.LogWarning("Invalid block!");
            return;
        }

        block.transform.position = new Vector2(x, y);
        block.transform.rotation = Quaternion.identity;
        if (IsWithinBounds(x, y))
        {
            blocks[x, y] = block;
        }
        block.SetCoord(x, y);
    }

    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    private void FillBoard(int falseYOffset = 0, float moveTime = 0.1f)
    {
        int maxIterations = 100;
        int iterations = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (blocks[i, j] == null && tiles[i, j].tileType != TileType.Obstacle)
                {
                    FillRandomAt(i, j, falseYOffset, moveTime);
                    iterations = 0;

                    while (HasMatchOnFill(i, j))
                    {
                        RemoveBlockAt(i, j);
                        FillRandomAt(i, j, falseYOffset, moveTime);

                        iterations++;
                        if (iterations >= maxIterations)
                        {
                            Debug.Log("Break... Too many iterations!");
                            break;
                        }
                    }
                }
            }
        }
    }

    Block FillRandomAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (IsWithinBounds(x, y))
        {
            GameObject randomBlock = Instantiate(GetRandomBlock(), Vector2.zero, Quaternion.identity) as GameObject;

            return MakeGameBlock(randomBlock, x, y, falseYOffset, moveTime);        
        }
        return null;
    }

    bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        List<Block> leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);
        List<Block> downwardMatches = FindMatches(x, y, new Vector2(0, -1), minLength);

        if (leftMatches == null)
        {
            leftMatches = new List<Block>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<Block>();
        }

        return (leftMatches.Count > 0 || downwardMatches.Count > 0);
    }

    public void ClickTile(Tile tile)
    {
        if (clickedTile == null)
        {
            clickedTile = tile;
            //Debug.Log($"Clicked tile: {tile.name}");
        }
    }

    public void DragToTile(Tile tile)
    {
        if (clickedTile != null && IsNextTo(tile, clickedTile))
        {
            targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (clickedTile != null && targetTile != null)
        {
            SwitchTiles(clickedTile, targetTile);
        }
        clickedTile = null;
        targetTile = null;
    }

    private void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
    }

    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        if (playerInputEnabled && !GameManager.Instance.IsGameOver)
        {
            Block clickedBlock = blocks[clickedTile.xIndex, clickedTile.yIndex];
            Block targetBlock = blocks[targetTile.xIndex, targetTile.yIndex];

            if (targetBlock != null && clickedBlock != null)
            {
                clickedBlock.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                targetBlock.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);

                yield return new WaitForSeconds(swapTime);

                List<Block> clickedBlockMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                List<Block> targetBlockMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);

                if (targetBlockMatches.Count == 0 && clickedBlockMatches.Count == 0)
                {
                    clickedBlock.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                    targetBlock.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                }
                else
                {
                    if (GameManager.Instance != null)
                    {
                        //GameManager.Instance.movesLeft--;
                        GameManager.Instance.UpdateMoves();
                    }
                    yield return new WaitForSeconds(swapTime);
                    Vector2 swipeDirection = new Vector2(targetTile.xIndex - clickedTile.xIndex, targetTile.yIndex - clickedTile.yIndex);
                    clickedTileBomb = DropBomb(clickedTile.xIndex, clickedTile.yIndex, swipeDirection, clickedBlockMatches);
                    targetTileBomb = DropBomb(targetTile.xIndex, targetTile.yIndex, swipeDirection, targetBlockMatches);
                    ClearAndRefillBoard(clickedBlockMatches.Union(targetBlockMatches).ToList());
                }
            }
        }
    }

    bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
        {
            return true;
        }
        if (Mathf.Abs(start.yIndex- end.yIndex) == 1 && start.xIndex == end.xIndex)
        {
            return true;
        }
        return false;
    }

    List<Block> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<Block> matches = new List<Block>();
        Block startBlock = null;

        if (IsWithinBounds(startX, startY))
        {
            startBlock = blocks[startX, startY];
        }
        if (startBlock != null)
        {
            matches.Add(startBlock);
        }
        else
        {
            return null;
        }

        int nextX;
        int nextY;
        int maxValue = (width > height) ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            Block nextBlock = blocks[nextX, nextY];

            if (nextBlock == null)
            {
                break;
            }
            else
            {
                if (nextBlock.matchValue == startBlock.matchValue && !matches.Contains(nextBlock))
                {
                    matches.Add(nextBlock);
                }
                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }
        return null;
    }

    List<Block> FindAllMatches()
    {
        List<Block> combinedMatches = new List<Block>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<Block> matches = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }

    List<Block> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<Block> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<Block> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<Block>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<Block>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();
        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    List<Block> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<Block> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<Block> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<Block>();
        }
        if (leftMatches == null)
        {
            leftMatches = new List<Block>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();
        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    private void HighlightMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                HighlightMatchesAt(i, j);
            }
        }
    }

    private void HighlightMatchesAt(int x, int y)
    {
        SpriteRenderer spriteRenderer = HighlightTileOff(x, y);

        List<Block> combinedMatches = FindMatchesAt(x, y);

        if (combinedMatches.Count > 0)
        {
            foreach (Block block in combinedMatches)
            {
                spriteRenderer = HighlightTileOn(block.xIndex, block.yIndex, Color.cyan);
            }
        }
    }

    private SpriteRenderer HighlightTileOn(int x, int y, Color color)
    {
        SpriteRenderer spriteRenderer = tiles[x, y].GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        return spriteRenderer;
    }

    private SpriteRenderer HighlightTileOff(int x, int y)
    {
        if (tiles[x,y].tileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = tiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
            return spriteRenderer;
        }
        return null;
    }

    private List<Block> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<Block> horizontalMatches = FindHorizontalMatches(x, y, minLength);
        List<Block> verticalMatches = FindVerticalMatches(x, y, minLength);

        if (horizontalMatches == null)
        {
            horizontalMatches = new List<Block>();
        }
        if (verticalMatches == null)
        {
            verticalMatches = new List<Block>();
        }

        var combinedMatches = horizontalMatches.Union(verticalMatches).ToList();
        return combinedMatches;
    }

    private List<Block> FindMatchesAt(List<Block> blocks, int minLength = 3)
    {
        List<Block> matches = new List<Block>();

        foreach (Block block in blocks)
        {
            matches = matches.Union(FindMatchesAt(block.xIndex, block.yIndex, minLength)).ToList();
        }
        return matches;
    }

    private void RemoveBlockAt(int x, int y)
    {
        Block blockToRemove = blocks[x, y];

        if (blockToRemove != null)
        {
            blocks[x, y] = null;
            Destroy(blockToRemove.gameObject);
        }
        HighlightTileOff(x, y);
    }

    private void RemoveBlockAt(List<Block> blocks)
    {
        foreach (Block block in blocks)
        {
            if (block != null)
            {
                RemoveBlockAt(block.xIndex, block.yIndex);

                int bonus = 0;
                if (blocks.Count >= 4)
                {
                    bonus = 20;
                }

                block.ScorePoints(scoreMultiplier, bonus);
            }
        }
    }

    private void BreakTileAt(int x, int y)
    {
        Tile tileToBreak = tiles[x, y];

        if (tileToBreak != null)
        {
            tileToBreak.BreakTile();
        }
    }

    private void BreakTileAt(List<Block> blocks)
    {
        foreach (Block block in blocks)
        {
            if (block != null)
            {
                BreakTileAt(block.xIndex, block.yIndex);
            }
        }
    }

    private void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                RemoveBlockAt(i, j);
            }
        }
    }

    List<Block> CollapseColumn(int column, float collapseTime = 0.2f)
    {
        List<Block> movingBlocks = new List<Block>();

        for (int i = 0; i < height - 1; i++)
        {
            if (blocks[column, i] == null && tiles[column, i].tileType != TileType.Obstacle)
            {
                for (int j = i + 1; j < height; j++)
                {
                    if (blocks[column, j] != null)
                    {
                        blocks[column, j].Move(column, i, collapseTime * (j - i));
                        blocks[column, i] = blocks[column, j];
                        blocks[column, i].SetCoord(column, i);

                        if (!movingBlocks.Contains(blocks[column, i]))
                        {
                            movingBlocks.Add(blocks[column, i]);
                        }

                        blocks[column, j] = null;
                        break;
                    }
                }
            }
        }
        return movingBlocks;
    }

    List<Block> CollapseColumn(List<Block> blocks)
    {
        List<Block> movingBlocks = new List<Block>();
        List<int> columnsToCollapse = GetColumns(blocks);

        foreach (int column in columnsToCollapse)
        {
            movingBlocks = movingBlocks.Union(CollapseColumn(column)).ToList();
        }
        return movingBlocks;
    }

    List<Block> CollapseColumn(List<int> columnsToCollapse)
    {
        List<Block> movingBlocks = new List<Block>();
        foreach (int column in columnsToCollapse)
        {
            movingBlocks = movingBlocks.Union(CollapseColumn(column)).ToList();
        }
        return movingBlocks;
    }

    List<int> GetColumns(List<Block> blocks)
    {
        List<int> columns = new List<int>();

        foreach (Block block in blocks)
        {
            if (block != null)
            {
                if (!columns.Contains(block.xIndex))
                {
                    columns.Add(block.xIndex);
                }
            }
        }
        return columns;
    }

    private void ClearAndRefillBoard(List<Block> blocks)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(blocks));
    }

    IEnumerator ClearAndRefillBoardRoutine(List<Block> blocks)
    {
        playerInputEnabled = false;
        isRefilling = true;

        List<Block> matches = blocks;

        scoreMultiplier = 0;

        do
        {
            scoreMultiplier++;

            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;

            yield return StartCoroutine(RefillRoutine());
            matches = FindAllMatches();

            yield return new WaitForSeconds(0.5f);
        }
        while (matches.Count != 0);

        playerInputEnabled = true;
        isRefilling = false;
    }

    IEnumerator RefillRoutine()
    {
        FillBoard(fillYOffset, fillMoveTime);
        yield return null;
    }

    IEnumerator ClearAndCollapseRoutine(List<Block> blocks)
    {
        List<Block> movingBlocks = new List<Block>();
        List<Block> matches = new List<Block>();

        yield return new WaitForSeconds(0.25f);

        bool isFinished = false;

        while (!isFinished)
        {
            List<Block> bombedBlocks = GetBombedBlocks(blocks);
            blocks = blocks.Union(bombedBlocks).ToList();

            bombedBlocks = GetBombedBlocks(blocks);
            blocks = blocks.Union(bombedBlocks).ToList();

            List<int> columnsToCollapse = GetColumns(blocks);

            RemoveBlockAt(blocks);
            BreakTileAt(blocks);

            if (clickedTileBomb != null)
            {
                ActivateBomb(clickedTileBomb);
                clickedTileBomb = null;
            }

            if (targetTileBomb != null)
            {
                ActivateBomb(targetTileBomb);
                targetTileBomb = null;
            }

            yield return new WaitForSeconds(0.25f);
            movingBlocks = CollapseColumn(columnsToCollapse);

            while (!IsCollapsed(movingBlocks))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);
            matches = FindMatchesAt(movingBlocks);

            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                scoreMultiplier++;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayRandomBonusSFX();
                }
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
        yield return null;
    }

    private bool IsCollapsed(List<Block> blocks)
    {
        foreach (Block block in blocks)
        {
            if (block != null)
            {
                if (block.transform.position.y - (float)block.yIndex > 0.001f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    List<Block> GetRowBlocks(int row)
    {
        List<Block> blocks = new List<Block>();

        for (int i = 0; i < width; i++)
        {
            if (this.blocks[i, row] != null)
            {
                blocks.Add(this.blocks[i, row]);
            }
        }
        return blocks;
    }

    List<Block> GetColumnBlocks(int column)
    {
        List<Block> blocks = new List<Block>();

        for (int i = 0; i < height; i++)
        {
            if (this.blocks[column, i] != null)
            {
                blocks.Add(this.blocks[column, i]);
            }
        }
        return blocks;
    }

    List<Block> GetAdjacentBlocks(int x, int y, int offset = 1)
    {
        List<Block> blocks = new List<Block>();

        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (IsWithinBounds(i, j))
                {
                    blocks.Add(this.blocks[i, j]);
                }
            }
        }
        return blocks;
    }

    List<Block> GetBombedBlocks(List<Block> blocks)
    {
        List<Block> allBlocksToClear = new List<Block>();

        foreach (Block block in blocks)
        {
            if (block != null)
            {
                List<Block> blocksToClear = new List<Block>();
                Bomb bomb = block.GetComponent<Bomb>();

                if (bomb != null)
                {
                    switch (bomb.bombType)
                    {
                        case BombType.Column:
                            blocksToClear = GetColumnBlocks(bomb.xIndex);
                            break;
                        case BombType.Row:
                            blocksToClear = GetRowBlocks(bomb.yIndex);
                            break;
                        case BombType.Adjacent:
                            blocksToClear = GetAdjacentBlocks(bomb.xIndex, bomb.yIndex, 1);
                            break;
                        case BombType.Color:
                            break;
                        default:
                            break;
                    }

                    allBlocksToClear = allBlocksToClear.Union(blocksToClear).ToList();
                }
            }
        }
        return allBlocksToClear;
    }

    bool IsCornerMatch(List<Block> blocks)
    {
        bool vertical = false;
        bool horizontal = false;
        int xStart = -1;
        int yStart = -1;

        foreach (Block block in blocks)
        {
            if (block != null)
            {
                if (xStart == -1 || yStart == -1)
                {
                    xStart = block.xIndex;
                    yStart = block.yIndex;
                    continue;
                }
                if (block.xIndex != xStart && block.yIndex == yStart)
                {
                    horizontal = true;
                }
                if (block.xIndex == xStart && block.yIndex != yStart)
                {
                    vertical = true;
                }
            }
        }
        return (horizontal && vertical);
    }

    GameObject DropBomb(int x, int y, Vector2 swapDirection, List<Block> blocks)
    {
        GameObject bomb = null;

        if (blocks.Count >= 4)
        {
            if (IsCornerMatch(blocks))
            {
                if (adjacentBombPrefab != null)
                {
                    bomb = MakeBomb(adjacentBombPrefab, x, y);
                }
            }
            else
            {
                if (swapDirection.x != 0)
                {
                    if (rowBombPrefab != null)
                    {
                        bomb = MakeBomb(rowBombPrefab, x, y);
                    }
                }
                else
                {
                    if (columnBombPrefab != null)
                    {
                        bomb = MakeBomb(columnBombPrefab, x, y);
                    }
                }
            }
        }
        return bomb;
    }

    void ActivateBomb(GameObject bomb)
    {
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;

        if (IsWithinBounds(x,y))
        {
            blocks[x, y] = bomb.GetComponent<Block>();
        }
    }
}
