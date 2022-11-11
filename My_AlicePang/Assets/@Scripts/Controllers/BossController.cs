using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using Spine.Unity;

public class BossController : BaseController
{
    public int TemplateID { get; set; }
    BossData _bossData;

    public bool IsBusy { get; set; }

    StatusBar _statusBar;

    Animator _animator;
    SkeletonAnimation _spineAni;

    public int MaxHp { get; set; }
    int _hp = 0;
    public int Hp
    {
        get { return _hp; }
        set
        {
            _hp = value;
            if (_statusBar != null && MaxHp > 0)
            {
                _statusBar.SetHp(_hp, MaxHp);
            }
        }
    }

    float _angleSpeed = RADAR_SPEED;

    int _turnRemaining;
    public int MoveTurnRemaining
    {
        get { return _turnRemaining; }
        set
        {
            _turnRemaining = value;
            if (_statusBar != null)
                _statusBar.SetTurnText(_turnRemaining);
        }
    }

    CreatureState _state = CreatureState.Idle;
    public CreatureState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (value)
            {
                case CreatureState.Idle:
                    break;
                case CreatureState.Moving:
                    break;
                case CreatureState.Attack:
                    break;
                case CreatureState.Dead:
                    //_spriteRenderer.DOFade(0f, 0.5f).SetEase(Ease.InSine).OnComplete(() => Managers.Game.MonsterDead(this));
                    break;
            }
        }
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _statusBar = Utils.FindChild<StatusBar>(gameObject, recursive: true);

        if (TemplateID < 6)
            _animator = Utils.GetOrAddComponent<Animator>(gameObject);
        else
            _spineAni = Utils.GetOrAddComponent<SkeletonAnimation>(gameObject);

        ResetMoveTurn();

        Hp = MaxHp;

        return true;
    }

    public void SetInfo(BossData bossData)
    {
        _bossData = bossData;
        transform.position = Vector3.up * bossData.PositionY;

        TemplateID = _bossData.TemplateID;

        if (TemplateID == 1)
        {
            Managers.Game.OnPlayerAttack -= StopRadarEffectSound;
            Managers.Game.OnPlayerAttack += StopRadarEffectSound;

            Managers.Game.OnPlayerInput -= PlayRadarEffectSound;
            Managers.Game.OnPlayerInput += PlayRadarEffectSound;
        }

        MaxHp = bossData.Hp;
        Hp = MaxHp;

    }

    void ResetMoveTurn()
    {
        if (_init == false)
            return;

        //MoveTurnRemaining = BossData.MoveTurn;
    }

    public void StartBossTurn()
    {
        Managers.Sound.Play(Sound.Effect, "Sound_ClockBoss");
        StartCoroutine(StartBossAttack());
        
    }

    public IEnumerator StartBossAttack()
    {
        IsBusy = true;
        //yield return null;
        if (TemplateID < 6)
            _animator.SetTrigger("UseSkill");
        else
            Managers.Resource.LoadAsync<SkeletonDataAsset>("Boss6_UseSkill", callback: (ani) =>
            {
                _spineAni.skeletonDataAsset = ani;
                _spineAni.loop = false;
                _spineAni.Initialize(true);
            });
        for (int i = 0; i < _bossData.bossSkillData.Count; i++)
        {
            SkillType skillID = _bossData.bossSkillData[i].SkillID;

            switch (skillID)
            {
                case SkillType.ControllRadar:
                    ChangeRadarSpeed();

                    break;

                case SkillType.SummonMonster:
                    SummonMonster(_bossData.bossSkillData[i]);
                    break;

                case SkillType.ReduceTurn:
                    ReduceMonsterTurn(_bossData.bossSkillData[i]);
                    break;

                case SkillType.LockWeapon:
                    LockTheWeapon();
                    break;
            }
        }

        yield return new WaitForSeconds(_bossData.UseSkillAnimationDuration);

        if (TemplateID < 6)
            _animator.SetTrigger("Idle");
        else
            Managers.Resource.LoadAsync<SkeletonDataAsset>("Boss6_Idle", callback: (ani) =>
            {
                _spineAni.skeletonDataAsset = ani;
                _spineAni.loop = true;
                _spineAni.Initialize(true);
            });
        IsBusy = false;
    }

    void UpdateIdle()
    {

    }

    void UpdateAttack()
    {
        //보스의 패턴 입력
    }

    void EndAttack()
    {

    }

    public void OnDamaged()
    {
        if (TemplateID < 6)
            _animator.SetTrigger("Damaged");
        else
            Managers.Resource.LoadAsync<SkeletonDataAsset>("Boss6_Damaged", callback: (ani) =>
            {
                _spineAni.skeletonDataAsset = ani;
                _spineAni.loop = false;
                _spineAni.Initialize(true);
            });
        if (Hp <= 0)
            OnDead();
    }

    void OnDead()
    {

    }

    void ChangeRadarSpeed()
    {
        int random = Random.Range(0, 3);
        _angleSpeed = RADAR_SPEED;

        switch (random)
        {
            case 0:
                //Managers.Sound.Stop(Sound.SubBgm);
                //Managers.Sound.Play(Sound.SubBgm, "Sound_RadarSlow");
                _angleSpeed *= 0.4f;
                break;
            case 1:
                
                //Managers.Sound.Play(Sound.SubBgm, "Sound_RadarFast");
                break;

            case 2:
                
                //Managers.Sound.Play(Sound.SubBgm, "Sound_RadarFast");
                _angleSpeed *= 3f;
                break;
        }

        Managers.Game.RadarAngleSpeed = _angleSpeed;
    }

    void SummonMonster(BossSkillData bossSkillData)
    {
        StartCoroutine(Managers.Object.BossSummonMonster(transform.position, 78, bossSkillData.MonsterCount));
        //int count = bossSkillData.MonsterCount;
        //for (int i = 0; i < count; i++)
        //{
        //    int layer = Random.Range(2, 5);
        //    Managers.Object.ClearFreeIndex(layer);
        //    Managers.Object.SpawnMonsterAtRandomPos(bossSkillData.Value);
        //}
    }

    void ReduceMonsterTurn(BossSkillData bossSkillData)
    {
        int count = Mathf.Min(Managers.Object.Monsters.Length, bossSkillData.Value);
        int value = bossSkillData.Value;
        List<MonsterController> monsters = Managers.Object.GetHighestTurnMonsters();
        for (int i = 0; i < count; i++)
        {
            monsters[i].MoveTurnRemaining = Mathf.Max(monsters[i].MoveTurnRemaining - value, 0);
        }
    }

    void LockTheWeapon()
    {
        int random = Random.Range(1, 4);

        if(random == (int)Managers.Game.WeaponType)
        {
            Managers.Game.WeaponType = (WeaponRangeType)((random + 1)%3 + 1);
        }

        Managers.Game.DisableWeaponRangeType = (WeaponRangeType)random;
    }

    void PlayRadarEffectSound()
    {
        if (_angleSpeed > RADAR_SPEED)
            Managers.Sound.Play(Sound.SubBgm, "Sound_RadarFast");
        else if (_angleSpeed < RADAR_SPEED)
            Managers.Sound.Play(Sound.SubBgm, "Sound_RadarSlow");
        else
            Managers.Sound.Stop(Sound.SubBgm);
    }

    void StopRadarEffectSound()
    {
        Managers.Sound.Stop(Sound.SubBgm);
    }
}
