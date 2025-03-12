using UnityEngine;

public class AdaptiveResolution : MonoBehaviour
{
    [Range(0.5f, 1.0f)]
    public float minResolutionScale = 0.7f; 
    public float maxResolutionScale = 1.0f; 

    public int targetFrameRate = 30;

    void Start()
    {
        Application.targetFrameRate = targetFrameRate; 
    }

    void Update()
    {
        if (Time.deltaTime > 1.0f / targetFrameRate)
        {
            float currentScale = ScalableBufferManager.widthScaleFactor;
            float newScale = Mathf.Clamp(currentScale - 0.05f, minResolutionScale, maxResolutionScale);
            ScalableBufferManager.ResizeBuffers(newScale, newScale);
        }
        else if (Time.deltaTime < 1.0f / targetFrameRate)
        {
            float currentScale = ScalableBufferManager.widthScaleFactor;
            float newScale = Mathf.Clamp(currentScale + 0.05f, minResolutionScale, maxResolutionScale);
            ScalableBufferManager.ResizeBuffers(newScale, newScale);
        }
    }
}
