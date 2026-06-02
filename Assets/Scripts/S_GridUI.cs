using UnityEngine;
using UnityEngine.UI;

public class S_GridUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _gridContainer;
    [SerializeField] private Image _gridImage;

    [Header("Grid Settings")]
    [SerializeField] private int _gridSize = 4;
    [SerializeField] private Color _gridColor;
    [SerializeField] private float _cellSize = 100f;
    [SerializeField] private float _spacing = 10f;
    [SerializeField] private Color _cellBackgroundColor;

    public int GridSize => _gridSize;
    public RectTransform GridContainer => _gridContainer;
    public float CellSize => _cellSize;
    public float Spacing => _spacing;

    private void Awake()
    {
        CreateBackgroundCells(_gridSize * _gridSize);
    }

    // Creates the backgrounds of free cells
    private void CreateBackgroundCells(int total)
    {
        for (int i = 0; i < total; i++)
        {
            GameObject cellBg = new GameObject($"Cell_{i}");
            cellBg.transform.SetParent(_gridContainer, false);

            RectTransform rect = cellBg.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(_cellSize - 5, _cellSize - 5); // -5px margin
            rect.anchoredPosition = GetTilePosition(i);

            Image image = cellBg.AddComponent<Image>();
            image.color = _cellBackgroundColor;
        }
    }

    // From the index, calculates the tile position spatially [X,Y]
    public Vector2 GetTilePosition(int index)
    {
        int row = index / _gridSize;
        int col = index % _gridSize;

        float totalGridSize = _gridSize * _cellSize + (_gridSize + 1) * _spacing;

        float x = col * (_cellSize + _spacing) + _spacing - totalGridSize / 2 + _cellSize / 2;
        float y = -row * (_cellSize + _spacing) - _spacing + totalGridSize / 2 - _cellSize / 2;

        return new Vector2(x, y);
    }

    // Instead of Awake, initialize the grid in OnValidate for quick visual feedback
    private void OnValidate()
    {
        if (_gridContainer != null)
        {
            float total = _gridSize * _cellSize + (_gridSize + 1) * _spacing;
            float margin = 4f;
            _gridContainer.sizeDelta = new Vector2(total + margin, total + margin);
        }
        if (_gridImage != null)
        {
            _gridImage.color = _gridColor;
        }
    }
}
