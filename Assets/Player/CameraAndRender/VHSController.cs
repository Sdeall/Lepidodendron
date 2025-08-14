using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// VHSController: attaches to the RawImage that shows the low-res RenderTexture.
/// It creates an instance material from the "Unlit/VHSUnlit" shader and updates
/// runtime parameters (jitter, time, etc.) so the UI remains smooth while the camera feed updates at low FPS.
/// </summary>
[RequireComponent(typeof(RawImage))]
public class VHSController : MonoBehaviour
{
    [Header("Shader")]
    public Shader vhsShader; // set via inspector or left empty to Find at runtime

    [Header("Tweakables")]
    [Range(0f, 1f)] public float noiseIntensity = 0.12f;
    public float noiseScale = 200f;
    [Range(0f, 1f)] public float scanlineIntensity = 0.25f;
    public float scanlineCount = 400f;
    public float scanlineSpeed = 1f;
    [Range(0f, 0.02f)] public float chromatic = 0.003f;
    [Range(0f, 0.05f)] public float wobbleAmp = 0.01f;
    public float wobbleFreq = 8f;
    [Range(0f, 1f)] public float vignette = 0.25f;
    [Range(0f, 1f)] public float desaturation = 0f;
    [Range(0.5f, 2f)] public float contrast = 1.1f;

    [Header("Jitter settings")]
    public float jitterSpeed = 10f;
    public float jitterAmount = 1.0f;

    RawImage rawImage;
    Material materialInstance;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();

        if (vhsShader == null)
            vhsShader = Shader.Find("Unlit/VHSUnlit");

        if (vhsShader == null)
        {
            Debug.LogError("VHSController: shader 'Unlit/VHSUnlit' not found. Make sure you imported VHSUnlit.shader.");
            enabled = false;
            return;
        }

        // create instance so editing params at runtime doesn't change the project material
        materialInstance = new Material(vhsShader);
        // assign material to RawImage (it will still use RawImage.texture assigned by PlayerCamera)
        rawImage.material = materialInstance;

        // initial values
        UpdateMaterialProperties(0f);
    }

    void Update()
    {
        float t = Time.unscaledTime; // use unscaled so UI jitter continues if game slows
        // jitter based on Perlin noise for smoothness
        float jx = (Mathf.PerlinNoise(t * jitterSpeed, 0.0f) - 0.5f) * 2f * jitterAmount;
        float jy = (Mathf.PerlinNoise(0.0f, t * jitterSpeed * 0.7f) - 0.5f) * 2f * jitterAmount;

        // small additional randomness for kicks
        Vector4 jitter = new Vector4(jx, jy, 0f, 0f);

        UpdateMaterialProperties(t, jitter);
    }

    void UpdateMaterialProperties(float time, Vector4? jitter = null)
    {
        if (materialInstance == null) return;

        materialInstance.SetFloat("_NoiseIntensity", noiseIntensity);
        materialInstance.SetFloat("_NoiseScale", noiseScale);
        materialInstance.SetFloat("_ScanlineIntensity", scanlineIntensity);
        materialInstance.SetFloat("_ScanlineCount", scanlineCount);
        materialInstance.SetFloat("_ScanlineSpeed", scanlineSpeed);
        materialInstance.SetFloat("_Chromatic", chromatic);
        materialInstance.SetFloat("_WobbleAmp", wobbleAmp);
        materialInstance.SetFloat("_WobbleFreq", wobbleFreq);
        materialInstance.SetFloat("_Vignette", vignette);
        materialInstance.SetFloat("_Desaturation", desaturation);
        materialInstance.SetFloat("_Contrast", contrast);
        materialInstance.SetFloat("_TimeGlobal", time);

        if (jitter.HasValue)
            materialInstance.SetVector("_Jitter", jitter.Value);
    }

    void OnDestroy()
    {
        if (materialInstance != null)
            Destroy(materialInstance);
    }
}