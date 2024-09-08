using UnityEngine;

// 이 스크립트는 미니맵 카메라가 플레이어를 내려다보도록 합니다.
public class MinimapCamera : MonoBehaviour
{
    public Transform playerTransform;
    public Camera playerFollowCamera;

    private void FixedUpdate() 
    {
        // 플레이어의 위치를 가져옵니다.
        var playerPos = playerTransform.position;
        var cameraPos = transform.position;

        // 카메라의 위치를 플레이어의 위치에 맞춥니다 (Y축은 변하지 않음).
        cameraPos.Set(playerPos.x, cameraPos.y, playerPos.z);
        transform.position = cameraPos;

        // 카메라의 회전을 가져옵니다.
        var cameraRotation = transform.rotation;
        var playerFollowCameraRotation = playerFollowCamera.transform.rotation;

        // 미니맵 카메라의 Y축 회전을 메인 카메라의 Y축 회전과 일치시킵니다.
        transform.rotation = Quaternion.Euler(cameraRotation.eulerAngles.x, playerFollowCameraRotation.eulerAngles.y, cameraRotation.eulerAngles.z);
    }
}
