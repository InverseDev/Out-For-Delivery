using UnityEngine;
using UnityEngine.UI;
using CandyCoded.HapticFeedback;

public class QualitySettingsManager : MonoBehaviour
{
    public Button lowQualityButton;
    public Button mediumQualityButton;
    public Button highQualityButton;

    void Start()
    {
        lowQualityButton.onClick.AddListener(SetLowQuality);
        mediumQualityButton.onClick.AddListener(SetMediumQuality);
        highQualityButton.onClick.AddListener(SetHighQuality);
    }

    public void SetLowQuality()
    {
        QualitySettings.SetQualityLevel(0);
        HapticFeedback.MediumFeedback();
    }

    public void SetMediumQuality()
    {
        QualitySettings.SetQualityLevel(2);
        HapticFeedback.MediumFeedback();
    }

    public void SetHighQuality()
    {
        QualitySettings.SetQualityLevel(5);
        HapticFeedback.MediumFeedback();
    }
}
