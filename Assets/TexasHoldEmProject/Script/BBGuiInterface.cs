using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BLabTexasHoldEmProject {

public class BBGuiInterface : MonoBehaviour {

  public GameObject[] m_PlayersDataList;
  public List<Image> PlayerActiveImageList = new List<Image>();
  public GameObject[] SpotlightOnPlayersList;

  public Image ImageGameGeneralPhase;
  public Text TextGamePhase;
  public Text TextGamePhaseInfo;
  public Text TextGamePOT;
  public Text TextGameLastBetvalue;
  public Text TextGameSTACK;

  BBGameController _BBGameController; 

  public Button[] BettingChipsButtonsList;
    private Button buttonGameCALL;
	private Button buttonGameFOLD;
	private Button buttonGameRAISE;
	private Button buttonGameCHECK;
	private Button buttonGameALLIN;


	public Transform GameActionButtonsRoot;
	public GameObject ImageCardsContainerOnButtons;

	private Button buttonBet_5;
	private Button buttonBet_25;
	private Button buttonBet_50;
	private Button buttonBet_100;

	[HideInInspector]
	public Text TextGameRAISECounter;
	[HideInInspector]
	public Text TextGameRAISEValue;

   void Awake() {

		_BBGameController = GetComponent<BBGameController>();

		ImageGameGeneralPhase = GameObject.Find("ImageGameGeneralPhase").GetComponent<Image>();

		  if(_BBGameController._BBGlobalDefinitions.isAnOpenGame) ImageGameGeneralPhase.color = Color.green;
		else ImageGameGeneralPhase.color = Color.red;

		GameObject BettingChipsButtons = GameObject.Find("BettingChipsButtons");
		BettingChipsButtonsList = BettingChipsButtons.GetComponentsInChildren<Button>();
		setBettingChipButtons(false,false,false,false);
		SpotlightOnPlayersList = new GameObject[8];
	

   }

	// Use this for initialization
	void Start () {

		buttonGameCALL = GameObject.Find("buttonGameCALL").GetComponent<Button>();
		buttonGameCALL.interactable = false;
		buttonGameFOLD = GameObject.Find("buttonGameFOLD").GetComponent<Button>();
		buttonGameFOLD.interactable = false;
		buttonGameRAISE = GameObject.Find("buttonGameRAISE").GetComponent<Button>();
		buttonGameRAISE.interactable = false;
		//buttonGameCHECK = GameObject.Find("buttonGameCHECK").GetComponent<Button>();
		//buttonGameCHECK.interactable = false;

		GameObject buttAllIn = GameObject.Find("buttonGameALLIN");
		if(buttAllIn != null) {
			buttonGameALLIN = GameObject.Find("buttonGameALLIN").GetComponent<Button>();
		    buttonGameALLIN.interactable = false;
		}

		buttonBet_5 = GameObject.Find("ImageCHIP_5").GetComponent<Button>();
		buttonBet_25 = GameObject.Find("ImageCHIP_25").GetComponent<Button>();
		buttonBet_50 = GameObject.Find("ImageCHIP_50").GetComponent<Button>();
		buttonBet_100 = GameObject.Find("ImageCHIP_100").GetComponent<Button>();

		TextGameSTACK = GameObject.Find("TextGameSTACK").GetComponent<Text>();
		TextGameSTACK.text = "Stack : " + BBStaticData.getMoneyValue( BBStaticData.gameLimitedStackValue );

		TextGamePhase = GameObject.Find("TextGamePhase").GetComponent<Text>();
		TextGamePhase.text = "Phase : " + "Wait...";

		TextGamePhaseInfo = GameObject.Find("TextGamePhaseInfo").GetComponent<Text>();
		TextGamePhaseInfo.text = BBStaticData.phaseMessageInfoWaitGameStarting;

		TextGameRAISECounter = GameObject.Find("TextGameRAISECounter").GetComponent<Text>();
		TextGameRAISEValue = GameObject.Find("TextGameRAISEValue").GetComponent<Text>();

		TextGamePOT = GameObject.Find("TextGamePOT").GetComponent<Text>();
		TextGameLastBetvalue = GameObject.Find("TextGameLastBetvalue").GetComponent<Text>();
		TextGameLastBetvalue.text = "";

		setGameButtons(false,false,false,false,false);
		setActionButtonPosition(false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setPlayersDataList(GameObject[] data) {
	   m_PlayersDataList = data;
	}
	public void setPlayersSpotLight(GameObject[] data) {
	  SpotlightOnPlayersList = data;
	}

	public void setPlayersData(List<BBPlayerData> pDataList) {

	   for(int x = 0;x < m_PlayersDataList.Length;x++) {
			Text TextPlayerName = m_PlayersDataList[x].transform.Find("TextPlayerName").GetComponent<Text>();
			pDataList[x].T_playerName = TextPlayerName;
			pDataList[x].T_playerName.text = _BBGameController._BBGlobalDefinitions.playersName[x]; //pDataList[x].playerName;

			Text TextPlayerBetType = m_PlayersDataList[x].transform.Find("TextPlayerBetType").GetComponent<Text>();
			pDataList[x].T_TextPlayerBetType = TextPlayerBetType;
			pDataList[x].T_TextPlayerBetType.text = "";

			Text TextPlayerMoneyOnTable = m_PlayersDataList[x].transform.Find("TextPlayerMoneyOnTable").GetComponent<Text>();
			pDataList[x].T_PlayerMoneyOnTable = TextPlayerMoneyOnTable;
			pDataList[x].T_PlayerMoneyOnTable.text = "";

			Text TextPlayerMoneyTotal = m_PlayersDataList[x].transform.Find("TextPlayerMoneyTotal").GetComponent<Text>();
			pDataList[x].T_PlayerMoneyTotal = TextPlayerMoneyTotal;

			switch(_BBGameController._BBGlobalDefinitions.gameType) {
			case BBGlobalDefinitions.GameType.Limited: 
			         pDataList[x].T_PlayerMoneyTotal.text = BBStaticData.getMoneyValue( _BBGameController._BBGlobalDefinitions.playersCashDuringOpenGame[x] );
				     pDataList[x].currentPlayerTotalMoney = _BBGameController._BBGlobalDefinitions.playersCashDuringOpenGame[x]; 
			    break;
			case BBGlobalDefinitions.GameType.NoLimit: 
			         pDataList[x].T_PlayerMoneyTotal.text = BBStaticData.getMoneyValue( _BBGameController._BBGlobalDefinitions.playersCashDuringOpenGame[x] );
				     pDataList[x].currentPlayerTotalMoney = _BBGameController._BBGlobalDefinitions.playersCashDuringOpenGame[x]; 
			    break;

			}

			Image PlayerActiveImage = m_PlayersDataList[x].transform.Find("PlayerActiveImage").GetComponent<Image>();
			pDataList[x].playerActiveImage = PlayerActiveImage;
			   if(_BBGameController._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[x]) {
				   pDataList[x].playerActiveImage.color = Color.blue;
			   } else {
			       pDataList[x].playerActiveImage.color = Color.white;
			   }

			PlayerActiveImageList.Add(pDataList[x].playerActiveImage);

			var sprite = Resources.Load<Sprite>("Avatar/playerAvatar_" + x.ToString());
			Image AvatarImage = m_PlayersDataList[x].transform.Find("AvatarImage").GetComponent<Image>();
			pDataList[x].playerAvatarImage = AvatarImage;
			pDataList[x].playerAvatarImage.overrideSprite = sprite;


			GameObject PlayerDealerImage = m_PlayersDataList[x].transform.Find("PlayerDealerImage").gameObject;
			pDataList[x].PlayerDealerImage = PlayerDealerImage;
			pDataList[x].PlayerDealerImage.SetActive(false);

			pDataList[x].playerPosition = x;

			if(!(_BBGameController._BBGlobalDefinitions.localPlayer == x)) {
				m_PlayersDataList[x].transform.Find("YouImage").gameObject.SetActive(false);
			}

			SpotlightOnPlayersList[x] = m_PlayersDataList[x].transform.Find("Spotlight").gameObject;
			SpotlightOnPlayersList[x].SetActive(false);

			Transform checkAllInImg = m_PlayersDataList[x].transform.Find("AllInImage");
			if(checkAllInImg != null)
				pDataList[x].AllInImage = checkAllInImg.gameObject.GetComponent<Image>();
	   }

	}


	public void activateDealer() {

		if( (PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color == Color.black) || 
			(PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color == Color.blue) ) {

		} else {

			BBStaticData.setAllPlayersStateImgRed(PlayerActiveImageList,"BBGuiInterface/activateDealer");

			PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color = Color.green;
       }

		foreach(GameObject i in SpotlightOnPlayersList) { i.SetActive(false);}
		if( PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color == Color.green ) {
			SpotlightOnPlayersList[_BBGameController._BBGlobalDefinitions.currentActivedealer].SetActive(true);
		}

	}

	public void setGreenOnPlayer(int playerID) {

		    BBStaticData.setAllPlayersStateImgRed(PlayerActiveImageList,"BBGuiInterface/setGreenOnPlayer PId : " + playerID);

		if( (PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color == Color.black) || 
			(PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color == Color.blue) ) {
		} else {
			PlayerActiveImageList[playerID].color = Color.green;
        }

		foreach(GameObject i in SpotlightOnPlayersList) { i.SetActive(false);}
		if( PlayerActiveImageList[playerID].color == Color.green ) {
			SpotlightOnPlayersList[playerID].SetActive(true);
		}


	}

	public void activatePlayer() {

		BBStaticData.setAllPlayersStateImgRed(PlayerActiveImageList,"BBGuiInterface/activatePlayer");

		if( (PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color == Color.black) || 
			(PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color == Color.blue) ) {
		} else {
		 PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivePlayer].color = Color.green;
		}

		foreach(GameObject i in SpotlightOnPlayersList) { i.SetActive(false);}
		if( PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivePlayer].color == Color.green ) {
			SpotlightOnPlayersList[_BBGameController._BBGlobalDefinitions.currentActivePlayer].SetActive(true);
		}

	}


	public void setPlayerMoneyValue(float val) {

		float onTableVal = _BBGameController.playerDataList[_BBGameController._BBGlobalDefinitions.currentActivePlayer].currentMoneyOnTable += val;
		float totalVal = _BBGameController.playerDataList[_BBGameController._BBGlobalDefinitions.currentActivePlayer].currentPlayerTotalMoney -= val;

		_BBGameController.playerDataList[_BBGameController._BBGlobalDefinitions.currentActivePlayer].T_PlayerMoneyOnTable.text = BBStaticData.getMoneyValue(onTableVal);
		_BBGameController.playerDataList[_BBGameController._BBGlobalDefinitions.currentActivePlayer].T_PlayerMoneyTotal.text = BBStaticData.getMoneyValue(totalVal);

//		Debug.Log("[BBGuiInterface][setPlayerMoneyValue] val : " + val + " onTableVal : " + onTableVal + " totalVal : " + totalVal);

		setPotValue(val);
	}


	public void setPlayerMoneyValue(float val, int playerID) {

		float onTableVal = _BBGameController.playerDataList[playerID].currentMoneyOnTable += val;
		float totalVal = _BBGameController.playerDataList[playerID].currentPlayerTotalMoney -= val;



		_BBGameController.playerDataList[playerID].T_PlayerMoneyOnTable.text = BBStaticData.getMoneyValue(onTableVal);
		_BBGameController.playerDataList[playerID].T_PlayerMoneyTotal.text = BBStaticData.getMoneyValue(totalVal);

//		Debug.Log("[BBGuiInterface][setPlayerMoneyValue] val : " + val + " onTableVal : " + onTableVal + " totalVal : " + totalVal + " playerID : " + playerID);

		setPotValue(val);

	}


	public void setDealer() {
		_BBGameController.playerDataList[_BBGameController._BBGlobalDefinitions.currentActivedealer].PlayerDealerImage.SetActive(true);
	}

	public void activatePlayerToTalk() {

		if(_BBGameController._BBGlobalDefinitions.playerToTalk == 10)  _BBGameController._BBGlobalDefinitions.playerToTalk = 0;

		Debug.Log("[BBGuiInterface][setPlayerMoneyValue] playerToTalk : " + (_BBGameController._BBGlobalDefinitions.playerToTalk) + " : " + _BBGameController._BBGlobalDefinitions.localPlayer);

		BBStaticData.setAllPlayersStateImgRed(PlayerActiveImageList,"BBGuiInterface/activatePlayerToTalk");

		if( (PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color == Color.black) || 
			(PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.currentActivedealer].color == Color.blue) ) {
		} else {
		  PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.playerToTalk].color = Color.green;
		}

		if((_BBGameController._BBGlobalDefinitions.playerToTalk) == _BBGameController._BBGlobalDefinitions.localPlayer) {
			setActionButtonPosition(true);
		} else {

		}

		foreach(GameObject i in SpotlightOnPlayersList) { i.SetActive(false);}
		if( PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.playerToTalk].color == Color.green ) {
			SpotlightOnPlayersList[_BBGameController._BBGlobalDefinitions.playerToTalk].SetActive(true);
		}


	}

	public void activatePlayerToTalk(int playerID) {

		GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().genBip);

		_BBGameController._BBGlobalDefinitions.playerToTalk = playerID;

