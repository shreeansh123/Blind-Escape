using UnityEngine;
using UnityEngine.Rendering;
using System;

public class GoggleSystem : MonoBehaviour
{
    [Header("Visual References")]
    public Volume sonarVolume;
    public Camera mainCam;
    public Light sonarLight;

    [Header("Visibility Settings")]
    public Color sonarAmbientColor = new Color(0f, 0.3f, 0f);
    public float blindFog = 0.5f;
    public float sonarFog = 0.02f;

    [Header("Layer Masks")]
    public LayerMask defaultMask;
    public LayerMask goggleMask;

    public static event Action<Vector3> OnGoggleNoise; 

  
    private Color defaultAmbient;
    private bool isGoggleOn = false;

    void Start()
    {
        defaultAmbient = RenderSettings.ambientLight;
        mainCam.cullingMask = defaultMask;
        RenderSettings.fogDensity = blindFog;
        
        if (sonarVolume != null) sonarVolume.weight = 0;
        if (sonarLight != null) sonarLight.enabled = false;
    }

    void Update()
    {
        //detect if space bar is held
        if (Input.GetKeyDown(KeyCode.Space)) SetGoggles(true);
        if (Input.GetKeyUp(KeyCode.Space))   SetGoggles(false);

        //reveal location if goggle is on
        
        if (isGoggleOn)
        {
            OnGoggleNoise?.Invoke(transform.position);
        }
    }

    void SetGoggles(bool isActive)
    {
        isGoggleOn = isActive;

        if (isActive)
        {
            if (sonarVolume != null) sonarVolume.weight = 1; 
            RenderSettings.fogDensity = sonarFog;
            if (sonarLight != null) sonarLight.enabled = true;
            RenderSettings.ambientLight = sonarAmbientColor;
            mainCam.cullingMask = goggleMask;
        }
        else
        {
            if (sonarVolume != null) sonarVolume.weight = 0;
            RenderSettings.fogDensity = blindFog;
            if (sonarLight != null) sonarLight.enabled = false;
            RenderSettings.ambientLight = defaultAmbient;
            mainCam.cullingMask = defaultMask;
        }
    }
}