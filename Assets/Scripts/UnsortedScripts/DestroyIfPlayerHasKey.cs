using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfPlayerHasKey : MonoBehaviour
{
    private void LateUpdate()
    {
        if (LocalGameManager.Instance.GetPlayerStats().GetCurrentKeys() > 0) { Destroy(gameObject); }
    }
}
