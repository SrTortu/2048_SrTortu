using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class S_GridLogic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private S_GridUI _gridUI;
    [SerializeField] private S_TileData _tileSet;
    [SerializeField] private S_PoolManager _poolManager;

    public event Action OnTileSpawned;
    public event Action OnTileMerge;

    public int LastMergeValue { get; private set; }

    private S_Tile[] _tiles;
    private List<int> _freeIndexes;
    private bool[] _mergedThisTurnCache;

    private void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Creates the tile array
        int total = _gridUI.GridSize * _gridUI.GridSize;
        _tiles = new S_Tile[total];
        _freeIndexes = new List<int>(total);
        _mergedThisTurnCache = new bool[total];

        // Creates the list of free indexes
        for (int i = 0; i < total; i++)
            _freeIndexes.Add(i);
    }

    public void StartNewGame()
    {
        ClearGrid();
        SpawnTile();
        SpawnTile();
    }

    private void ClearGrid()
    {
        // First, return all tiles to the pool
        for (int i = _gridUI.GridContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = _gridUI.GridContainer.GetChild(i);
            if (child.GetComponent<S_Tile>() != null)
            {
                _poolManager.ReturnToPool(child.gameObject);
            }
        }

        InitializeGrid();
    }

    public bool TryMove(Direction direction)
    {
        bool moved = Move(direction);

        if (moved)
            StartCoroutine(SpawnAfterAnimation());

        return moved;
    }

    private IEnumerator SpawnAfterAnimation()
    {
        yield return new WaitForSeconds(0.1f); // Wait for movement animations to finish
        SpawnTile();
    }

    private bool Move(Direction direction)
    {
        bool moved = false;
        Array.Clear(_mergedThisTurnCache, 0, _mergedThisTurnCache.Length);
        LastMergeValue = 0;

        // First: move all tiles
        moved = MoveTiles(direction);

        // Second: do the merges
        bool merged = MergeTiles(direction, _mergedThisTurnCache);

        // Third: move the tiles resulting from merges again
        if (merged)
        {
            moved = MoveTiles(direction) || moved;
        }

        if (moved || merged)
        {
            UpdateFreeIndexes();
            UpdateTiles();
        }

        return moved || merged;
    }

    private bool MoveTiles(Direction direction)
    {
        bool moved = false;
        int startIndex, endIndex, step;

        GetLoopParameters(direction, out startIndex, out endIndex, out step);

        for (int i = startIndex; i != endIndex; i += step)
        {
            if (_tiles[i] != null)
            {
                int targetIndex = i;
                int nextIndex = GetNeighborIndex(targetIndex, direction);

                // Find the farthest possible position
                while (nextIndex >= 0 && nextIndex < _tiles.Length && _tiles[nextIndex] == null)
                {
                    targetIndex = nextIndex;
                    nextIndex = GetNeighborIndex(targetIndex, direction);
                }

                if (targetIndex != i)
                {
                    _tiles[targetIndex] = _tiles[i];
                    _tiles[i] = null;
                    moved = true;
                }
            }
        }

        return moved;
    }

    private bool MergeTiles(Direction direction, bool[] mergedThisTurnCache)
    {
        bool merged = false;
        int startIndex, endIndex, step ;
        GetLoopParameters(direction, out startIndex, out endIndex, out step);

        for (int i = startIndex; i != endIndex; i += step)
        {
            if (_tiles[i] != null)
            {
                int neighborIndex = GetNeighborIndex(i, direction);
                if (neighborIndex >= 0 && neighborIndex < _tiles.Length &&
                    _tiles[neighborIndex] != null &&
                    _tiles[neighborIndex].GetValue() == _tiles[i].GetValue() &&
                    !mergedThisTurnCache[neighborIndex])
                {
                    int newIndex = _tiles[i].GetIDataIndex() + 1;
                    
                    if (newIndex >= _tileSet.tileData.Count)
                    {
                        HandleMaxLevelMerge(i, neighborIndex);
                    }
                    else
                    {
                        HandleNormalMerge(i, neighborIndex, newIndex, mergedThisTurnCache);
                    }
                    merged = true;
                }
            }
        }

        if (merged)
        {
            OnTileMerge?.Invoke();
        }

        return merged;
    }

    private void HandleMaxLevelMerge(int tileIndex, int neighborIndex)
    {
        int lastValue = _tileSet.tileData[_tileSet.tileData.Count - 1].value;
        LastMergeValue += lastValue * lastValue;
        _poolManager.ReturnToPool(_tiles[tileIndex].gameObject);
        _poolManager.ReturnToPool(_tiles[neighborIndex].gameObject);
        _tiles[tileIndex] = null;
        _tiles[neighborIndex] = null;
    }

    private void HandleNormalMerge(int tileIndex, int neighborIndex, int newIndex, bool[] mergedThisTurnCache)
    {
        _tiles[neighborIndex].UpgradeData(newIndex);
        LastMergeValue += _tileSet.tileData[newIndex].value;
        _poolManager.ReturnToPool(_tiles[tileIndex].gameObject);
        _tiles[tileIndex] = null;
        mergedThisTurnCache[neighborIndex] = true;
    }

