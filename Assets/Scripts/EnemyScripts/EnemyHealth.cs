﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QTArts.Interfaces;

public class EnemyHealth : MonoBehaviour, iDamagable<float>
{
    public EnemyController enemyController;
    public EnemyHealthDisplay healthDisplayController;
    public SkinnedMeshRenderer healthDisplay;

    public float Health { get; set; }

    public bool isBoss;
    public float maxHealth;

    [Header("EnemyMesh")]
    public SkinnedMeshRenderer enemyMesh;
    public Material normal, wasHit;

    [HideInInspector] 
    public EnemyStatusController statusController;
    
    public bool isDead { get; set; }

    public virtual void Awake()
    {
        statusController = GetComponent<EnemyStatusController>();

        if (LocalGameManager.Instance.currentGameMode == LocalGameManager.GameMode.master)
            maxHealth += maxHealth * 0.25f;
    }

    private void Start()
    {
        Health = maxHealth;
        HealthDisplay();
    }

    public void Damage(float damageAmount)
    {
        AdjustHealth(damageAmount, false);
    }

    public virtual void Death()
    {
        isDead = true;
        enemyController.dropScript.disableDrop = false;

        if (healthDisplay.gameObject)
            Destroy(healthDisplay.gameObject);

        enemyController.EnemyDead();
    }

    public virtual void AdjustHealth(float adjustmentValue, bool coopSync)
    {
        if (!isDead)
        {
            if (MultiplayerManager.Instance.coop && !coopSync)
                MultiplayerManager.Instance.GetCoopManager().coopEnemyController.AdjustEnemyHealth(adjustmentValue, enemyController.spawnID, enemyController.enemyTracker.isBoss);

            if (adjustmentValue < 0)
                WasHit();

            Health += adjustmentValue;

            if (Health <= 0)
            {
                Debug.Log("Enemy Died");

                if (!coopSync)
                    AdjustTotalStats();

                Death();
            }
            else if (Health >= maxHealth)
                Health = maxHealth;

            HealthDisplay();
        }
    }

    public void HealthDisplay()
    {
        float healthPercentage = (Health / maxHealth);
        float healthWholeNumber = healthPercentage * 100;
        healthDisplay.SetBlendShapeWeight(0, healthWholeNumber);
        healthDisplayController.UpdateDisplay(Mathf.RoundToInt(Health), Mathf.RoundToInt(maxHealth));
    }

    public void WasHit()
    {
        enemyMesh.material = wasHit;
        Invoke("ReturnToNormalMaterial", 0.1f);
    }

    private void ReturnToNormalMaterial()
    {
        enemyMesh.material = normal;
    }

    public void AdjustTotalStats()
    {
        PlayerTotalStats totalStats = PlayerTotalStats.Instance;

        switch (enemyController.enemyName)
        {
            case EnemyController.Enemy.bat:
                totalStats.AdjustStats(PlayerTotalStats.StatType.batsKilled);
                break;

            case EnemyController.Enemy.bee:
                totalStats.AdjustStats(PlayerTotalStats.StatType.beesKilled);
                break;

            case EnemyController.Enemy.bunny:
                totalStats.AdjustStats(PlayerTotalStats.StatType.bunniesKilled);
                break;

            case EnemyController.Enemy.goblin:
                totalStats.AdjustStats(PlayerTotalStats.StatType.goblinsKilled);
                break;

            case EnemyController.Enemy.mushroom:
                totalStats.AdjustStats(PlayerTotalStats.StatType.mushroomsKilled);
                break;

            case EnemyController.Enemy.plant:
                totalStats.AdjustStats(PlayerTotalStats.StatType.plantsKilled);
                break;

            case EnemyController.Enemy.wolf:
                totalStats.AdjustStats(PlayerTotalStats.StatType.wolvesKilled);
                break;

            case EnemyController.Enemy.golem:
                totalStats.AdjustStats(PlayerTotalStats.StatType.golemsKilled);
                break;

            case EnemyController.Enemy.treant:
                totalStats.AdjustStats(PlayerTotalStats.StatType.treantsKilled);
                break;

            case EnemyController.Enemy.dragon:
                totalStats.AdjustStats(PlayerTotalStats.StatType.dragonsKilled);
                break;

            case EnemyController.Enemy.babyReaper:
                totalStats.AdjustStats(PlayerTotalStats.StatType.babyReaperKills);
                break;

            case EnemyController.Enemy.princeReaper:
                totalStats.AdjustStats(PlayerTotalStats.StatType.princeReapersKilled);
                break;

            case EnemyController.Enemy.godReaper:
                totalStats.AdjustStats(PlayerTotalStats.StatType.godReapersKilled);
                break;
        }

        totalStats.AdjustStats(PlayerTotalStats.StatType.enemiesKilled);
    }
}
