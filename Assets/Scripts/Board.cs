using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int borderSize = 1;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject[] characterPrefabs;

    Tile[,] tiles;
    Character[,] characters; 

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

    private void PlaceCharacter(Character character, int x, int y)
    {
        if (character == null)
        {
            Debug.LogWarning("Invalid character!");
            return;
        }

        character.transform.position = new Vector2(x, y);
        character.transform.rotation = Quaternion.identity;
        character.SetCoord(x, y);
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
                    PlaceCharacter(randomCharacter.GetComponent<Character>(), i, j);
                }
            }
        }
    }
}
