using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/**
 *       TITLE: IntroSceneController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-06-19
 * DESCRIPTION: 게임의 인트로 씬을 컨트롤하는 스크립트입니다. 
 *     DEV LOG: 씬의 전환에 페이드인/아웃을 넣어 부드러운 화면 전환을 지원합니다. 
 *              일정 시간이 지나면 다음 씬으로 넘어갈 수 있도록 합니다. 3
 *  2018-07-04: GUI 디자인에 따라 로비의 종료 화면을 추가합니다. 
 */

public class LobbySceneController : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] AudioClip sfxIntro;
    AudioSource bgmPlayer;

    [Header("UI SFX")]
    [SerializeField] AudioClip sfxHover;
    [SerializeField] AudioClip sfxClick;
    [SerializeField] AudioClip sfxAccept;
    [SerializeField] AudioClip sfxDecline;
    [SerializeField] AudioClip sfxIconPop;
    AudioSource sfxPlayer;

    [Header("For UI System: Contract Info Panel")]
    [SerializeField] GameObject objContractInfoPanel;
    [SerializeField] GameObject txtContractTitle;
    [SerializeField] GameObject txtContractDescription;
    [SerializeField] GameObject txtContractConditions;
    [SerializeField] GameObject txtContractorName;
    [SerializeField] GameObject txtGameModeType;
    [SerializeField] Image imgContractImage;
    [SerializeField] Button btnAcceptContract;
    [SerializeField] Button btnDeclineContract;

    [Header("For UI System: Game Exit Panel")]
    [SerializeField] GameObject objGameExitPanel;
    [SerializeField] Button btnExitConfirm;
    [SerializeField] Button btnExitCancel;

    [Header("Setting for Displaying Contracts")]
    [SerializeField] GameObject objCanvasBoard;
    [SerializeField] GameObject objContractIcon;
    [SerializeField][Range(1.0f, 4.0f)] float fContractDisplayTime = 2.0f;

    [Header("Setting for Fading While Scene Transition")]
    [SerializeField] Image imgPanelFading;
    [SerializeField][Range(1.0f, 2.5f)] float fFadeInTime = 1.5f;
    [SerializeField][Range(1.0f, 2.5f)] float fFadeOutTime = 1.5f;

    List<ContractInfo> listContracts = new List<ContractInfo>();

    bool bFadingDone = false;
    bool bDisplayingContracts = false;
    int nClickedContractNum = -1;
    bool bReadyToContract = false;
    bool bReadyToExit = false;

    void Start ()
    {
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.clip = sfxIntro;
        bgmPlayer.volume = 0.0f;
        bgmPlayer.Play();

        sfxPlayer = gameObject.AddComponent<AudioSource>();

        StartCoroutine(FadeIn());

        ContractInfo[] contractInfos = GetComponentsInChildren<ContractInfo>();
        foreach(ContractInfo cInfo in contractInfos)
            listContracts.Add(cInfo);

        btnAcceptContract.onClick.AddListener(OnButtonAcceptContractClicked);
        btnDeclineContract.onClick.AddListener(OnButtonDeclineContractClicked);

        btnExitConfirm.onClick.AddListener(OnButtonExitConfirmClicked);
        btnExitCancel.onClick.AddListener(OnButtonExitCancelClicked);
    }
	
	void Update ()
    {
		if (!bDisplayingContracts && !objContractInfoPanel.activeInHierarchy)
            StartCoroutine(ShowContracts());

        if (bFadingDone)
        {
            if (bReadyToContract)
            {
                SceneManager.LoadScene("Desert01");

                // 임시방편 - 동적 씬 로딩이 안됨
                switch (listContracts[nClickedContractNum].sceneToContinue.name)
                {
                    case "Desert01":
                        SceneManager.LoadScene("Desert01");
                        break;
                    default:
                        SceneManager.LoadScene("Desert01");
                        break;
                }
            }
            else if (bReadyToExit)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
		        Application.Quit();
#endif
            }
        }

        if (!bgmPlayer.isPlaying)
            bgmPlayer.Play();

		if (Input.GetButtonDown("FaceButtonB") || Input.GetKeyDown(KeyCode.Escape))
		{
            if (objContractInfoPanel.activeInHierarchy)
            {
                sfxPlayer.PlayOneShot(sfxDecline);
                objContractInfoPanel.SetActive(false);
            }
            else
            {
                // 종료창을 열음
                if (!objGameExitPanel.activeInHierarchy)
                {
                    objGameExitPanel.SetActive(true);
                }
                // 종료창을 닫음
                else
                {
                    objGameExitPanel.SetActive(false);
                }
            }
		}
    }

    /// <summary>
    /// 페이드인, 화면을 서서히 밝혀줍니다. 
    /// </summary>
    IEnumerator FadeIn()
    {
        bFadingDone = false;
        while (imgPanelFading.color.a > 0.0f)
        {
            imgPanelFading.color = new Color(0.0f, 0.0f, 0.0f, imgPanelFading.color.a - (fFadeInTime / 100.0f));
            bgmPlayer.volume = 1.0f - imgPanelFading.color.a;
            yield return new WaitForSeconds(fFadeInTime / 100.0f);
        }
        bFadingDone = true;
    }

    /// <summary>
    /// 페이드아웃, 화면을 서서히 어둡게 합니다. 
    /// </summary>
    IEnumerator FadeOut()
    {
        bFadingDone = false;
        while (imgPanelFading.color.a < 1.0f)
        {
            imgPanelFading.color = new Color(0.0f, 0.0f, 0.0f, imgPanelFading.color.a + (fFadeOutTime / 100.0f));
            bgmPlayer.volume = 1.0f - imgPanelFading.color.a;
            yield return new WaitForSeconds(fFadeOutTime / 100.0f);
        }
        bFadingDone = true;
    }

    IEnumerator ShowContracts()
    {
        bDisplayingContracts = true;

        for (int i = 0; i < listContracts.Count; i++)
        {
            if (objContractInfoPanel.activeInHierarchy)
                StopCoroutine(ShowContracts());

            if (listContracts[i].bDisplayed) continue;
            else
            {
                yield return new WaitForSeconds(fContractDisplayTime + Random.Range(-1.0f, 1.0f));

                listContracts[i].strContractorName = "0x" + GetRandomHex(8);
                // 아이콘의 위치 결정
                Vector3 v3ContractIconPosition = Vector3.zero;
                v3ContractIconPosition.x = Random.Range(-29.0f, 29.0f);
                v3ContractIconPosition.y = Random.Range(-28.0f, 30.0f);

                sfxPlayer.PlayOneShot(sfxIconPop);
                GameObject objContract = Instantiate(
                    objContractIcon,
                    Vector3.zero, objCanvasBoard.transform.rotation,
                    objCanvasBoard.transform
                );
                objContract.transform.localPosition = v3ContractIconPosition;
                objContract.transform.SetSiblingIndex(0);
                objContract.transform.name = objContract.transform.GetHashCode().ToString("X");

                objContract.GetComponent<ContractInfo>().SetData(
                    listContracts[i].strContractTitle,      listContracts[i].strGameModeType,
                    listContracts[i].spriteGameModeIcon,    listContracts[i].strContractDescription,
                    listContracts[i].strContractConditions, listContracts[i].spriteContractImage,
                    listContracts[i].sceneToContinue,       listContracts[i].strContractorName
                );

                int nCurrValue = i;
                objContract.GetComponent<Button>().onClick.AddListener(delegate { OnContractIconClicked(nCurrValue); });
                objContract.GetComponent<Button>().image.sprite = listContracts[i].spriteGameModeIcon;

                listContracts[i].strBtnHashData = objContract.transform.name;
                listContracts[i].bDisplayed = true;
            }
        }

        bDisplayingContracts = false;
    }

    /// <summary>
    /// <code>nCount</code>자리의 더미 16진수 문자열을 생성합니다. 
    /// </summary>
    /// <param name="nCount">문자열 자리수</param>
    /// <returns>16진수 더미 문자열</returns>
    string GetRandomHex(int nCount)
    {
        string strResult = "";

        for (int i = 0; i < nCount; i++)
            strResult += Random.Range(0, 16).ToString("X");

        return strResult;
    }

    /// <summary>
    /// 계약 아이콘을 클릭했을 때 정보가 UI에 출력되도록 합니다. 
    /// </summary>
    /// <param name="nNum">클릭한 계약의 번호</param>
    void OnContractIconClicked(int nNum)
    {
        sfxPlayer.PlayOneShot(sfxClick);

        nClickedContractNum = nNum;
        objContractInfoPanel.SetActive(true);
        
        txtContractTitle.GetComponent<TextMeshProUGUI>().text = listContracts[nNum].strContractTitle;
        txtContractDescription.GetComponent<TextMeshProUGUI>().text = listContracts[nNum].strContractDescription;
        txtContractConditions.GetComponent<TextMeshProUGUI>().text = listContracts[nNum].strContractConditions;
        txtContractorName.GetComponent<TextMeshProUGUI>().text = "Contractor: " + listContracts[nNum].strContractorName;
        txtGameModeType.GetComponent<TextMeshProUGUI>().text = "Game Mode: " + listContracts[nNum].strGameModeType;
        imgContractImage.sprite = listContracts[nNum].spriteContractImage;

        Destroy(GameObject.Find(listContracts[nNum].strBtnHashData));
        listContracts[nNum].bDisplayed = false;
    }

    /// <summary>
    /// 계약 수락 버튼을 클릭했을 때 페이드아웃 후 해당 임무로 이동하도록 합니다. 
    /// </summary>
    void OnButtonAcceptContractClicked()
    {
        sfxPlayer.PlayOneShot(sfxAccept);

        bReadyToContract = true;
        StartCoroutine(FadeOut());
    }
    /// <summary>
    /// 계약 거절 버튼을 클릭했을 때 계약 선택 화면으로 돌아가도록 합니다. 
    /// </summary>
    void OnButtonDeclineContractClicked()
    {
        sfxPlayer.PlayOneShot(sfxDecline);

        objContractInfoPanel.SetActive(false);
    }

    /// <summary>
    /// 게임 종료창의 네 버튼을 클릭할 경우 게임이 페이드아웃 된 후 종료되도록 합니다. 
    /// </summary>
    void OnButtonExitConfirmClicked()
    {
        bReadyToExit = true;
        StartCoroutine(FadeOut());
    }
    /// <summary>
    /// 게임 종료창의 아니오 버튼을 클릭할 경우 게임으로 돌아가도록 합니다. 
    /// </summary>
    void OnButtonExitCancelClicked()
    {
        objGameExitPanel.SetActive(false);
    }
}
