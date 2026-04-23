using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class GlitchEffect : MonoBehaviour
{
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
        if (Camera.main != null)
            cameraData = Camera.main.GetComponent<UniversalAdditionalCameraData>();
        
        globalVolume = FindFirstObjectByType<Volume>();
    }

      public void Play(float glitchTime)
    {
        if (cameraData == null || globalVolume == null) return;
        
        StopAllCoroutines();
        StartCoroutine(GlitchEffectRoutine(glitchTime));
    }

    private IEnumerator GlitchEffectRoutine(float glitchTime)
    {
        cameraData.SetRenderer(glitchRendererIndex);
        globalVolume.profile = glitchProfile;

        yield return new WaitForSeconds(glitchTime);

        cameraData.SetRenderer(normalRendererIndex);
        globalVolume.profile = normalProfile;
    }
}