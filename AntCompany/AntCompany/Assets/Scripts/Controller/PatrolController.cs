using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolController : MonoBehaviour
{
    [SerializeField]
	GameObject _left;
    [SerializeField]
    GameObject _right;

	bool _movingLeft = true;

	public float MovingSpeed = 100.0f;
	public bool StopMove { get; set; } = false;
	public bool SwapLookDirection { get; set; } = true;

	void Start()
    {
		_movingLeft = true;
		if (SwapLookDirection)
			gameObject.GetOrAddComponent<BaseController>().LookLeft(true);

		transform.position = _right.transform.position;
	}

	const float EPSILLON = 50.0f;

    void Update()
    {
		if (StopMove)
			return;

        if (_movingLeft)
		{
			Vector3 dir = _left.transform.position - transform.position;

			// 목표 지점에 도착
			if (dir.magnitude < EPSILLON || dir.x > 0)
			{
				_movingLeft = false;
				if (SwapLookDirection)
					gameObject.GetOrAddComponent<BaseController>().LookLeft(false);

				return;
			}

			// 이동
			dir = dir.normalized;
			transform.position += MovingSpeed * dir * Time.deltaTime;
			return;
		}
		else
		{
			Vector3 dir = _right.transform.position - transform.position;

			// 목표 지점에 도착
			if (dir.magnitude < EPSILLON || dir.x < 0)
			{
				_movingLeft = true;
				if (SwapLookDirection)
					gameObject.GetOrAddComponent<BaseController>().LookLeft(true);

				return;
			}

			// 이동
			dir = dir.normalized;
			transform.position += MovingSpeed * dir * Time.deltaTime;
		}
    }
}
