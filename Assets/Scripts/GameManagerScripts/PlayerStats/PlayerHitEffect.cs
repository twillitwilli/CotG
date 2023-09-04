using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitEffect : MonoBehaviour
{
    [SerializeField] 
    private VRPlayerController player;

    [SerializeField] 
    private Animator hitEffect;

    public void PlayerHit()
    {
        AnimationClip("ReturnVisionToNormal");
    }

    public void CheckVision()
    {
        AnimationClip("NormalVision");
        //if (CheckIfAlmostDead()) AnimationClip("PlayerAlmostDead");
        //else AnimationClip("NormalVision");
    }

    public bool CheckIfAlmostDead()
    {
        float healthPercentage = (PlayerStats.Instance.GetCurrentHealth() / PlayerStats.Instance.GetMaxHealth());

        if (healthPercentage > 0.15f)
            return true;

        else
            return false;
    }

    public void AnimationClip(string animationClipName)
    {
        hitEffect.Play(animationClipName);
    }
}
