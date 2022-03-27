using UnityEngine;
using UnityEngine.Audio;

public class SettingsController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public bool isFullscreen;
    public float volume;

    public Settings savedSettings;

    public Color enabledColor = new Color(0.5960785f, 0.3490196f, 0.1803922f, 1f);
    public Color disabledColor = new Color(0.8018868f, 0.6193426f, 0.494749f, 0.75f);

    public void SetVolume(float vol) // 0 - 8
    {
        switch ((int)vol)
        {
            case 8:
                volume = 0;
                break;
            case 7:
                volume = -4;
                break;
            case 6:
                volume = -10;
                break;
            case 5:
                volume = -15;
                break;
            case 4:
                volume = -20;
                break;
            case 3:
                volume = -30;
                break;
            case 2:
                volume = -40;
                break;
            case 1:
                volume = -50;
                break;
            case 0:
                volume = -80;
                break;
        }

        audioMixer.SetFloat("Volume", volume);
        Save();
    }

    public void SetScreenMode()
    {
        Screen.fullScreen = isFullscreen;
        Save();
    }

    public void LoadData()
    {
        if (SaveData.current.savedSettings == null)
        {
            volume = 0f;
            isFullscreen = true;
        }
        else
        {
            volume = SaveData.current.savedSettings.volume;
            isFullscreen = SaveData.current.savedSettings.isFullscreen;
        }

        SetScreenMode();
        audioMixer.SetFloat("Volume", volume);
    }

    public void Save()
    {
        savedSettings.volume = volume;
        savedSettings.isFullscreen = isFullscreen;
        SaveData.current.savedSettings = savedSettings;
    }
}

[System.Serializable]
public class Settings
{
    public float volume;
    public bool isFullscreen;
}