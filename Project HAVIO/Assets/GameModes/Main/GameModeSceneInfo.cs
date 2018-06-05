using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 *       TITLE: GameModeSceneInfo.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-06-05
 * DESCRIPTION: 게임모드를 실행할 때 사용하는 스크립트입니다. 
 *     DEV LOG: 사실상 게임모드의 정보가 들어있는 스크립트입니다. 
 *              버튼을 클릭하면 게임모드를 시작할 수 있습니다. 
 */

public class GameModeSceneInfo : MonoBehaviour
{
    public string strSceneName;
    public string strGameModeType;
    public string strGameModeDescription;
    public Object scene;
    public Vector3 v3Position = Vector3.zero;
    public Texture textureIcon;
    public bool bDisplayed = false;

	void Start ()
    {
		
	}
}
