using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : MonoSingleton<MultiplayerManager>
{
    public bool coop { get; private set; }
    public void ToggleCoop(bool coopStatus)
    {
        coop = coopStatus;
    }

    [SerializeField]
    private NetworkManager _networkManager;
    public NetworkManager GetNetworkManager() { return _networkManager; }

    [SerializeField]
    private CoopManager _coopManager;
    public CoopManager GetCoopManager() { return _coopManager; }
}