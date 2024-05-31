using System;
using UnityEngine;

// 이 스크립트는 기능 테스트를 위해 사용됩니다.
public class Test : MonoBehaviour
{
    public Rigidbody rb;
    public BoxCollider col;

    public void EditorTestMethod()
    {
        var pos = rb.position + new Vector3(0,0,0.1f);
        rb.transform.position = pos;
    }
}
