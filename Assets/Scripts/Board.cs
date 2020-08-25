using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int borderSize = 1;
    [SerializeField] GameObject tilePrefab;

    Tile[,] tiles;

    private void Start()
    {
        tiles = new Tile[width, height];
        SetupTiles();
        SetupCamera();
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
}
