using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TileData
{
    public int value;
    public Color color;
}

[CreateAssetMenu(fileName = "New Tile Data", menuName = "ScriptableObjects/TileData")]
public class S_TileData : ScriptableObject
{
    public List<TileData> tileData;
}
