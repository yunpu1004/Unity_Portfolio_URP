using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// 이 스크립트는 게임 내에서 사용되는 비주얼 이펙트를 관리합니다.
// 포스트 프로세싱, 임펄스 등의 시각적 효과를 관리합니다.
public class VisualEffectManager : MonoBehaviour
{
    public CinemachineImpulseSource hitImpulseSource;
    public CinemachineImpulseSource skillQImpulseSource;
    public CinemachineImpulseSource skillEImpulseSource;
    public Volume volume;

    public static VisualEffectManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void PlayHitEffect()
    {
        hitImpulseSource.GenerateImpulse();
    }

    public void PlaySkillQEffect()
    {
        skillQImpulseSource.GenerateImpulse();
    }

    public void PlaySkillEEffect()
    {
        skillEImpulseSource.GenerateImpulse();
    }

    public void SetVignetteIntensity(float intensity)
    {
        volume.profile.TryGet(out Vignette vignette);
        vignette.intensity.value = intensity;
    }
}
