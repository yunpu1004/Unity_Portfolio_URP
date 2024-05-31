using System;
using UnityEngine;

// 이 스크립트는 기능 테스트를 위해 사용됩니다.
public class PhysicsTest : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other) {
        Debug.Log("OnTriggerEnter");
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("OnTriggerExit");
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("OnCollisionEnter");
    }

    private void OnCollisionExit(Collision other) {
        Debug.Log("OnCollisionExit");
    }
}
