using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpellCastingForHands : MonoBehaviour
{
    VRPlayer _player;
    PlayerComponents _playerComponents;
    VRHand _hand;

    PlayerMagicController _magicObjects;
    SpellCasting _spellCasting;

    public Transform 
        magicCircleSpawn, 
        magicChargesSpawn, 
        magicProjectileSpawn, 
        magicAccelEffectSpawn, 
        effectBurstWhenCast;
    
    [HideInInspector] 
    public bool 
        magicActive, 
        canCastSpell, 
        concentrationSpell, 
        castingSpell, 
        maxCharge;
    
    [HideInInspector] 
    public GameObject 
        spellReadyVisual, 
        sorcerySpellReadyEffect;
   
    Vector3 
        startSpellPos, 
        startConcentrationPos;

    ParticleSystem magicFocusCharges;
    bool tickDownMagicFocus;
    float cooldownTimer;
    int currentMagic;
    GameObject currentConcentrationSpell;

    private void Start()
    {
        _player = LocalGameManager.Instance.player;

        _playerComponents = _player.GetPlayerComponents();
        _hand = GetComponent<VRHand>();

        _magicObjects = MasterManager.playerMagicController;

        PlayerCardContnroller.newSorcerer += NewSorcerer;
    }

    public void NewSorcerer(Sorcerer newSorcerer)
    {
        _spellCasting = newSorcerer.GetSpellCasting();
    }

    public void LateUpdate()
    {
        if (_spellCasting != null && castingSpell)
        {
            if (Vector3.Distance(startConcentrationPos, _hand.transform.localPosition) > 0.5f)
            {
                castingSpell = false;
                Destroy(currentConcentrationSpell);
            }

            var maxParticles = magicFocusCharges.main;

            if (maxParticles.maxParticles > 0)
                ConcentrationSpellCastTime();

            else
            {
                castingSpell = false;
                Destroy(currentConcentrationSpell);
            }
        }
    }

    public void SorcererMagic()
    {
        currentMagic = MagicController.Instance.magicIdx;
        magicFocusCharges = spellReadyVisual.GetComponent<ParticleSystem>();

        var maxParticles = magicFocusCharges.main;

        if (maxParticles.maxParticles == PlayerStats.Instance.data.magicFocus) { maxCharge = true; }

        else { maxCharge = false; }

        if (maxParticles.maxParticles <= 0) 
        {
            Destroy(sorcerySpellReadyEffect);
            ResetSpell(); 
        }

        else if (!canCastSpell && maxParticles.maxParticles > 0)
        {
            if (_hand.GetHandAcceleration().magnitude > _spellCasting.accelerationReq)
            {
                startSpellPos = transform.localPosition;
                canCastSpell = true;
            }
        }

        else if (canCastSpell && _hand.GetHandAcceleration().magnitude <= _spellCasting.stoppingForce)
        {
            canCastSpell = false;
            if (Vector3.Distance(startSpellPos, transform.localPosition) >= _spellCasting.distanceReq && Vector3.Distance(_playerComponents.GetOriginPoint(0).transform.position, transform.position) *100 > _spellCasting.distanceBetweenHandAndChest) 
            {
                CastSorcererSpells();
            }
        }
    }

    private async void CastSorcererSpells()
    {
        if (PlayerStats.Instance.currentMagicFocus <= 0)
            return;

        else
        {
            PlayerStats.Instance.currentMagicFocus--;

            CastChargedSpell(MagicController.Instance.magicIdx);

            await Task.Delay(250);

            CastSorcererSpells();
        }
    }

    public void ResetSpell()
    {
        Destroy(spellReadyVisual);
        magicActive = false;
    }

    public void CastChargedSpell(int magicType)
    {
        var maxParticles = magicFocusCharges.main;
        maxParticles.maxParticles += -1;

        Instantiate(_magicObjects.effectBurstWhenCast[magicType], effectBurstWhenCast.position, effectBurstWhenCast.rotation);
        GameObject obj = Instantiate(_magicObjects.sorcererChargedSpells[magicType], magicProjectileSpawn.position, magicProjectileSpawn.rotation);

        obj.GetComponent<BasicProjectile>().player = _player;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.velocity = _hand.GetHandVelocity();
        rb.angularVelocity = _hand.GetHandAngularVelocity();
    }

    public void CastBeam(int magicType)
    {
        currentConcentrationSpell = Instantiate(_magicObjects.sorcererConstantSpells[magicType]);
        currentConcentrationSpell.GetComponent<BasicBeamAttack>().player = _player;
        currentConcentrationSpell.transform.SetParent(magicProjectileSpawn);
        currentConcentrationSpell.transform.localPosition = new Vector3(0, 0, 0);
        currentConcentrationSpell.transform.localEulerAngles = new Vector3(0, 0, 0);
        currentConcentrationSpell.transform.localScale = new Vector3(1, 1, 1);
        startConcentrationPos = _hand.transform.localPosition;
        castingSpell = true;
    }

    public void CastRapidFireSpell(int magicType)
    {
        var maxParticles = magicFocusCharges.main;
        maxParticles.maxParticles += -1;
        Instantiate(_magicObjects.effectBurstWhenCast[magicType], effectBurstWhenCast.position, effectBurstWhenCast.rotation);
        GameObject obj = Instantiate(_magicObjects.sorcererRapidFireSpells[magicType], magicProjectileSpawn.position, magicProjectileSpawn.rotation);
        obj.transform.SetParent(null);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.velocity = _hand.GetHandVelocity();
        rb.angularVelocity = _hand.GetHandAngularVelocity();
    }

    public void ConcentrationSpellCastTime()
    {
        if (tickDownMagicFocus)
        {
            cooldownTimer = 2f;
            tickDownMagicFocus = false;
        }

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        else if (cooldownTimer <= 0)
        {
            cooldownTimer = 0;
            var maxParticles = magicFocusCharges.main;
            maxParticles.maxParticles += -1;
            PlayerStats.Instance.currentMagicFocus--;
            tickDownMagicFocus = true;
        }
    }
}
