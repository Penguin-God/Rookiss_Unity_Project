using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleCreature
{
    void AttackHitEvent();
    void OnDamaged(int damage);
}
