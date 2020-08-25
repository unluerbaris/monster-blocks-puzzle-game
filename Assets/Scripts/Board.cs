using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] GameObject tilePrefab;

    Tile[,] tiles;

    private void Start()
    {
        tiles = new Tile[width, height];
        SetupTiles();
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
}
