using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/**
 *       TITLE: MainSceneController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-06-05
 * DESCRIPTION: 메인 씬의 UI와 씬간 연결을 진행하는 스크립트
 *     DEV LOG: 
 */

public class MainSceneController : MonoBehaviour
{
    [Header("UI Display List")]
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject uiTeamLogo;
    [SerializeField] float fTeamLogoPlayTime = 3.0f;
    [SerializeField] GameObject uiPressAnyKey;
    [SerializeField] GameObject uiMissionSelect;

    [Header("UI Display: Mission Detail Panel")]
    [SerializeField] GameObject uiMissionDetail;
    [SerializeField] Text txtTitle;
    [SerializeField] Text txtGameModeType;
    [SerializeField] Text txtDescription;
    [SerializeField] Image imgDescription;
    [SerializeField] Button btnAccept;
    [SerializeField] Button btnDecline;

    [Header("Scene Icon")]
    [SerializeField] GameObject iconModeSelector;

    //GameModeSceneInfo[] gmsiList;

    enum UIStatus { None = 0, TeamLogo = 1, PressAnyKey = 2, MissionSelect = 3 }
    UIStatus uiStatus = UIStatus.None;

    Vector3 v3LeftBottomCorner = new Vector3(-73.0f, -37.0f, 0.0f),
            v3RightTopCorner   = new Vector3( 73.0f,  37.0f, 0.0f);

    List<GameObject> objContractIcon = new List<GameObject>();
    bool bIsDisplayingContract = false;
    bool bIsDisplayingDetail = false;
    int nNumLastClickedButton = -1;

	void Start ()
    {
        if (PlayerPrefs.HasKey("UIStatus"))
        {
            uiStatus = (UIStatus)PlayerPrefs.GetInt("UIStatus");
            PlayerPrefs.DeleteKey("UIStatus");
        }

        switch (uiStatus)
        {
            // 처음 켤 경우 팀 로고를 보여준다
            case UIStatus.None:
                uiStatus = UIStatus.TeamLogo;
                PlayerPrefs.SetInt("UIStatus", (int)UIStatus.MissionSelect);
                StartCoroutine(PlayTeamLogo());
                break;

            // 씬을 임무 선택 화면으로 시작할 경우
            case UIStatus.MissionSelect:
                uiStatus = UIStatus.MissionSelect;
                break;
        }

        //gmsiList = GetComponentsInChildren<GameModeSceneInfo>();
        //for (int i = 0; i < gmsiList.Length; i++) gmsiList[i].nNumber = i;
	}
	
	void Update ()
    {
        switch (uiStatus)
        {
            case UIStatus.PressAnyKey:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    uiPressAnyKey.SetActive(false);
                    uiMissionSelect.SetActive(true);

                    uiStatus = UIStatus.MissionSelect;
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                break;

            case UIStatus.MissionSelect:
                uiMissionSelect.SetActive(true);

                //if (objContractIcon.Count < gmsiList.Length && !bIsDisplayingContract && !bIsDisplayingDetail)
                //{
                //    StartCoroutine(DisplayContractIcon());
                //}
                break;
        }
    }

    /// <summary>
    /// 시작할 때 로고를 <code>fTeamLogoPlayTime</code>만큼 재생하고 다음 씬으로 넘어갑니다. 
    /// </summary>
    IEnumerator PlayTeamLogo()
    {
        float fPlayTime = Time.time;

        uiTeamLogo.SetActive(true);

        while (Time.time < fPlayTime + fTeamLogoPlayTime)
        {
            yield return new WaitForEndOfFrame();
        }

        uiTeamLogo.SetActive(false);
        uiPressAnyKey.SetActive(true);

        uiStatus = UIStatus.PressAnyKey;
    }

    /// <summary>
    /// 임무(계약) 선택 아이콘을 1.5초 간격으로 출력합니다. 
    /// </summary>
    //IEnumerator DisplayContractIcon()
    //{
    //    bIsDisplayingContract = true;
    //    for (int i = 0; i < gmsiList.Length; i++)
    //    {
    //        if (bIsDisplayingDetail)
    //            StopCoroutine(DisplayContractIcon());
    //        if (!gmsiList[i].bDisplayed)
    //        {
    //            yield return new WaitForSeconds(1.5f);

    //            gmsiList[i].bDisplayed = true;

    //            gmsiList[i].v3Position.x = Random.Range(v3LeftBottomCorner.x, v3RightTopCorner.x);
    //            gmsiList[i].v3Position.y = Random.Range(v3LeftBottomCorner.y, v3RightTopCorner.y);
    //            gmsiList[i].v3Position.z = canvas.transform.position.z;

    //            GameObject objIcon = Instantiate(iconModeSelector, gmsiList[i].v3Position, Quaternion.identity, uiMissionSelect.transform);
    //            objIcon.GetComponent<GameModeSceneInfo>().InitInfo(
    //                gmsiList[i].strSceneName, gmsiList[i].strGameModeType, gmsiList[i].strGameModeDescription, gmsiList[i].spriteDescription, gmsiList[i].scene
    //            );
    //            objIcon.GetComponent<GameModeSceneInfo>().nNumber = gmsiList[i].nNumber;
    //            objIcon.transform.SetSiblingIndex(0);
    //            objIcon.transform.localPosition = gmsiList[i].v3Position;
    //            objIcon.GetComponent<Image>().sprite = gmsiList[i].spriteIcon;
    //            objIcon.GetComponent<Button>().onClick.AddListener(OnContractIconClicked);

    //            objContractIcon.Add(objIcon);
    //        }
    //    }

    //    bIsDisplayingContract = false;
    //}

    /// <summary>
    /// 임무(계약) 선택 아이콘을 클릭했을 때 임무의 세부사항을 출력합니다. 
    /// </summary>
    //void OnContractIconClicked()
    //{
    //    bIsDisplayingDetail = true;
    //    uiMissionDetail.SetActive(true);

    //    GameObject obj = EventSystem.current.currentSelectedGameObject;
    //    nNumLastClickedButton = obj.GetComponent<GameModeSceneInfo>().nNumber;
    //    gmsiList[nNumLastClickedButton].bDisplayed = false;
    //    objContractIcon.Remove(obj);
    //    Destroy(obj);

    //    txtTitle.text = gmsiList[nNumLastClickedButton].strSceneName;
    //    txtGameModeType.text = gmsiList[nNumLastClickedButton].strGameModeType;
    //    txtDescription.text = gmsiList[nNumLastClickedButton].strGameModeDescription;
    //    imgDescription.sprite = gmsiList[nNumLastClickedButton].spriteDescription;

    //    btnAccept.onClick.AddListener(OnButtonAcceptClicked);
    //    btnDecline.onClick.AddListener(OnButtonDeclineClicked);
    //}

    /// <summary>
    /// 계약을 수락하였을 경우: 해당 임무에 투입됩니다. 
    /// </summary>
    //void OnButtonAcceptClicked()
    //{
    //    SceneManager.LoadScene(gmsiList[nNumLastClickedButton].scene.name);
    //}

    /// <summary>
    /// 계약을 거절하였을 경우: 임무 세부창이 꺼지고 다른 임무를 선택하도록 합니다. 
    /// </summary>
    void OnButtonDeclineClicked()
    {
        uiMissionDetail.SetActive(false);
        bIsDisplayingDetail = false;
    }
}
