using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_Tile : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private S_TileAnimator _animator;

    private S_TileData _tileSet;
    private int _dataIndex;
    

    public void Init(S_TileData data, int dataIndex)
    {
        _tileSet = data;
        _dataIndex = dataIndex;
        UpdateColor();
        UpdateText();
    }

    public void UpdateColor() 
    {
        Color color = _tileSet.tileData[_dataIndex].color;
        _background.color = color;   
    }
    
    private void UpdateText()
    {
        if (_valueText != null)
            _valueText.text = _tileSet.tileData[_dataIndex].value.ToString();
    }
    
    public int GetValue()
    {
        return _tileSet.tileData[_dataIndex].value;
    }
    
    public void UpgradeData(int newIndex)
    {
        _dataIndex = newIndex;
        UpdateColor();
        UpdateText();
        AnimateMerge();
    }
    
    public int GetIDataIndex()
    {
        return _dataIndex;
    }

    public void AnimateMerge()
    {
        _animator.AnimateMerge();
    }
    public void AnimateToPosition(Vector2 targetPosition)
    {
        _animator.AnimateToPosition(targetPosition);
    }
    
    public void AnimateSpawn()
    {
        _animator.AnimateSpawn();
    }

}