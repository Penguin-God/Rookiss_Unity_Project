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
    protected NavMeshAgent _nav;

    protected virtual CreatureState State
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
        _nav = GetComponent<NavMeshAgent>();
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

    [SerializeField] protected GameObject _lockTarget = null;
    protected virtual void UpdateBattle() { }
}
