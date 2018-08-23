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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnButtonQuitClicked()
    {
        SceneManager.LoadScene("Lobby");
    }
}
