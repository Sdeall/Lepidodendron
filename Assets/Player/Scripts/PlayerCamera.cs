using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Improved PlayerCamera with safer RenderTexture management, editor validation,
/// optional use of temporary RTs, and skipping renders when not needed.
/// Designed for a low-FPS "slideshow" camera feed while UI stays at normal FPS.
/// </summary>
[DisallowMultipleComponent]
public class PlayerCamera : MonoBehaviour
{
    [Header("RenderTexture settings")]
    public int targetWidth = 320;
    public int targetHeight = 180;
    public float updateInterval = 0.20f;
    public bool useTemporaryRT = false;

    [Header("References")]
    public RawImage renderOutput;

    Camera playerCamera;
    RenderTexture lowResRT;
    Coroutine autoRenderCoroutine;

    void OnValidate()
    {
        targetWidth = Mathf.Max(8, targetWidth);
        targetHeight = Mathf.Max(8, targetHeight);
        updateInterval = Mathf.Max(0f, updateInterval);

        if (!Application.isPlaying) return;

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera != null)
            Recreate(targetWidth, targetHeight);
    }

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("PlayerCamera: no Camera found in children.");
            enabled = false;
            return;
        }

        playerCamera.allowMSAA = false;
        playerCamera.allowHDR = false;
        playerCamera.depthTextureMode = DepthTextureMode.None;
        playerCamera.enabled = false;

        CreateRenderTexture();

        if (renderOutput != null)
            renderOutput.texture = lowResRT;
    }

    void OnEnable()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (lowResRT == null)
        {
            CreateRenderTexture();
            if (renderOutput != null) renderOutput.texture = lowResRT;
        }

        if (updateInterval > 0f)
            autoRenderCoroutine = StartCoroutine(AutoRenderLoop());
        else
            ForceRenderOnce();
    }

    void OnDisable()
    {
        StopAutoUpdates();

        if (playerCamera != null)
            playerCamera.targetTexture = null;

        if (useTemporaryRT)
        {
            if (lowResRT != null)
            {
                RenderTexture.ReleaseTemporary(lowResRT);
                lowResRT = null;
            }
        }
        else
        {
            if (lowResRT != null)
            {
                lowResRT.Release();
                Destroy(lowResRT);
                lowResRT = null;
            }
        }
    }

    void CreateRenderTexture()
    {
        if (lowResRT != null && lowResRT.width == targetWidth && lowResRT.height == targetHeight)
        {
            playerCamera.targetTexture = lowResRT;
            return;
        }

        if (lowResRT != null)
        {
            if (useTemporaryRT)
                RenderTexture.ReleaseTemporary(lowResRT);
            else
            {
                lowResRT.Release();
                Destroy(lowResRT);
            }
            lowResRT = null;
        }

        if (useTemporaryRT)
        {
            lowResRT = RenderTexture.GetTemporary(targetWidth, targetHeight, 16, RenderTextureFormat.Default);
        }
        else
        {
            lowResRT = new RenderTexture(targetWidth, targetHeight, 16, RenderTextureFormat.Default)
            {
                useMipMap = false,
                autoGenerateMips = false,
                filterMode = FilterMode.Point,
                antiAliasing = 1
            };
            lowResRT.Create();
        }

        playerCamera.targetTexture = lowResRT;

        if (renderOutput != null)
            renderOutput.texture = lowResRT;
    }

    IEnumerator AutoRenderLoop()
    {
        ForceRenderOnce();

        while (updateInterval > 0f)
        {
            if (!Application.isFocused || !gameObject.activeInHierarchy)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSecondsRealtime(updateInterval);
            ForceRenderOnce();
        }
    }

    public void ForceRenderOnce()
    {
        if (playerCamera == null) return;

        if (lowResRT == null) CreateRenderTexture();
        if (lowResRT == null) return;

        if (!Application.isFocused) return;

        playerCamera.Render();
    }

    public void StartAutoUpdates()
    {
        if (autoRenderCoroutine == null && updateInterval > 0f)
            autoRenderCoroutine = StartCoroutine(AutoRenderLoop());
    }

    public void StopAutoUpdates()
    {
        if (autoRenderCoroutine != null)
        {
            StopCoroutine(autoRenderCoroutine);
            autoRenderCoroutine = null;
        }
    }

    public void Recreate(int width, int height)
    {
        targetWidth = Mathf.Max(8, width);
        targetHeight = Mathf.Max(8, height);
        CreateRenderTexture();
        if (renderOutput != null) renderOutput.texture = lowResRT;
        ForceRenderOnce();
    }
}
