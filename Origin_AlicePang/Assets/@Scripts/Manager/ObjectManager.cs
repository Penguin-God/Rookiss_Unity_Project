using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using static Define;

public class ObjectManager
{


	const float START_DEGREE = DELTA_DEGREE / 2;
	const float DELTA_DEGREE = (2 * Mathf.PI / 24);
	const int SLICE_COUNT = 24;
	const int LAYER_COUNT = 6;

	// 셀 좌표 미리 계산해둔다
	Vector3[] _pos = new Vector3[SLICE_COUNT * LAYER_COUNT];
	public Vector3[] Pos { get { return _pos; } }

	MonsterController[] _monsters = new MonsterController[SLICE_COUNT * LAYER_COUNT];
	public MonsterController[] Monsters { get { return _monsters; } }

	BossController _boss;
	public BossController Boss { get { return _boss; } }

	PlayerController _player;
	public PlayerController Player { get { return _player; } set { _player = value; } }

	CameraController _camera;
	public CameraController Camera
	{
		get
		{
			if (_camera == null)
				_camera = GameObject.Find("Main Camera").GetComponent<CameraController>();

			return _camera;
		}
	}

	List<int> FreeIndex = new List<int>();

	List<RespawnData> _respawnData;
	int[] _point;

	float interval = 0f;

	List<Coin> _coins = new List<Coin>();

	int _remainRespawnTurn = 0;

    public ObjectManager()
	{
		Init();
	}

	public void Init()
	{
		float[] radiusArray = new float[LAYER_COUNT] { 1.55f, 2.25f, 3.0f, 3.68f, 4.35f, 5.0f };

		for (int layer = 0; layer < LAYER_COUNT; layer++)
		{
			float radius = radiusArray[layer];

			for (int slice = 0; slice < SLICE_COUNT; slice++)
			{
				Vector3 pos;

				float angle = START_DEGREE + slice * DELTA_DEGREE;
				float sin = Mathf.Sin(angle);
				float cos = Mathf.Cos(angle);

				if (sin < 0f)
					pos = new Vector3(radius * cos, radius * sin - Mathf.Pow(0.2f, -1 / sin) - 2.2f, radius * sin - Mathf.Pow(0.2f, -1 / sin) - 2.2f);
				else
					pos = new Vector3(radius * cos, radius * sin - Mathf.Pow(0.1f, 1 / sin) - 2.2f, radius * sin - Mathf.Pow(0.1f, 1 / sin) - 2.2f);

				_pos[layer * SLICE_COUNT + slice] = pos;
			}
		}
	}

	public void LoadStageData(StageData stageData)
	{
		_respawnData = stageData.respawnData;
		_respawnData.Sort((RespawnData x, RespawnData y) => y.SummonPoint.CompareTo(x.SummonPoint));

		int bossTemplateID = stageData.BossTemplateID;
		if (bossTemplateID > 0)
        {
			//보스 사망에 관한 미션 목표 처리 필요
            //보스 있으므로 보스 생성
            if (Managers.Data.Bosses.TryGetValue(bossTemplateID, out BossData bossData) == false)
                return;

			Managers.Resource.Instantiate(bossData.Prefab, callback: (go) =>
			{
				BossController bc = go.GetOrAddComponent<BossController>();
				bc.SetInfo(bossData);
				_boss = bc;
			});
        }
        else
        {
			_boss = null;
        }

		_point = new int[_respawnData.Count];
		_remainRespawnTurn = 0;
	}

	public List<MonsterController> GetMonsters()
	{
		return _monsters.Where(m => { return m != null; }).OrderBy(m => m.GetDistance(Vector2.zero)).ToList();
	}

	public List<MonsterController> GetLowestHpSelectedMonsters()
	{
		return _monsters.Where(m => { return m != null && m.Selected; }).OrderBy(x => x.Hp).ToList();		//무기에 따라 정렬 규칙이 달라질 경우 수정 필요
	}

	public List<MonsterController> GetHighestTurnMonsters()
    {
		return _monsters.Where(m => { return m != null; }).OrderByDescending(x=> x.MoveTurnRemaining).ToList();
    }

	public void ShowDamageText(Vector2 pos, int damage)
	{
		Managers.Resource.Instantiate("DamageText", callback: (go) =>
		{
			DamageText damageText = go.GetOrAddComponent<DamageText>();
			damageText.SetInfo(pos, damage);
		});
	}

