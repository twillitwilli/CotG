using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBow : MonoBehaviour
{
    bool 
        _holdingBow, 
        _holdingString;

    VRHand 
        _primaryHand, 
        _offHand;

    bool 
        _setNormalChargeCooldown, 
        _setDoubleChargeCooldown, 
        _chargingArrow;

    float 
        _normalChargeTimer, 
        _doubleChargeTimer, 
        _stringPullDistance;

    [SerializeField] 
    Transform 
        _arrowSpellSpawn, 
        _boneParent, 
        _chargingEffectSpawn, 
        _handOnStringSpawn;
    
    [SerializeField] 
    GameObject 
        _stringBone, 
        _normalChargeArrow, 
        _doubleChargeArrow, 
        _arrowReady; 
    
    [SerializeField] 
    Vector3 _offsetBowDirection;
    
    [SerializeField] 
    BoxCollider _bowCollider;
    
    [SerializeField] 
    CapsuleCollider _stringTrigger;
    
    [SerializeField] 
    Transform _stringDefaultPos;

    bool 
        _stringNormalCharge, 
        _stringDoubleCharge, 
        _removedFromParent, 
        _grabbedString;

    GameObject _spawnedChargingEffect;
    
    Vector3 
        _defaultPos, 
        _defaultRot;

    void Start()
    {
        SetHands();

        UpdateOffsetDirection();

        _setNormalChargeCooldown = true;
        _setDoubleChargeCooldown = true;
    }

    public void LateUpdate()
    {
        if (_holdingBow && _holdingString)
        {
            if (!_grabbedString)
                GrabString();

            HoldingString();

            if (!_removedFromParent)
                transform.SetParent(null); _removedFromParent = true;

            transform.position = _offHand.transform.position;

            FaceAwayFromObject();

            if (!_stringNormalCharge && NormalChargeCooldown())
            {
                _normalChargeArrow.SetActive(true);
                _arrowReady.SetActive(true);
                _stringNormalCharge = true;
            }

            if (!_stringDoubleCharge && Vector3.Distance(_stringBone.transform.position, _stringDefaultPos.position) > 0.3f)
            {
                if (DoubleChargeCooldown()) 
                {
                    _doubleChargeArrow.SetActive(true);
                    _stringDoubleCharge = true;
                }
            }
        }
    }

    void SetHands()
    {
        PlayerComponents playerComponents = LocalGameManager.Instance.player.GetPlayerComponents();

        if (!LocalGameManager.Instance.player.isLeftHanded)
        {
            _primaryHand = playerComponents.GetHand(1);
            _offHand = playerComponents.GetHand(0);
        }

        else
        {
            _primaryHand = playerComponents.GetHand(0);
            _offHand = playerComponents.GetHand(1);
        }
    }

    public void UpdateOffsetDirection()
    {
        if (!LocalGameManager.Instance.player.isLeftHanded)
            _offsetBowDirection = new Vector3(0, 0, 180);

        else
            _offsetBowDirection = new Vector3(0, 0, 0);
    }

    public void FaceAwayFromObject()
    {
        //transform.LookAt(player.playerComponents.hand[player.primaryHand].transform.position, transform.forward);

        Vector3 direction = (_primaryHand.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));

        transform.rotation = lookRotation;
        transform.localEulerAngles -= _offsetBowDirection;
    }

    public void ArrowReleased()
    {
        if (NormalChargeCooldown())
        {
            int currentSpell = MagicController.Instance.magicIdx;

            if (DoubleChargeCooldown())
                ShootArrow(true, currentSpell);

            else
                ShootArrow(false, currentSpell);
        }

        ResetString();
    }

    public void ShootArrow(bool addPeircing, int whichSpell)
    {
        GameObject newProjectile;

        switch (MagicController.Instance.currentCastingType)
        {
            case MagicController.CastingType.charge:
                newProjectile = Instantiate(MasterManager.Instance.magicController.conjurerChargedSpells[MagicController.Instance.magicIdx], _arrowSpellSpawn.position, _arrowSpellSpawn.rotation);
                newProjectile.transform.SetParent(null);

                BasicProjectile arrowAttack = newProjectile.GetComponent<BasicProjectile>();

                if (_stringPullDistance > -0.03)
                    arrowAttack.rb.useGravity = true;

                if (addPeircing)
                {
                    arrowAttack.tempPeircing = true;
                    arrowAttack.projectileSpeed += 4;
                }
                break;

            case MagicController.CastingType.rapidFire:
                break;

            case MagicController.CastingType.beam:
                break;
        }
    }

    public bool NormalChargeCooldown()
    {
        if (_setNormalChargeCooldown)
        {
            _normalChargeTimer = PlayerStats.Instance.data.attackCooldown;
            _setNormalChargeCooldown = false;
        }

        if (_normalChargeTimer > 0)
            _normalChargeTimer -= Time.deltaTime;

        else if (_normalChargeTimer <= 0)
        {
            if (_spawnedChargingEffect != null)
                Destroy(_spawnedChargingEffect);

            _normalChargeTimer = 0;
            return true;
        }

        return false;
    }

    public bool DoubleChargeCooldown()
    {
        if (_setDoubleChargeCooldown)
        {
            _doubleChargeTimer = (PlayerStats.Instance.data.attackCooldown + 1);
            _setDoubleChargeCooldown = false;
        }

        if (_doubleChargeTimer > 0)
            _doubleChargeTimer -= Time.deltaTime;

        else if (_doubleChargeTimer <= 0)
        {
            if (_spawnedChargingEffect != null)
                Destroy(_spawnedChargingEffect);

            _doubleChargeTimer = 0;
            return true;
        }

        return false;
    }

    public void GrabBow(Transform spawnLocation)
    {
        _holdingBow = true;
        transform.SetParent(spawnLocation);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);

        _bowCollider.enabled = false;
        _stringTrigger.enabled = true;

        transform.SetParent(_offHand.transform);

        _defaultPos = transform.localPosition;
        _defaultRot = transform.localEulerAngles;
    }

    public void GrabString()
    {
        _spawnedChargingEffect = Instantiate(MasterManager.Instance.magicController.chargedVisual[MagicController.Instance.magicIdx], _chargingEffectSpawn);

        _spawnedChargingEffect.transform.SetParent(_chargingEffectSpawn);
        _spawnedChargingEffect.transform.localScale = new Vector3(6, 6, 6);
        _spawnedChargingEffect.transform.localPosition = new Vector3(0, 0, 0);

        GameObject stringHand = _primaryHand.GetHandModel();
        stringHand.transform.SetParent(_handOnStringSpawn);
        stringHand.transform.localPosition = new Vector3(0, 0, 0);

        if (LocalGameManager.Instance.player.isLeftHanded)
            stringHand.transform.localEulerAngles = new Vector3(0, 0, 0);

        else
            stringHand.transform.localEulerAngles = new Vector3(0, 180, 0);

        _primaryHand.GetHandAnimationState().SwitchHandState(HandAnimationState.HandState.holdingBowString);

        _grabbedString = true;
    }

    public void BowDropped()
    {
        GameObject stringHand = _primaryHand.GetHandModel();
        stringHand.transform.SetParent(_primaryHand.transform);
        _primaryHand.ResetHandAlignment();

        _holdingBow = false;
        ResetString();
        _holdingBow = false;
        _holdingString = false;
        _bowCollider.enabled = true;
        _stringTrigger.enabled = false;

        BowMagicController.Instance.ResetToBack();
    }

    public void HoldingString()
    {
        _stringPullDistance = (-0.1f * (Vector3.Distance(_primaryHand.transform.position, _stringDefaultPos.position)));
        _stringPullDistance = Mathf.Clamp(_stringPullDistance, -0.05f, -0.0164f);
        _stringBone.transform.localPosition = new Vector3(0, _stringPullDistance, 0);
    }

    public void ResetString()
    {
        _grabbedString = false;
        _holdingString = false;

        _arrowReady.SetActive(false);
        _normalChargeArrow.SetActive(false);
        _doubleChargeArrow.SetActive(false);

        _setNormalChargeCooldown = true;
        _setDoubleChargeCooldown = true;
        _stringNormalCharge = false;
        _stringDoubleCharge = false;

        _stringBone.transform.localPosition = _stringDefaultPos.localPosition;

        _primaryHand.GetHandModel().transform.SetParent(_primaryHand.transform);
        _primaryHand.ResetHandAlignment();
        _primaryHand.GetHandAnimationState().SwitchHandState(HandAnimationState.HandState.idle);

        transform.SetParent(_offHand.transform);
        transform.localPosition = _defaultPos;
        transform.localEulerAngles = _defaultRot;

        _removedFromParent = false;

        if (_spawnedChargingEffect != null) { Destroy(_spawnedChargingEffect); }
    }
}
