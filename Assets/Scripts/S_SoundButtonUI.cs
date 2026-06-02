using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_SoundButtonUI : MonoBehaviour
{
    private Button _button;
    private bool _isMuted = false;
    
    [SerializeField] private Sprite _mutedImage;
    [SerializeField] private Sprite _unmutedImage;
    
    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }
    
    private void HandleClick()
    {
        _isMuted = !_isMuted;
        _button.image.sprite = _isMuted ? _mutedImage : _unmutedImage;
    }
}