	public void DropCoin(Vector2 dropPos, int amount = 1)
    {
		Coin coin = _coins.Find(obj => !obj.gameObject.activeSelf);
		
		if (coin == null)
		{
			Managers.Resource.Instantiate("Coin", callback: (go) =>
			{
				coin = Utils.GetOrAddComponent<Coin>(go);
				_coins.Add(coin);
				coin.SetInfo(dropPos, amount);				
			});
		}
		else
		{
			coin.SetInfo(dropPos, amount);
		}
	}

    public void SpawnPlayer(string key, Vector2 pos)
	{
		Managers.Resource.Instantiate(key, callback: (go) =>
		{
			PlayerController pc = go.GetOrAddComponent<PlayerController>();
			pc.SetInfo(pos);
			_player = pc;
		});
	}

	public IEnumerator BossSummonMonster(Vector3 summonPosition, int templateID, int monsterCount)
    {
		if (Managers.Data.Monsters.TryGetValue(templateID, out MonsterData monsterData) == false)
			yield break;

        for (int i = 0; i < monsterCount; i++)
        {
			int randomLayer = Random.Range(2, LAYER_COUNT - 1);
			ClearFreeIndex(randomLayer);

			int spawnIndex = SpawnMonsterAtRandomPos();

            //float t = interval;

            Managers.Resource.Instantiate(monsterData.Prefab, callback: (go) =>
            {
                MonsterController mc = go.GetOrAddComponent<MonsterController>();
                mc.SetInfo(monsterData, 0.3f);
				
                _monsters[spawnIndex] = mc;
                mc.CellIndex = spawnIndex;

                mc.SetPos(summonPosition);
				Debug.Log(summonPosition);
				mc.transform.DOJump(_pos[spawnIndex], 5f, 1, 0.5f).PrependInterval(0.5f);
            });

        }
		yield return null;
    }
	
	public IEnumerator SpawnMonster(int layer)
	{
		if (_remainRespawnTurn > 0)
		{
			int remainMonstersCount = _monsters.ToList().FindAll(obj => (obj != null && obj.Hp > 0)).Count;

			if (remainMonstersCount > 3)
			{
				_remainRespawnTurn--;
				yield break;
			}
        }
		
		int maxMonsterCount = 9;
		int monsterCount = 0;
		interval = 0f;
		for (int i = 0; i < _respawnData.Count; i++)
		{
			int random = Random.Range(_respawnData[i].MinPoint, _respawnData[i].MaxPoint);

			_point[i] += random;
			//SummonPoint != 0
			int count = 0;

			if (_respawnData[i].SummonPoint != 0)
			{
				count = _point[i] / _respawnData[i].SummonPoint;
				_point[i] = _point[i] % _respawnData[i].SummonPoint;
			}
			else
			{
				count = maxMonsterCount - monsterCount;
			}

			for (int j = 0; j < count; j++)
			{
				SpawnMonsterAtRandomPos(_respawnData[i].MonsterID);
				yield return new WaitForSeconds(0.05f);
			}

			monsterCount += count;

			if (monsterCount >= maxMonsterCount)
				yield return null;
		}
		_remainRespawnTurn = 3;
	}

	public void SpawnMonsterAtRandomPos(int templateID)
	{
		if (Managers.Data.Monsters.TryGetValue(templateID, out MonsterData monsterData) == false)
			return;

		int spawnIndex = SpawnMonsterAtRandomPos();
		if (spawnIndex == -1)
			return;

		interval += 0.1f;

		float t = interval;
		FreeIndex.Remove(spawnIndex);
		Managers.Resource.Instantiate(monsterData.Prefab, callback: (go) =>
		{
			MonsterController mc = go.GetOrAddComponent<MonsterController>();
			mc.SetInfo(monsterData, t);

			_monsters[spawnIndex] = mc;
			mc.CellIndex = spawnIndex;
			mc.SetPos(_pos[spawnIndex]);
		});
		
	}

	public void ClearFreeIndex(int layer)
	{
		FreeIndex.Clear();

		int startIndex = layer * SLICE_COUNT;

		for (int slice = 0; slice < SLICE_COUNT; slice++)
		{
			int index = startIndex + slice;
			if (_monsters[index] != null)
				continue;

			FreeIndex.Add(index);
		}
	}

