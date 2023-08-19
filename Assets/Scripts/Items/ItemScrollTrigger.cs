using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScrollTrigger : MonoBehaviour
{
    private LocalGameManager _gameManager;
    private PlayerStats _playerStats;
    private PlayerTotalStats _playerTotalStats;

    [SerializeField] private GameObject _scrollParent;

    private int _scrollPrice;

    private void Start()
    {
        _gameManager = LocalGameManager.instance;
        _playerStats = _gameManager.GetPlayerStats();
        _playerTotalStats = _gameManager.GetTotalStats();
    }

    public void SetScrollPrice(int scrollPrice)
    {
        _scrollPrice = scrollPrice;
    }

    private void OnTriggerEnter(Collider other)
    {
        WalletItem wallet;
        if (other.gameObject.TryGetComponent<WalletItem>(out wallet))
        {
            if (_playerStats.GetCurrentGold() >= _scrollPrice)
            {
                AbsorbScrollKnowledge();
                _playerStats.AdjustGoldAmount(-_scrollPrice);
                _playerTotalStats.AdjustStats(PlayerTotalStats.StatType.itemsBought);
            }
        }
    }

    public void AbsorbScrollKnowledge()
    {
        if (_gameManager.inDungeon)
        {
            _playerTotalStats.AdjustStats(PlayerTotalStats.StatType.scrollsAbsorbed);
        }

        Destroy(_scrollParent);
    }
}
