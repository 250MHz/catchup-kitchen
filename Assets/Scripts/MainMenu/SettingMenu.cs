using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import textMeshPro dropdown

public class SettingMenu : MonoBehaviour
{
    Resolution[] resolutions; // Resolution Array
    public TMP_Dropdown resolutionDropdown;

    void Start()
    {
        resolutions = Screen.resolutions; // Check avaliable resolutions of the screen

        resolutionDropdown.ClearOptions(); // Clear options in the dropdown menu

        List<string> options = new List<string> ();

        // Add avaliable resolutions to options list
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add (option);
        }

        resolutionDropdown.AddOptions(options);
    }

    public void SetVolume (float volume)
    {
        Debug.Log(volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
