using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
	public enum UIEvent
	{
		Click,
		Press
	}

	public enum SceneType
    {
        Unknown,
        GameScene,
		SelectStageScene,
		TitleScene,
	}

    public enum Sound
    {
        Bgm,
		SubBgm,
        Effect,
        Max,
    }

	public enum WeaponRangeType
	{
		None,
		Short,
		Middle,
		Long
	}

    public enum WeaponAttackType
    {
        Melee,
        Range,
        Trap,
    }

    public enum BattleState
	{
		Ready,
		PlayerInput,
		PlayerAttack,
		BossAttack,
		MonsterAttack,
		GameOver,
	}

	public enum CreatureState
	{
		Idle,
		Moving,
		Attack,
		Dead
	}

	public enum KnockbackDirection
	{
		Front,
		Back,
		Clockwise,
		AntiClockwise,
	}

	public const float START_DEGREE = DELTA_DEGREE / 2;
	public const float DELTA_DEGREE = (2 * Mathf.PI / 24);
	public const int SLICE_COUNT = 24;
	public const int LAYER_COUNT = 6;
	public const float RADAR_SPEED = 200f;

	public const int WEAPON_COUNT = 26;

	public const int DAILY_QUEST_COUNT = 5;

	public const string TEST_ID = "TEST";
}
