using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class GlitchEffect : MonoBehaviour
{
    // 어디서든 편하게 부를 수 있게 싱글톤 처리 (선택 사항)
    public static GlitchEffect Instance;

    [Header("Renderer Settings")]
    [SerializeField] private int normalRendererIndex = 1;
    [SerializeField] private int glitchRendererIndex = 0;

    [Header("Volume Settings")]
    [SerializeField] private VolumeProfile normalProfile; 
    [SerializeField] private VolumeProfile glitchProfile;

    private UniversalAdditionalCameraData cameraData;
    private Volume globalVolume;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 메인 카메라의 URP 데이터 가져오기
        if (Camera.main != null)
            cameraData = Camera.main.GetComponent<UniversalAdditionalCameraData>();
        
        // 씬에 배치된 메인 볼륨 찾기
        globalVolume = FindFirstObjectByType<Volume>();
    }

    /// <summary>
    /// 기믹 실패 시 호출: 지정된 시간 동안 화면을 지지직거리게 만듭니다.
    /// </summary>
    public void Play(float duration = 0.3f)
    {
        if (cameraData == null || globalVolume == null) return;
        
        StopAllCoroutines();
        StartCoroutine(GlitchSequence(duration));
    }

    private IEnumerator GlitchSequence(float duration)
    {
        // 1. Glitch 상태로 전환
        cameraData.SetRenderer(glitchRendererIndex);
        globalVolume.profile = glitchProfile;

        // 2. 유지
        yield return new WaitForSeconds(duration);

        // 3. 원래 상태로 복구
        cameraData.SetRenderer(normalRendererIndex);
        globalVolume.profile = normalProfile;
    }
}