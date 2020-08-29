using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int borderSize = 1;
    [SerializeField] float swapTime = 0.5f;
    [SerializeField] GameObject tileNormalPrefab;
    [SerializeField] GameObject tileObstaclePrefab;
    [SerializeField] GameObject[] characterPrefabs;

    bool playerInputEnabled = true;

    [SerializeField] StartingTile[] startingTiles;

    Tile[,] tiles;
    Character[,] characters;
    Tile clickedTile;
    Tile targetTile;

    [System.Serializable]
    public class StartingTile
    {
        public GameObject tilePrefab;
        public int x;
        public int y;
    }

    private void Start()
    {
        tiles = new Tile[width, height];
        characters = new Character[width, height];
        SetupTiles();
        SetupCamera();
        FillBoard(10, 0.5f);
        // HighlightMatches();
    }

    private void SetupTiles()
    {
        foreach (StartingTile startingTile in startingTiles)
        {
            if (startingTile != null)
            {
                MakeTile(startingTile.tilePrefab, startingTile.x, startingTile.y);
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
        if (tilePrefab != null)
        {
            GameObject tile = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity) as GameObject;
            tile.name = $"Tile({x}, {y})";
            tiles[x, y] = tile.GetComponent<Tile>();
            tile.transform.parent = transform;
            tiles[x, y].Init(x, y, this);
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

    private GameObject GetRandomCharacter()
    {
        int randomIndex = Random.Range(0, characterPrefabs.Length);

        if (characterPrefabs[randomIndex] == null)
        {
            Debug.LogWarning($"Index {randomIndex} does not contain a valid prefab!");
        }
        return characterPrefabs[randomIndex];
    }

    public void PlaceCharacter(Character character, int x, int y)
    {
        if (character == null)
        {
            Debug.LogWarning("Invalid character!");
            return;
        }

        character.transform.position = new Vector2(x, y);
        character.transform.rotation = Quaternion.identity;
        if (IsWithinBounds(x, y))
        {
            characters[x, y] = character;
        }
        character.SetCoord(x, y);
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
                if (characters[i, j] == null && tiles[i, j].tileType != TileType.Obstacle)
                {
                    Character character = FillRandomAt(i, j, falseYOffset, moveTime);
                    iterations = 0;

                    while (HasMatchOnFill(i, j))
                    {
                        RemoveCharacterAt(i, j);
                        character = FillRandomAt(i, j, falseYOffset, moveTime);

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

    Character FillRandomAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        GameObject randomCharacter = Instantiate(GetRandomCharacter(), Vector2.zero, Quaternion.identity) as GameObject;

        if (randomCharacter != null)
        {
            randomCharacter.GetComponent<Character>().Init(this);
            PlaceCharacter(randomCharacter.GetComponent<Character>(), x, y);

            if (falseYOffset != 0)
            {
                randomCharacter.transform.position = new Vector3(x, y + falseYOffset, 0);
                randomCharacter.GetComponent<Character>().Move(x, y, moveTime);
            }

            randomCharacter.transform.parent = transform;
            return randomCharacter.GetComponent<Character>();
        }
        return null;
    }

    bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        List<Character> leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);
        List<Character> downwardMatches = FindMatches(x, y, new Vector2(0, -1), minLength);

        if (leftMatches == null)
        {
            leftMatches = new List<Character>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<Character>();
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
        if (playerInputEnabled)
        {
            Character clickedCharacter = characters[clickedTile.xIndex, clickedTile.yIndex];
            Character targetCharacter = characters[targetTile.xIndex, targetTile.yIndex];

            if (targetCharacter != null && clickedCharacter != null)
            {
                clickedCharacter.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                targetCharacter.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);

                yield return new WaitForSeconds(swapTime);

                List<Character> clickedCharacterMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                List<Character> targetCharacterMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);

                if (targetCharacterMatches.Count == 0 && clickedCharacterMatches.Count == 0)
                {
                    clickedCharacter.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                    targetCharacter.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                }
                else
                {
                    yield return new WaitForSeconds(swapTime);

                    ClearAndRefillBoard(clickedCharacterMatches.Union(targetCharacterMatches).ToList());
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

    List<Character> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<Character> matches = new List<Character>();
        Character startCharacter = null;

        if (IsWithinBounds(startX, startY))
        {
            startCharacter = characters[startX, startY];
        }
        if (startCharacter != null)
        {
            matches.Add(startCharacter);
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

            Character nextCharacter = characters[nextX, nextY];

            if (nextCharacter == null)
            {
                break;
            }
            else
            {
                if (nextCharacter.matchValue == startCharacter.matchValue && !matches.Contains(nextCharacter))
                {
                    matches.Add(nextCharacter);
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

    List<Character> FindAllMatches()
    {
        List<Character> combinedMatches = new List<Character>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<Character> matches = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }

    List<Character> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<Character> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<Character> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<Character>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<Character>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();
        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    List<Character> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<Character> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<Character> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<Character>();
        }
        if (leftMatches == null)
        {
            leftMatches = new List<Character>();
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

        List<Character> combinedMatches = FindMatchesAt(x, y);

        if (combinedMatches.Count > 0)
        {
            foreach (Character character in combinedMatches)
            {
                spriteRenderer = HighlightTileOn(character.xIndex, character.yIndex, Color.cyan);
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

    private List<Character> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<Character> horizontalMatches = FindHorizontalMatches(x, y, minLength);
        List<Character> verticalMatches = FindVerticalMatches(x, y, minLength);

        if (horizontalMatches == null)
        {
            horizontalMatches = new List<Character>();
        }
        if (verticalMatches == null)
        {
            verticalMatches = new List<Character>();
        }

        var combinedMatches = horizontalMatches.Union(verticalMatches).ToList();
        return combinedMatches;
    }

    private List<Character> FindMatchesAt(List<Character> characters, int minLength = 3)
    {
        List<Character> matches = new List<Character>();

        foreach (Character character in characters)
        {
            matches = matches.Union(FindMatchesAt(character.xIndex, character.yIndex, minLength)).ToList();
        }
        return matches;
    }

    private void RemoveCharacterAt(int x, int y)
    {
        Character characterToRemove = characters[x, y];

        if (characterToRemove != null)
        {
            characters[x, y] = null;
            Destroy(characterToRemove.gameObject);
        }
        HighlightTileOff(x, y);
    }

    private void RemoveCharacterAt(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            if (character != null)
            {
                RemoveCharacterAt(character.xIndex, character.yIndex);
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

    private void BreakTileAt(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            if (character != null)
            {
                BreakTileAt(character.xIndex, character.yIndex);
            }
        }
    }

    private void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                RemoveCharacterAt(i, j);
            }
        }
    }

    List<Character> CollapseColumn(int column, float collapseTime = 0.2f)
    {
        List<Character> movingCharacters = new List<Character>();

        for (int i = 0; i < height - 1; i++)
        {
            if (characters[column, i] == null && tiles[column, i].tileType != TileType.Obstacle)
            {
                for (int j = i + 1; j < height; j++)
                {
                    if (characters[column, j] != null)
                    {
                        characters[column, j].Move(column, i, collapseTime * (j - i));
                        characters[column, i] = characters[column, j];
                        characters[column, i].SetCoord(column, i);

                        if (!movingCharacters.Contains(characters[column, i]))
                        {
                            movingCharacters.Add(characters[column, i]);
                        }

                        characters[column, j] = null;
                        break;
                    }
                }
            }
        }
        return movingCharacters;
    }

    List<Character> CollapseColumn(List<Character> characters)
    {
        List<Character> movingCharacters = new List<Character>();
        List<int> columnsToCollapse = GetColumns(characters);

        foreach (int column in columnsToCollapse)
        {
            movingCharacters = movingCharacters.Union(CollapseColumn(column)).ToList();
        }
        return movingCharacters;
    }

    List<int> GetColumns(List<Character> characters)
    {
        List<int> columns = new List<int>();

        foreach (Character character in characters)
        {
            if (!columns.Contains(character.xIndex))
            {
                columns.Add(character.xIndex);
            }
        }
        return columns;
    }

    private void ClearAndRefillBoard(List<Character> characters)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(characters));
    }

    IEnumerator ClearAndRefillBoardRoutine(List<Character> characters)
    {
        playerInputEnabled = false;
        List<Character> matches = characters;

        do
        {
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;

            yield return StartCoroutine(RefillRoutine());
            matches = FindAllMatches();

            yield return new WaitForSeconds(0.5f);
        }
        while (matches.Count != 0);

        playerInputEnabled = true;
    }

    IEnumerator RefillRoutine()
    {
        FillBoard(10, 0.5f);
        yield return null;
    }

    IEnumerator ClearAndCollapseRoutine(List<Character> characters)
    {
        List<Character> movingCharacters = new List<Character>();
        List<Character> matches = new List<Character>();

        yield return new WaitForSeconds(0.25f);

        bool isFinished = false;

        while (!isFinished)
        {
            RemoveCharacterAt(characters);
            BreakTileAt(characters);

            yield return new WaitForSeconds(0.25f);
            movingCharacters = CollapseColumn(characters);

            while (!IsCollapsed(movingCharacters))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);
            matches = FindMatchesAt(movingCharacters);

            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
        yield return null;
    }

    private bool IsCollapsed(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            if (character != null)
            {
                if (character.transform.position.y - (float)character.yIndex > 0.001f)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
