using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentMinion : MonoBehaviour
{
    public int currentMinionStage;
    public GameObject currentMinion;

    public void CheckMinionStage()
    {
        currentMinionStage = GetMinionStage();

        if (currentMinion != null)
        {
            int spawnedMinionStage = currentMinion.GetComponent<MinionPetController>().minionStage;

            if (spawnedMinionStage != currentMinionStage)
            {
                Destroy(currentMinion.GetComponent<MinionPetController>().minion);
                Destroy(currentMinion);
                currentMinion = null;
                SpawnNewMinion();
            }
        }
    }

    public int GetMinionStage()
    {
        int magicFocus = (int)PlayerStats.Instance.data.magicFocus;

        if (magicFocus < 6)
            return 0;

        else if (magicFocus >= 6 && magicFocus < 12)
            return 1;

        else if (magicFocus >= 12)
            return 2;

        return 0;
    }

    public void SpawnNewMinion()
    {
        currentMinion = Instantiate(MasterManager.playerMagicController.minions[currentMinionStage].minion[MagicController.Instance.magicIdx]);
    }
}
