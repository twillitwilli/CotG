using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour
{
    // --- FOR MAGE ONLY ---

    [SerializeField] private VRPlayerController player;
    public Transform[] backWandSpawn;

    private void OnEnable()
    {
        //player.playerComponents.shieldController.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        //player.playerComponents.shieldController.gameObject.SetActive(false);
    }
}