/*
 * Depending on the direction, the movement and merge loops must traverse the tile array
 * in a specific order to ensure merges are done correctly. With this function I get the
 * necessary parameters for each direction.
 */
    private void GetLoopParameters(Direction direction, out int startIndex, out int endIndex, out int step)
    {
        switch (direction)
        {
            case Direction.Up:
                startIndex = 0;
                endIndex = _tiles.Length;
                step = 1;
                break;
            case Direction.Down:
                startIndex = _tiles.Length - 1;
                endIndex = -1;
                step = -1;
                break;
            case Direction.Left:
                startIndex = 0;
                endIndex = _tiles.Length;
                step = 1;
                break;
            case Direction.Right:
                startIndex = _tiles.Length - 1;
                endIndex = -1;
                step = -1;
                break;
            default:
                startIndex = 0;
                endIndex = _tiles.Length;
                step = 1;
                break;
        }
    }

    /*
     * Calculates the neighbor index depending on the direction.
     */
    private int GetNeighborIndex(int index, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                if (index - _gridUI.GridSize >= 0)
                    return index - _gridUI.GridSize;
                break;
            case Direction.Down:
                if (index + _gridUI.GridSize < _tiles.Length)
                    return index + _gridUI.GridSize;
                break;
            case Direction.Left:
                if (index % _gridUI.GridSize != 0)
                    return index - 1;
                break;
            case Direction.Right:
                if ((index + 1) % _gridUI.GridSize != 0)
                    return index + 1;
                break;
        }
        return -1;
    }

    private void UpdateFreeIndexes()
    {
        _freeIndexes.Clear();
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] == null)
            {
                _freeIndexes.Add(i);
            }
        }
    }

    private void SpawnTile()
    {
        if (_freeIndexes.Count == 0) return;

        int randomIndex = Random.Range(0, _freeIndexes.Count);
        int tileIndex = _freeIndexes[randomIndex];
        int dataIndex = 0;
        _freeIndexes.RemoveAt(randomIndex);

        if (Random.value > 0.9f)
        {
            dataIndex = 1;
        }

        S_Tile newTile = _poolManager.SpawnFromPool(_gridUI.GridContainer);

        RectTransform tileRect = newTile.transform as RectTransform;
        tileRect.sizeDelta = new Vector2(_gridUI.CellSize, _gridUI.CellSize);
        tileRect.anchoredPosition = _gridUI.GetTilePosition(tileIndex);

        newTile.Init(_tileSet, dataIndex);
        _tiles[tileIndex] = newTile;

        newTile.AnimateSpawn();
        OnTileSpawned?.Invoke();
    }

    private void UpdateTiles()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] != null)
            {
                _tiles[i].AnimateToPosition(_gridUI.GetTilePosition(i));
            }
        }
    }

    public bool HasAvailableMoves()
    {
        // If there are empty spaces, there are available moves
        if (_freeIndexes.Count > 0)
            return true;

        // Verify if there are possible merges
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] == null) continue;

            int currentValue = _tiles[i].GetValue();

            // Verify neighbor above
            int upIndex = GetNeighborIndex(i, Direction.Up);
            if (upIndex >= 0 && _tiles[upIndex] != null && _tiles[upIndex].GetValue() == currentValue)
                return true;

            // Verify neighbor below
            int downIndex = GetNeighborIndex(i, Direction.Down);
            if (downIndex >= 0 && _tiles[downIndex] != null && _tiles[downIndex].GetValue() == currentValue)
                return true;

            // Verify neighbor left
            int leftIndex = GetNeighborIndex(i, Direction.Left);
            if (leftIndex >= 0 && _tiles[leftIndex] != null && _tiles[leftIndex].GetValue() == currentValue)
                return true;

            // Verify neighbor right
            int rightIndex = GetNeighborIndex(i, Direction.Right);
            if (rightIndex >= 0 && _tiles[rightIndex] != null && _tiles[rightIndex].GetValue() == currentValue)
                return true;
        }

        return false;
    }
}
