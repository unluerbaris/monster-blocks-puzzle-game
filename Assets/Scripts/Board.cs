﻿using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int borderSize = 1;
    [SerializeField] float swapTime = 0.5f;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject[] characterPrefabs;

    Tile[,] tiles;
    Character[,] characters;
    Tile clickedTile;
    Tile targetTile;

    private void Start()
    {
        tiles = new Tile[width, height];
        characters = new Character[width, height];
        SetupTiles();
        SetupCamera();
        FillRandom();
    }

    private void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector2(i, j), Quaternion.identity) as GameObject;
                tile.name = $"Tile({i}, {j})";
                tiles[i, j] = tile.GetComponent<Tile>();
                tile.transform.parent = transform;
                tiles[i, j].Init(i, j, this);
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

    private void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject randomCharacter = Instantiate(GetRandomCharacter(), Vector2.zero, Quaternion.identity) as GameObject;

                if (randomCharacter != null)
                {
                    randomCharacter.GetComponent<Character>().Init(this);
                    PlaceCharacter(randomCharacter.GetComponent<Character>(), i, j);
                    randomCharacter.transform.parent = transform;
                }
            }
        }
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
        Character clickedCharacter = characters[clickedTile.xIndex, clickedTile.yIndex];
        Character targetCharacter = characters[targetTile.xIndex, targetTile.yIndex];

        clickedCharacter.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
        targetCharacter.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
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
}
