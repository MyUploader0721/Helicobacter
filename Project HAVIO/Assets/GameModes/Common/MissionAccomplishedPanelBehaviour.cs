using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MissionAccomplishedPanelBehaviour : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button btnRestart;
    [SerializeField] Button btnQuit;
    [Space]
    [Header("Fader")]
    [SerializeField] SceneFadingController sceneFadingController;
    
	void Start ()
    {
        btnRestart.onClick.AddListener(OnButtonRestartClicked);
        btnQuit.onClick.AddListener(OnButtonQuitClicked);
    }

	void Update ()
    {
		
	}

    void OnButtonRestartClicked()
    {
        sceneFadingController.FadeOut(true);
    }

    void OnButtonQuitClicked()
    {
        sceneFadingController.FadeOut(false);
    }
}
