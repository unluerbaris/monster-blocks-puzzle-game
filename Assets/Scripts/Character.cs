using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] int xIndex;
    [SerializeField] int yIndex;

    public void SetCoord(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
}
