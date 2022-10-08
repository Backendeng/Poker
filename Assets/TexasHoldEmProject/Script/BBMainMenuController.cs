using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BLabTexasHoldEmProject {

public class BBMainMenuController : MonoBehaviour {

	public BBCreateGameData _BBCreateGameData; 
	public BBGlobalDefinitions _BBGlobalDefinitions;

	public GameObject mainPanel;
	public GameObject optionsPanel;
	public GameObject rulesPanel;
	public GameObject PanelgetNickname;
	public GameObject PanelNotEnoughMoney;

	public GameObject ScrollViewLimited;
	public GameObject ScrollViewNOLimited;

	private BLab.GetCoinsSystem.InAppConfigScriptableAsset iap;

#if USE_GAME_CENTER
	void ProcessAuthentication (bool success) {
		if (success) {
			Debug.Log ("Authenticated, SUCCESS");
		}
		else
			Debug.Log ("Failed to authenticate");
	}
#endif

	IEnumerator Start () {

#if !USE_UNITY_IAP
			GameObject buttBuyCoins = GameObject.Find("ButtonBuyCoins");
			if(buttBuyCoins) {
			//   Destroy(buttBuyCoins);
			}

	   if(PlayerPrefs.HasKey("MPGeneralPlayerMoney")) {
	   } else {
			PlayerPrefs.SetFloat("MPGeneralPlayerMoney",BBStaticVariable.baseStartingMoneyForPlayer);
	   }

#else
	   iap = Resources.Load("InAppConfigScriptableAsset") as BLab.GetCoinsSystem.InAppConfigScriptableAsset;

	   if(PlayerPrefs.HasKey("MPGeneralPlayerMoney")) {
	   } else {
			PlayerPrefs.SetFloat("MPGeneralPlayerMoney",iap.gameMoneysettings.playerInitialMoney);
	   }
#endif

#if !USE_PHOTON
			GameObject buttMultiplayer = GameObject.Find("ButtonGoMultiplayer");
			if(buttMultiplayer) {
				Destroy(buttMultiplayer);
			}
#endif


#if USE_GAME_CENTER
   gameObject.AddComponent<BBGameCenterController>();
   GetComponent<BBGameCenterController>().initGC();
#endif

#if UNITY_EDITOR
    gameObject.AddComponent<BBGetScreenShoot>();
#endif

		Screen.sleepTimeout = SleepTimeout.NeverSleep;

	  if(!PlayerPrefs.HasKey("PlayerNickName")) {
	     mainPanel.SetActive(false);
	     PanelgetNickname.SetActive(true);
	  }

       yield return new WaitForSeconds(2);

		if(!_BBCreateGameData.isDataComplete) {
			GameObject.Find("ToggleUseSavedData").GetComponent<Toggle>().enabled = false;
		}
		_BBGlobalDefinitions.useSimulatedSavedData = false;
		_BBGlobalDefinitions.isAnOpenGame = false;
		_BBGlobalDefinitions.useLastCardsHandSavedData = false;
		BBStaticData.cardsHandProgressive = 1;

	}
	
	void gotGenericButtonClick(GameObject _go) {
       switch(_go.name) {
		case "ButtonPlayLimited":
		   if(checkCanPlayWithMyMoney(true)) {
		         _BBGlobalDefinitions.gameType = BBGlobalDefinitions.GameType.Limited;
				 SceneManager.LoadScene("LimitScene");
		   }
		break;
		case "ButtonPlayNoLimit":
			if(checkCanPlayWithMyMoney(true)) {
			  _BBGlobalDefinitions.gameType = BBGlobalDefinitions.GameType.NoLimit;
			  SceneManager.LoadScene("NoLimitScene");
			}
		break;
		case "ButtonCreateGame":
			SceneManager.LoadScene("CreateGameScene");
		break;
		case "ButtonSettings":
			mainPanel.SetActive(false);
			optionsPanel.SetActive(true);
		break;
		case "ButtonCloseOptionPanel":
			mainPanel.SetActive(true);
			optionsPanel.SetActive(false);
		break;
		case "ButtonRules":
			mainPanel.SetActive(false);
			rulesPanel.SetActive(true);
		break;
		case "ButtonCloseRulesPanel":
			mainPanel.SetActive(true);
			rulesPanel.SetActive(false);
		break;
		case "ButtonTryCraps":
		break;
		case "ButtonTryHorseRace":
		break;
		case "ButtonSaveNickName":
			saveNickName();
		break;
		case "ButtonTXHoldLimit":
		    ScrollViewLimited.SetActive(true);
		    ScrollViewNOLimited.SetActive(false);
		break;
		case "ButtonTXHoldNOLimit":
			ScrollViewLimited.SetActive(false);
		    ScrollViewNOLimited.SetActive(true);
		break;
		case "ButtonGameCenter":
#if USE_GAME_CENTER
		  GetComponent<BBGameCenterController>().showGCLeaderBoard();
#endif
		break;
		case "ButtonGoMultiplayer":
				if(checkCanPlayWithMyMoney(false)) {
		           SceneManager.LoadScene("demoMultiplayerMainMenu");
		        }
		break;
		case "ButtonBuyCoins":
		  SceneManager.LoadScene("MoneyControlScene");
		break;
		case "ButtonCloseNotEnoughMoney":
		 PanelNotEnoughMoney.SetActive(false);
		break;
       }
   }

   void saveNickName() {
		string nick = PanelgetNickname.transform.Find("InputFieldGetNickName").GetComponent<InputField>().text;
		if(nick.Length > 1) {
			PlayerPrefs.SetString("PlayerNickName",nick);
		} else {
			PlayerPrefs.SetString("PlayerNickName","Anonymous");
		}

		_BBGlobalDefinitions.playersName[0] = PlayerPrefs.GetString("PlayerNickName");
		PanelgetNickname.SetActive(false);
		mainPanel.SetActive(true);

   }

	public void ToggleUseSavedDataOnChange(Toggle t) {
	  _BBGlobalDefinitions.useSimulatedSavedData = t.isOn;
	}
	public void ToggleUseLastCardsHandSavedData(Toggle t) {
	  _BBGlobalDefinitions.useLastCardsHandSavedData = t.isOn;
	}

	bool checkCanPlayWithMyMoney(bool singlePlayerGame) {
	  bool tmpRet = false;
	  float currentMoney = PlayerPrefs.GetFloat("MPGeneralPlayerMoney");

	    if(singlePlayerGame) {
				if(currentMoney < _BBGlobalDefinitions.stackMoneyAtStart) {
				    PanelNotEnoughMoney.SetActive(true);
					PanelNotEnoughMoney.transform.Find("TextMinMoneyValue").GetComponent<Text>().text = BBStaticData.getMoneyValue(_BBGlobalDefinitions.stackMoneyAtStart);
					PanelNotEnoughMoney.transform.Find("TextPlayerMoneyValue").GetComponent<Text>().text = BBStaticData.getMoneyValue(currentMoney);
				} else {
				  tmpRet = true;
				}
				Debug.Log("[checkCanPlayWithMyMoney] currentMoney : " + currentMoney + 
			          " single stackMoneyAtStart : " + _BBGlobalDefinitions.stackMoneyAtStart);

	    } else {
				if(currentMoney < BBStaticVariable.gameLimitedStackValue) {
				    PanelNotEnoughMoney.SetActive(true);
					PanelNotEnoughMoney.transform.Find("TextMinMoneyValue").GetComponent<Text>().text = BBStaticData.getMoneyValue(BBStaticVariable.gameLimitedStackValue);
					PanelNotEnoughMoney.transform.Find("TextPlayerMoneyValue").GetComponent<Text>().text = BBStaticData.getMoneyValue(currentMoney);

				} else {
				  tmpRet = true;
				}
				Debug.Log("[checkCanPlayWithMyMoney] currentMoney : " + currentMoney + 
					" multi BBStaticVariable.gameLimitedStackValue : " + BBStaticVariable.gameLimitedStackValue);

	    }


	  return tmpRet;

	}

}
}