//		Debug.Log("[BBGuiInterface][activatePlayerToTalk] playerToTalk : " + (_BBGameController._BBGlobalDefinitions.playerToTalk) + " : " + _BBGameController._BBGlobalDefinitions.localPlayer + " phase : " + _BBGameController._BBGlobalDefinitions.gamePhaseDetail);

		BBStaticData.setAllPlayersStateImgRed(PlayerActiveImageList,"BBGuiInterface/activatePlayerToTalk playerId : " + playerID);


		if( (_BBGameController.playerDataList[_BBGameController._BBGlobalDefinitions.playerToTalk].isOutOfGame == false) || (_BBGameController._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[_BBGameController._BBGlobalDefinitions.playerToTalk] == true) ) {
		} else {
			PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.playerToTalk].color = Color.green;
			Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>[BBGuiInterface][activatePlayerToTalk] playerToTalk  Color.green");
		}

		if((_BBGameController._BBGlobalDefinitions.playerToTalk) == _BBGameController._BBGlobalDefinitions.localPlayer) {
			setActionButtonPosition(true);
		   switch(_BBGameController._BBGlobalDefinitions.gamePhaseDetail) {
			case BBGlobalDefinitions.GamePhaseDetail.FirstBettingRound: setGameButtons(true,true,true,false,false); break;
			case BBGlobalDefinitions.GamePhaseDetail.ClosingPreFlop: setGameButtons(true,true,false,false,false); break;
			case BBGlobalDefinitions.GamePhaseDetail.Flop: setGameButtons(false,true,true,true,true); break;
			case BBGlobalDefinitions.GamePhaseDetail.ClosingTurn: setGameButtons(true,true,false,false,false); break;
			case BBGlobalDefinitions.GamePhaseDetail.River: setGameButtons(true,true,true,true,true); break;
			case BBGlobalDefinitions.GamePhaseDetail.ClosingRiver: setGameButtons(true,true,false,false,false); break;
		   }

		} else {
			setGameButtons(false,false,false,false,false);
			setActionButtonPosition(false);
		}

		//Debug.Log("[BBGuiInterface][activatePlayerToTalk] playerToTalk  3 ");


		foreach(GameObject i in SpotlightOnPlayersList) { i.SetActive(false);}
		if( PlayerActiveImageList[_BBGameController._BBGlobalDefinitions.playerToTalk].color == Color.green ) {
			SpotlightOnPlayersList[_BBGameController._BBGlobalDefinitions.playerToTalk].SetActive(true);
		}

		//Debug.Log("[BBGuiInterface][activatePlayerToTalk] playerToTalk  4 ");

	}


	public void setBettingChipButtons(bool _5,bool _25,bool _50,bool _100) {
		BettingChipsButtonsList[0].interactable = _5;
		BettingChipsButtonsList[1].interactable = _25;
		BettingChipsButtonsList[2].interactable = _50;
		BettingChipsButtonsList[3].interactable = _100;
	}

	public void setGameButtons(bool call,bool fold,bool raise,bool check, bool allin) {

/*		Debug.Log("XXXXXXXXXXXXXXXXXXXXXXXXX setGameButtons : " + 
			" gamePhaseDetail : " + _BBGameController._BBGlobalDefinitions.gamePhaseDetail +
			" activeTalker : " + _BBGameController._BBGlobalDefinitions.playerToTalk + 
			" call : " + call +
			" fold : " + fold +
			" raise : " + raise +
			" check : " + check +
			" allIn : " + allin
		 );
*/
	    //buttonGameCHECK.interactable = check; buttonGameCHECK.GetComponent<Image>().enabled = check; buttonGameCHECK.GetComponentInChildren<Text>().enabled = check;
		buttonGameCALL.interactable = call; buttonGameCALL.GetComponent<Image>().enabled = call; buttonGameCALL.GetComponentInChildren<Text>().enabled = call;
		buttonGameFOLD.interactable = fold; buttonGameFOLD.GetComponent<Image>().enabled = fold; buttonGameFOLD.GetComponentInChildren<Text>().enabled = fold;
		buttonGameRAISE.interactable = raise; buttonGameRAISE.GetComponent<Image>().enabled = raise; buttonGameRAISE.GetComponentInChildren<Text>().enabled = raise;

		if(_BBGameController._BBGlobalDefinitions.gameType == BBGlobalDefinitions.GameType.Limited) {
		    allin = false;
		   // buttonGameALLIN.interactable = allin; buttonGameALLIN.GetComponent<Image>().enabled = allin; buttonGameALLIN.GetComponentInChildren<Text>().enabled = allin;
		} else {
			buttonGameALLIN.interactable = allin; buttonGameALLIN.GetComponent<Image>().enabled = allin; buttonGameALLIN.GetComponentInChildren<Text>().enabled = allin;
		}

	}

	public void simulateOnClickButton(string buttName) {

//			Debug.Log("[TOSAVEONFILE] simulateOnClickButton buttName : " + buttName + " currentActivePlayer : " + _BBGameController._BBGlobalDefinitions.currentActivePlayer);

		var pointer = new PointerEventData(EventSystem.current);

	  switch(buttName) {

	  case "FOLD":
			ExecuteEvents.Execute(buttonGameFOLD.gameObject, pointer, ExecuteEvents.pointerClickHandler);
	  break;
	  case "CALL":
			ExecuteEvents.Execute(buttonGameCALL.gameObject, pointer, ExecuteEvents.pointerClickHandler);
	  break;
	  case "RAISE":
			ExecuteEvents.Execute(buttonGameRAISE.gameObject, pointer, ExecuteEvents.pointerClickHandler);
	  break;
	  case "BET_5":
			ExecuteEvents.Execute(buttonBet_5.gameObject, pointer, ExecuteEvents.pointerClickHandler);
	  break;
	  case "BET_25":
			ExecuteEvents.Execute(buttonBet_25.gameObject, pointer, ExecuteEvents.pointerClickHandler);
	  break;
	  case "BET_50":
			ExecuteEvents.Execute(buttonBet_50.gameObject, pointer, ExecuteEvents.pointerClickHandler);
	  break;
	  case "BET_100":
			ExecuteEvents.Execute(buttonBet_100.gameObject, pointer, ExecuteEvents.pointerClickHandler);
	  break;

	  }

	}

	public void setPlayerOUTGame(int playerID) {
	   PlayerActiveImageList[playerID].color = Color.black;
	}

	public void setPlayerBetType(int playerID, string type) {
		PlayerActiveImageList[playerID].color = Color.white;
	 	_BBGameController.playerDataList[playerID].T_TextPlayerBetType.text = type;
	}

	public void setLastBetValue(float val) {
		TextGameLastBetvalue.text = "Last Bet Val : " + BBStaticData.getMoneyValue(val);
	}

	public void setPotValue(float val) {

	   _BBGameController._BBGlobalDefinitions.moneyOnTable += val;

		TextGamePOT.text = "Pot : " + BBStaticData.getMoneyValue(_BBGameController._BBGlobalDefinitions.moneyOnTable);
	}

	public void setActionButtonPosition(bool wantShow) {
		Image[] imList = GameActionButtonsRoot.GetComponentsInChildren<Image>(true); foreach(Image i in imList) i.enabled = wantShow;
		Text[] txList = GameActionButtonsRoot.GetComponentsInChildren<Text>(true); foreach(Text t in txList) t.enabled = wantShow;
		ImageCardsContainerOnButtons.SetActive(wantShow);

		if(wantShow) {
		  Vector2 playerCardVal_1 = _BBGameController.playerDataList[0].card_1_Value;
		  Vector2 playerCardVal_2 = _BBGameController.playerDataList[0].card_2_Value;
		  Texture2D card1 = getCardImage(playerCardVal_1);
		  Texture2D card2 = getCardImage(playerCardVal_2);
		  GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_1").GetComponent<RawImage>().texture = card1;
		  GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_2").GetComponent<RawImage>().texture = card2;

				if(_BBGameController.flopCardList[0] == Vector2.zero) {
				  GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_FLOP_1").GetComponent<RawImage>().enabled = false;
				} else {
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_FLOP_1").GetComponent<RawImage>().texture = getCardImage(_BBGameController.flopCardList[0]);
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_FLOP_1").GetComponent<RawImage>().enabled = true;
				}
				if(_BBGameController.flopCardList[1] == Vector2.zero) {
				  GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_FLOP_2").GetComponent<RawImage>().enabled = false;
				} else {
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_FLOP_2").GetComponent<RawImage>().texture = getCardImage(_BBGameController.flopCardList[1]);
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_FLOP_2").GetComponent<RawImage>().enabled = true;
				}
				if(_BBGameController.flopCardList[2] == Vector2.zero) {
				  GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_FLOP_3").GetComponent<RawImage>().enabled = false;
				} else {
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_FLOP_3").GetComponent<RawImage>().texture = getCardImage(_BBGameController.flopCardList[2]);
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_FLOP_3").GetComponent<RawImage>().enabled = true;
				}
				if(_BBGameController.turnCard == Vector2.zero) {
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_TURN").GetComponent<RawImage>().enabled = false;
				} else {
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_TURN").GetComponent<RawImage>().texture = getCardImage(_BBGameController.turnCard);
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_TURN").GetComponent<RawImage>().enabled = true;
				}
				if(_BBGameController.riverCard == Vector2.zero) {
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_RIVER").GetComponent<RawImage>().enabled = false;
				} else {
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_RIVER").GetComponent<RawImage>().texture = getCardImage(_BBGameController.riverCard);
					GameActionButtonsRoot.Find("ImageCardsContainer/ImageCARD_RIVER").GetComponent<RawImage>().enabled = true;
				}

				switch(_BBGameController._BBGlobalDefinitions.gamePhaseDetail) {
				  case BBGlobalDefinitions.GamePhaseDetail.Flop:
					if(_BBGameController.flopCardList[0] != Vector2.zero) {
					    int maxCardValue = 0;
					    string cardsRes = GetComponent<BBPlayerCardsResultValueController>().getCardsResult(0,out maxCardValue).ToString();
					    GameActionButtonsRoot.Find("ImageCardsContainer/TextCardsResult").GetComponent<Text>().text = cardsRes;
					 }
				  break;
				  case BBGlobalDefinitions.GamePhaseDetail.Turn:
					if(_BBGameController.turnCard != Vector2.zero) {
					    int maxCardValue = 0;
					    string cardsRes = GetComponent<BBPlayerCardsResultValueController>().getCardsResult(0,out maxCardValue).ToString();
					    GameActionButtonsRoot.Find("ImageCardsContainer/TextCardsResult").GetComponent<Text>().text = cardsRes;
					 }
				  break;
				  case BBGlobalDefinitions.GamePhaseDetail.River:
					if(_BBGameController.riverCard != Vector2.zero) {
					    int maxCardValue = 0;
					    string cardsRes = GetComponent<BBPlayerCardsResultValueController>().getCardsResult(0,out maxCardValue).ToString();
					    GameActionButtonsRoot.Find("ImageCardsContainer/TextCardsResult").GetComponent<Text>().text = cardsRes;
					 }
				  break;
				}

		}

	}

	Texture2D getCardImage(Vector2 cardVal) {
	  Texture2D tmpret = null;

	   switch((int)cardVal.x) {
	    case 1:
	     switch((int)cardVal.y) {
		        case 1: tmpret = (Texture2D)Resources.Load("1_c"); break;
				case 2: tmpret = (Texture2D)Resources.Load("2_c"); break;
				case 3: tmpret = (Texture2D)Resources.Load("3_c"); break;
				case 4: tmpret = (Texture2D)Resources.Load("4_c"); break;
				case 5: tmpret = (Texture2D)Resources.Load("5_c"); break;
				case 6: tmpret = (Texture2D)Resources.Load("6_c"); break;
				case 7: tmpret = (Texture2D)Resources.Load("7_c"); break;
				case 8: tmpret = (Texture2D)Resources.Load("8_c"); break;
				case 9: tmpret = (Texture2D)Resources.Load("9_c"); break;
				case 10: tmpret = (Texture2D)Resources.Load("10_c"); break;
				case 11: tmpret = (Texture2D)Resources.Load("11_c"); break;
				case 12: tmpret = (Texture2D)Resources.Load("12_c"); break;
				case 13: tmpret = (Texture2D)Resources.Load("13_c"); break;
	     }
	    break;
		case 2:
	     switch((int)cardVal.y) {
		        case 1: tmpret = (Texture2D)Resources.Load("1_f"); break;
				case 2: tmpret = (Texture2D)Resources.Load("2_f"); break;
				case 3: tmpret = (Texture2D)Resources.Load("3_f"); break;
				case 4: tmpret = (Texture2D)Resources.Load("4_f"); break;
				case 5: tmpret = (Texture2D)Resources.Load("5_f"); break;
				case 6: tmpret = (Texture2D)Resources.Load("6_f"); break;
				case 7: tmpret = (Texture2D)Resources.Load("7_f"); break;
				case 8: tmpret = (Texture2D)Resources.Load("8_f"); break;
				case 9: tmpret = (Texture2D)Resources.Load("9_f"); break;
				case 10: tmpret = (Texture2D)Resources.Load("10_f"); break;
				case 11: tmpret = (Texture2D)Resources.Load("11_f"); break;
				case 12: tmpret = (Texture2D)Resources.Load("12_f"); break;
				case 13: tmpret = (Texture2D)Resources.Load("13_f"); break;
	     }
	    break;
		case 3:
	     switch((int)cardVal.y) {
		        case 1: tmpret = (Texture2D)Resources.Load("1_p"); break;
				case 2: tmpret = (Texture2D)Resources.Load("2_p"); break;
				case 3: tmpret = (Texture2D)Resources.Load("3_p"); break;
				case 4: tmpret = (Texture2D)Resources.Load("4_p"); break;
				case 5: tmpret = (Texture2D)Resources.Load("5_p"); break;
				case 6: tmpret = (Texture2D)Resources.Load("6_p"); break;
				case 7: tmpret = (Texture2D)Resources.Load("7_p"); break;
				case 8: tmpret = (Texture2D)Resources.Load("8_p"); break;
				case 9: tmpret = (Texture2D)Resources.Load("9_p"); break;
				case 10: tmpret = (Texture2D)Resources.Load("10_p"); break;
				case 11: tmpret = (Texture2D)Resources.Load("11_p"); break;
				case 12: tmpret = (Texture2D)Resources.Load("12_p"); break;
				case 13: tmpret = (Texture2D)Resources.Load("13_p"); break;
	     }
	    break;
		case 4:
	     switch((int)cardVal.y) {
		        case 1: tmpret = (Texture2D)Resources.Load("1_q"); break;
				case 2: tmpret = (Texture2D)Resources.Load("2_q"); break;
				case 3: tmpret = (Texture2D)Resources.Load("3_q"); break;
				case 4: tmpret = (Texture2D)Resources.Load("4_q"); break;
				case 5: tmpret = (Texture2D)Resources.Load("5_q"); break;
				case 6: tmpret = (Texture2D)Resources.Load("6_q"); break;
				case 7: tmpret = (Texture2D)Resources.Load("7_q"); break;
				case 8: tmpret = (Texture2D)Resources.Load("8_q"); break;
				case 9: tmpret = (Texture2D)Resources.Load("9_q"); break;
				case 10: tmpret = (Texture2D)Resources.Load("10_q"); break;
				case 11: tmpret = (Texture2D)Resources.Load("11_q"); break;
				case 12: tmpret = (Texture2D)Resources.Load("12_q"); break;
				case 13: tmpret = (Texture2D)Resources.Load("13_q"); break;
	     }
	    break;

	   }


	  return tmpret;
	}

}
}