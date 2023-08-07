﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menuPrefab;
    [SerializeField] private PlayerPrefsSaveData _playerPrefSaveData;

    private VRPlayerController _player;
    private PlayerComponents _playerComponents;

    [Header("Secondary Menus")]
    public Transform secondaryMenuSpawnLocation;
    public GameObject handAdjusterPrefab, playerCalibrationPrefab;

    [HideInInspector] public GameObject spawnedMenu, spawnedHandAdjuster, spawnedPlayerCalibration;

    private void Awake()
    {
        LocalGameManager.playerCreated += NewPlayerCreated;
    }

    public void NewPlayerCreated(VRPlayerController player)
    {
        _player = player;
    }

    public void OpenMenu(int hand)
    {
        _playerComponents = _player.GetPlayerComponents();
        VRPlayerHand menuHand = _playerComponents.GetHand(hand);

        if (spawnedMenu == null)
        {
            spawnedMenu = Instantiate(_menuPrefab, menuHand.GetMenuSpawnLocation());
            spawnedMenu.transform.SetParent(menuHand.GetMenuSpawnLocation());

            if (hand == 1)
            {
                spawnedMenu.transform.localScale = new Vector3(1, 1, -1);
                HandSetup(1, 0);
            }

            else HandSetup(0, 1);
        }
    }

    public void OpenHandAdjuster()
    {
        spawnedHandAdjuster = Instantiate(handAdjusterPrefab, secondaryMenuSpawnLocation);
        spawnedHandAdjuster.transform.SetParent(null);

        HandAdjustmentController adjustmentController = spawnedHandAdjuster.GetComponent<HandAdjustmentController>();
        adjustmentController.menu = spawnedMenu;

        _playerComponents.GetHand(0).HandIdleState();
        _playerComponents.GetHand(1).HandIdleState();
    }

    public void HandSetup(int hand, int oppositeHand)
    {
        _playerComponents.GetHand(hand).HandIdleState();
        _playerComponents.GetHand(oppositeHand).GetHandAnimationState().SwitchHandState(HandAnimationState.HandState.fingerPoint);
    }

    public void OpenPlayerCalibration()
    {
        if (spawnedPlayerCalibration == null)
        {
            spawnedPlayerCalibration = Instantiate(playerCalibrationPrefab, secondaryMenuSpawnLocation);

            PlayerCalibrationController calibrationController = spawnedPlayerCalibration.GetComponent<PlayerCalibrationController>();

            if (spawnedMenu != null) { calibrationController.menu = spawnedMenu; }
            else
            {
                GameObject newMenu = Instantiate(_menuPrefab);
                newMenu.SetActive(false);
                calibrationController.menu = newMenu;
            }
        }
    }

    public void CloseMenu()
    {
        if (spawnedMenu != null)
        {
            _playerPrefSaveData.SaveData();

            Destroy(spawnedMenu);
            spawnedMenu = null;

            if (spawnedHandAdjuster != null) 
            {
                _playerComponents.GetHand(0).CheckHandModelDistance();
                _playerComponents.GetHand(1).CheckHandModelDistance();

                Destroy(spawnedHandAdjuster); 
            }
        }

        if (secondaryMenuSpawnLocation.childCount > 0)
        {
            for(int i = 0; i < secondaryMenuSpawnLocation.childCount; i++)
            {
                Destroy(secondaryMenuSpawnLocation.GetChild(i).gameObject);
            }
        }
    }
}
