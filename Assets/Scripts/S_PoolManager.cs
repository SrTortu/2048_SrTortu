using System.Collections.Generic;
using UnityEngine;

public class S_PoolManager : MonoBehaviour
{
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private S_GridManager _gridManager;

    private Queue<GameObject> _objectPool;
    private Transform _poolContainer;

    private void Awake()
    {
        _objectPool = new Queue<GameObject>();

        // Create container for pooled objects
        _poolContainer = new GameObject("PoolContainer").transform;
        _poolContainer.SetParent(transform, false);

        int gridSize = _gridManager.GridSize;
        int poolSize = gridSize * gridSize;

        // Create the tile pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tile =  Instantiate(_tilePrefab);
            tile.SetActive(false);
            tile.transform.SetParent(_poolContainer, false);
            _objectPool.Enqueue(tile);
        }
    }

    public S_Tile SpawnFromPool(Transform parent)
    {
        GameObject objectToSpawn = _objectPool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetParent(parent, false);
        return objectToSpawn.GetComponent<S_Tile>();
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(_poolContainer, false);
        _objectPool.Enqueue(obj);
    }
}
