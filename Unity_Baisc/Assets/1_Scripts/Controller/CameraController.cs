using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField] Vector3 _delta;
    [SerializeField] GameObject _player;

    // PlayerController에서는 Update에서 이동하므로 player를 쫒아가는 카메라는 LateUpdate에서 추적 코드 실행 
    // 같이 Update에서 실행하면 중간에 카메라가 떨림
    void LateUpdate() 
    {
        if(_mode == Define.CameraMode.QuarterView)
        {
            if(Physics.Raycast(_player.transform.position, _delta, out RaycastHit hit, _delta.magnitude, LayerMask.GetMask("Wall")))
            {
                float distance = (hit.point - _player.transform.position).magnitude * 0.8f;
                transform.position = _player.transform.position + _delta.normalized * distance; // 중간에 벽이 있어서 거리는 임의로 조정해야 하지만 방향은 기존에 _dalta 변수 사용
            }
            else
            {
                transform.position = _player.transform.position + _delta;
                //transform.LookAt(_player.transform);
            }
        }
    }

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