	int SpawnMonsterAtRandomPos()
	{
		if (FreeIndex.Count == 0)
		{
			Debug.LogError("SpawnMonsterAtRandomPos Failed");
			return -1;
		}

		int spawnIndex = FreeIndex[Random.Range(0, FreeIndex.Count)];

		return spawnIndex;
	}

	public void DespawnMonster(MonsterController mc)
	{
		if (mc == null)
		{
			return;
		}
		if (mc.CellIndex == -1)
		{
			Debug.LogError("DespawnMonster Failed");
			return;
		}
		_monsters[mc.CellIndex] = null;
		Managers.Resource.Destroy(mc.gameObject);
	}

	public void ResetStageObjects()
	{
		_coins.Clear();

		foreach (MonsterController monster in _monsters)
			DespawnMonster(monster);
	}

	public int TryMoveForward(MonsterController mc, int distance = 1)
	{
		if (mc.CellIndex == -1)
		{
			Debug.LogError("TryMoveForward Failed");
			return -1;
		}

		int index = GetFront(mc.CellIndex, distance);
		if (index == -1)
			return -1;

		// 앞이 비어있다면 이동
		if (_monsters[index] == null)
		{
			// 기존 자리 비우기
			_monsters[mc.CellIndex] = null;
			// 다음 칸으로 이동
			_monsters[index] = mc;
			mc.CellIndex = index;
			// 목적지로 설정
			

			return index;
		}

		return index;
	}

	public int TryMoveBackward(MonsterController mc)
	{
		if(mc.CellIndex == -1)
		{
			Debug.LogError("TryMoveBackward Failed");
			return -1;
		}

		int index = GetBack(mc.CellIndex);
		if (index == -1)
			return -1;

		// 뒤가 비어있다면 이동
		if (_monsters[index] == null)
		{
			// 기존 자리 비우기
			_monsters[mc.CellIndex] = null;
			// 다음 칸으로 이동
			_monsters[index] = mc;
			mc.CellIndex = index;
			// 목적지로 설정
			//mc.SetDestination(_pos[index]);
			return index;
		}

		return index;
	}

	public int TryMoveClockwise(MonsterController mc, bool clockwise,int distance = 1)
	{
		if (mc.CellIndex == -1)
		{
			Debug.LogError("TryMoveBackward Failed");
			return -1;
		}

		int index = GetSide(mc.CellIndex, clockwise);
		if (index == -1)
			return -1;

		// 옆 자리가 비어있다면 이동
		if (_monsters[index] == null)
		{
			// 기존 자리 비우기
			_monsters[mc.CellIndex] = null;
			// 다음 칸으로 이동
			_monsters[index] = mc;
			mc.CellIndex = index;
			// 목적지로 설정
			//mc.SetDestination(_pos[index]);
		}
		return index;
	}

	public int GetFront(int cellIndex, int distance = 1)
	{
		int layer = cellIndex / SLICE_COUNT;
		int slice = cellIndex % SLICE_COUNT;

		if (layer == 0)
			return -1;
		for (; distance > 0; distance--)
		{
			int index = (layer - distance) * SLICE_COUNT + slice;
			if (index < 0)
				continue;

			if (_monsters[index] != null)
				continue;

			if (index > 0)
				return index;
			
		}
		return -1;
	}

	public int GetSide(int cellIndex, bool clockwise, int distance = 1)
	{
		int layer = cellIndex / SLICE_COUNT;
		int slice = cellIndex % SLICE_COUNT;

		int index;
		for (; distance > 0; distance--)
		{
			if (clockwise)
				index = layer * SLICE_COUNT + (slice - distance) % SLICE_COUNT;
			else
				index = layer * SLICE_COUNT + (slice + distance) % SLICE_COUNT;

			if (_monsters[index] != null)
				continue;

			if (index > 0)
				return index;
		}
		return -1;
	}

	public int GetBack(int cellIndex, int distance = 1)
	{
		int layer = cellIndex / SLICE_COUNT;
		int slice = cellIndex % SLICE_COUNT;

		for (; distance > 0; distance--)
		{
			if (layer >= LAYER_COUNT - distance)
				continue;
			
			int index = (layer + distance) * SLICE_COUNT + slice;
			if (_monsters[index] != null)
				continue;

			return index;
		}

		return -1;
	}

	public void Clear()
	{

	}
}
