using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CreatureState
{
    Idle,
    Moveing,
    Die,
    Battle,
}

public abstract class BaseController : MonoBehaviour
{
    [SerializeField] private CreatureState _state = CreatureState.Idle;
    [SerializeField] protected Vector3 _destination;

    protected Animator _anim;
    [SerializeField] protected GameObject _lockTarget = null;
    protected BaseController TargetController => _lockTarget.GetComponent<BaseController>();

    public virtual bool IsDead { get; } = false;

    protected CreatureState State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (_state)
            {
                case CreatureState.Idle:
                    _anim.CrossFade("WAIT", 0.2f);
                    break;
                case CreatureState.Moveing:
                    _anim.CrossFade("RUN", 0.1f);
                    break;
                case CreatureState.Battle:
                    _anim.CrossFade("ATTACK", 0.1f);
                    break;
            }
        }
    }

    protected abstract void Init();
    void Start()
    {
        _anim = GetComponent<Animator>();
        Init();
        if (GetComponentInChildren<UI_HpBar>() == null)
            Managers.UI.MakeWorldSapce_UI<UI_HpBar>(transform);
    }

    void Update()
    {
        switch (State)
        {
            case CreatureState.Idle: UpdateIdle(); break;
            case CreatureState.Moveing: UpdateMove(); break;
            case CreatureState.Battle: UpdateBattle(); break;
            default: break;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMove() { }
    protected virtual void UpdateBattle()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.3f);
        }
    }


    protected virtual void AttackHitEvent() { }
    public virtual void OnDamaged(int damage) { }
    protected virtual void DecideBattleOrNot() { }
}
