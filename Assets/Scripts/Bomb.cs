﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombType
{
    None,
    Column,
    Row,
    Adjacent,
    Color
}

public class Bomb : Character
{
    public BombType bombType;
}
