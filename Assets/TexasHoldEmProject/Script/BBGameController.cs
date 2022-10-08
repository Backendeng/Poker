using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BLabTexasHoldEmProject {

public class BBGameController : MonoBehaviour {

  public GameObject UIMoveingController;

  [HideInInspector]
  public bool allInInAction = false;
  [HideInInspector]
  public float allInInActionValue = 0;
  [HideInInspector]
  public int allInInActionForPlayer = 0;
  [HideInInspector]
  public float allInInActionExtraPotValue = 0;


   public GameObject PanelLocalPlayerLose;

   public bool simulateAllCardsUseInspectorData;
   public bool simulateAllCardsUseSavedData;

   public bool giveAllCardsOpen = false;

    public GameObject[] playersCardPositions;
	public Transform playersCardPositionsRoot;

  
    public List<BBPlayerData> playerDataList = new List<BBPlayerData>();   

	public BBGlobalDefinitions _BBGlobalDefinitions; 
	public BBLastCardsHand _BBLastCardsHand; 


	BBGuiInterface _BBGuiInterface;

	Quaternion rotatedCardRotation;
	Quaternion rotatedCardRotation180;
	[HideInInspector]
	public Transform cardsDiscardPosition;
	[HideInInspector]
	public Transform[] shuffledCardsList; 
	[HideInInspector]
	public int lastGivenCardIdx = 0;
	private Vector2 currentGivenCard;

	//[HideInInspector]
	public Vector2[] flopCardList = new Vector2[3];
	//[HideInInspector]
	public Vector2 turnCard;
	//[HideInInspector]
	public Vector2 riverCard;

	[HideInInspector]
	public bool waitingForManualButtonResponce = false;



	    public int timesToShuffle;
	    public List<Vector2> GlobalDeck = new List<Vector2>();
		private List<Vector2> mainDeck = new List<Vector2>();
		//private bool ableToShuffle = false;
		


	void Awake() {
//=========================================================

#if UNITY_EDITOR
    gameObject.AddComponent<BBGetScreenShoot>();
#endif

#if USE_GAME_CENTER
  gameObject.AddComponent<BBGameCenterController>();
#endif

        UIMoveingController.SetActive(false);

		simulateAllCardsUseSavedData = _BBGlobalDefinitions.useSimulatedSavedData;

	     Screen.sleepTimeout = SleepTimeout.NeverSleep;

		_BBGlobalDefinitions = Resources.Load("BBGlobalDefinitions") as BBGlobalDefinitions;
		_BBGuiInterface = gameObject.GetComponent<BBGuiInterface>();
		_BBGlobalDefinitions.setInitialData();
	}


	public bool checkIfLocalplayerWonBeforeShowDown() {
		int outCounter = 0;for(int x = 0; x < _BBGlobalDefinitions.playersStateIsOutDuringOpenGame.Length;x++) {if(_BBGlobalDefinitions.localPlayer != x) {if(_BBGlobalDefinitions.playersStateIsOutDuringOpenGame[x] == true) outCounter++;}}
             if(outCounter > 8) {
				executeLocalPlayerWon(playerDataList[_BBGlobalDefinitions.localPlayer].currentPlayerTotalMoney + _BBGlobalDefinitions.moneyOnTable,true);
				return true;
               }
             int tmpOut = 0;
		     for(int x = 0; x < playerDataList.Count;x++) {
		        if(playerDataList[x].isOutOfGame) tmpOut++;
		     }
		     if(tmpOut > 8) {
			  executeLocalPlayerWon(playerDataList[_BBGlobalDefinitions.localPlayer].currentPlayerTotalMoney + _BBGlobalDefinitions.moneyOnTable,true);
			  return true;
		     }
		     return false;
	}

	public void executeLocalPlayerLose() {

	    UIMoveingController.SetActive(true);
		PanelLocalPlayerLose.SetActive(true);
		Transform bng =  UIMoveingController.transform.Find("ButtonNewGame");
        if(bng != null)	bng.gameObject.SetActive(false);

	}

	public void executeLocalPlayerWon(float localPlayerWonValue, bool wonCoseAlone) {
		UIMoveingController.SetActive(true);
		GetComponent<BBShowDownResultController>().PanelLocalPlayerWon.SetActive(true);
		GetComponent<BBShowDownResultController>().PanelLocalPlayerWon.transform.Find("TextWinnerMoneyWon").GetComponent<Text>().text = BBStaticData.getMoneyValue(localPlayerWonValue);
	    GameObject bng = GameObject.Find("ButtonNewGame");
	    if(bng != null) bng.SetActive(false);

#if USE_GAME_CENTER
		GetComponent<BBGameCenterController>().ReportScore((long)localPlayerWonValue,"grp.maxHandvalue");
#endif

	}

	// Use this for initialization
	IEnumerator Start () {

		GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().cardsShuffle);

		rotatedCardRotation = GameObject.Find("RotatedCardOnTable").transform.rotation;
		rotatedCardRotation180 = GameObject.Find("RotatedCardOnTable180").transform.rotation;
		cardsDiscardPosition = GameObject.Find("RotatedCardOnTable").transform;

	   _BBGlobalDefinitions.playersOnTable = _BBGuiInterface.m_PlayersDataList.Length;

	   for(int x = 0;x < _BBGlobalDefinitions.playersOnTable;x++) {
	      BBPlayerData pd = new BBPlayerData();
	      pd.playerName = "player_" + (x).ToString();
	      pd.playerPosition = x+1;
	      playerDataList.Add(pd);
	   }

	   yield return new WaitForSeconds(2);

	   _BBGuiInterface.setPlayersData(playerDataList);

	   StartCoroutine(startGame());
	}
	
	// Update is called once per frame
	void Update () {

#if UNITY_ANDROID
		if(   Input.GetKeyDown(KeyCode.Escape) ) {
			SceneManager.LoadScene("MainMenu");
		}
#endif
		 	
	}

	bool checkForPlayersMoneyBeforeStart() {

		for(int x = 0;x < _BBGlobalDefinitions.playersStateIsOutDuringOpenGame.Length;x++) {
			if(_BBGlobalDefinitions.playersCashDuringOpenGame[x] < _BBGlobalDefinitions.limitedHight * BBStaticData.cardsHandProgressive) {
				_BBGlobalDefinitions.playersStateIsOutDuringOpenGame[x] = true;
				if(x == _BBGlobalDefinitions.localPlayer) {
				 return false;
				}
			}
		}

		return true;

	}


	bool setGameBeforeStart() {

		int tmpSmallBPlayerID = _BBGlobalDefinitions.smallBlindPlayerId; 
		bool gotSmallBlindPlayer = false;
		bool gotBigBlindPlayer = false;


		for(int x = 0; x < playerDataList.Count;x++) {
			if( _BBGlobalDefinitions.playersStateIsOutDuringOpenGame[tmpSmallBPlayerID] == false ) {
			            gotSmallBlindPlayer = true;
			       break;
			} else {
			     if(tmpSmallBPlayerID == 9) { 
			        tmpSmallBPlayerID = 0;
			     } else {
			       tmpSmallBPlayerID++;
			     }
				if( _BBGlobalDefinitions.playersStateIsOutDuringOpenGame[tmpSmallBPlayerID] == false ) {
				      gotSmallBlindPlayer = true;
				      break;   
				}
			}
		}

		if(!gotSmallBlindPlayer) return false;

		int startFindBigBlind = tmpSmallBPlayerID + 1;
		if(startFindBigBlind == 10) startFindBigBlind = 0;

		Debug.Log("firstLastPlayerToTalkOnTable ---------------> startFindBigBlind : " + startFindBigBlind + " tmpSmallBPlayerID : " + tmpSmallBPlayerID);  

		for(int x = 0; x < playerDataList.Count;x++) {

			Debug.Log("firstLastPlayerToTalkOnTable ---------------> startFindBigBlind : " + startFindBigBlind + " : " + _BBGlobalDefinitions.playersStateIsOutDuringOpenGame[startFindBigBlind]);  


			if( _BBGlobalDefinitions.playersStateIsOutDuringOpenGame[startFindBigBlind] == false ) {
				   gotBigBlindPlayer = true;
			       break;
			} else {
				if(startFindBigBlind == 9) { 
					startFindBigBlind = 0;
			     } else {
					startFindBigBlind++;
			     }
				if( _BBGlobalDefinitions.playersStateIsOutDuringOpenGame[startFindBigBlind] == false ) {
				      gotBigBlindPlayer = true;
				      break;   
				}
            }		     

        }


    int first = _BBGlobalDefinitions.currentActivedealer;
    if(first == 9) first = 0;
    else first++;

		_BBGlobalDefinitions.firstLastPlayerToTalkOnTable = new Vector2((float)first,_BBGlobalDefinitions.currentActivedealer);

		_BBGlobalDefinitions.smallBlindPlayerId = tmpSmallBPlayerID; 
		_BBGlobalDefinitions.bigBlindPlayerId = startFindBigBlind;


    if(gotBigBlindPlayer) return true;

	 return false;

	}


	IEnumerator startGame() {

	   yield return new WaitForSeconds(5);

		 if(!checkForPlayersMoneyBeforeStart()) {
		    executeLocalPlayerLose();
		    yield break;
		 }

	    if( !setGameBeforeStart() ) {
	    //TODO show message not players to play
			Debug.LogError(" setGameBeforeStart XXXXXXXXXXXXXXXXXXXXXX firstLastPlayerToTalkOnTable ---------show message not players to play------> : ");
          yield break;
	    }

	   _BBGuiInterface.ImageGameGeneralPhase.color = Color.green;
	   _BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.SmallBlind;
	   _BBGuiInterface.TextGamePhase.text = "Phase : " + _BBGlobalDefinitions.gamePhaseDetail.ToString();
	   _BBGlobalDefinitions.currentActivePlayer = _BBGlobalDefinitions.smallBlindPlayerId;

		_BBGuiInterface.TextGamePhaseInfo.text = playerDataList[_BBGlobalDefinitions.currentActivePlayer].playerName + " : " + BBStaticData.phaseMessageInfoMakeYourSmallBlindBet; 

		_BBGuiInterface.setBettingChipButtons(false,false,false,false);
	   
	   _BBGuiInterface.activatePlayer();
	   _BBGuiInterface.setDealer();

		yield return new WaitForSeconds(2);

	   GetComponent<BBPreFlopPhaseAIController>().executeSmallBlindBet();

		yield return new WaitForSeconds(2);

		GetComponent<BBPreFlopPhaseAIController>().executeBigBlindBet();

	}

	IEnumerator ie_gotGameActionButton(GameObject _go) {

		switch(_go.name) {
			    case "buttonGameCALL": 
				  switch(_BBGlobalDefinitions.gamePhaseDetail) {
				     case BBGlobalDefinitions.GamePhaseDetail.FirstBettingRound: 
					   yield return StartCoroutine( GetComponent<BBPreFlopPhaseAIController>().executeCall(_BBGlobalDefinitions.playerToTalk) );
	                 break;
				     case BBGlobalDefinitions.GamePhaseDetail.ClosingPreFlop: 
				       yield return StartCoroutine( GetComponent<BBPreFlopPhaseAIController>().executeCall(_BBGlobalDefinitions.playerToTalk) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.Flop:
				         yield return StartCoroutine( GetComponent<BBFlopPhaseAIController>().executeFlopCall(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingFlop:
				        yield return StartCoroutine( GetComponent<BBFlopPhaseAIController>().executeFlopCall(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.Turn:
				        yield return StartCoroutine( GetComponent<BBTurnPhaseAIController>().executeTurnCall(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingTurn:
				        yield return StartCoroutine( GetComponent<BBTurnPhaseAIController>().executeTurnCall(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.River:
				        yield return StartCoroutine( GetComponent<BBRiverPhaseAIController>().executeRiverCall(_BBGlobalDefinitions.localPlayer) );
			         break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingRiver:
				        yield return StartCoroutine( GetComponent<BBRiverPhaseAIController>().executeRiverCall(_BBGlobalDefinitions.localPlayer) );
	                 break;

				  }
			break;
		    case "buttonGameFOLD":
				switch(_BBGlobalDefinitions.gamePhaseDetail) {
				     case BBGlobalDefinitions.GamePhaseDetail.FirstBettingRound: 
				         yield return StartCoroutine( GetComponent<BBPreFlopPhaseAIController>().executeFold(_BBGlobalDefinitions.localPlayer) );
	                 break;
				     case BBGlobalDefinitions.GamePhaseDetail.ClosingPreFlop: 
				         yield return StartCoroutine( GetComponent<BBPreFlopPhaseAIController>().executeFold(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.Flop:
				         yield return StartCoroutine( GetComponent<BBFlopPhaseAIController>().executeFlopFold(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingFlop:
				         yield return StartCoroutine( GetComponent<BBFlopPhaseAIController>().executeFlopFold(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.Turn:
				        yield return StartCoroutine( GetComponent<BBTurnPhaseAIController>().executeTurnFold(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingTurn:
				        yield return StartCoroutine( GetComponent<BBTurnPhaseAIController>().executeTurnFold(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.River:
				        yield return StartCoroutine( GetComponent<BBRiverPhaseAIController>().executeRiverFold(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingRiver:
				        yield return StartCoroutine( GetComponent<BBRiverPhaseAIController>().executeRiverFold(_BBGlobalDefinitions.localPlayer) );
	                 break;

				  }
			break;
		    case "buttonGameRAISE":
			       switch(_BBGlobalDefinitions.gamePhaseDetail) {
				     case BBGlobalDefinitions.GamePhaseDetail.FirstBettingRound: 
				           yield return StartCoroutine( GetComponent<BBPreFlopPhaseAIController>().executeRaise(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingPreFlop: 
				           yield return StartCoroutine( GetComponent<BBPreFlopPhaseAIController>().executeRaise(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.Flop: 
				       yield return StartCoroutine( GetComponent<BBFlopPhaseAIController>().executeFlopRaise(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingFlop:
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.Turn:
				       yield return StartCoroutine( GetComponent<BBTurnPhaseAIController>().executeTurnRaise(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingTurn:
				       yield return StartCoroutine( GetComponent<BBTurnPhaseAIController>().executeTurnRaise(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.River:
				       yield return StartCoroutine( GetComponent<BBRiverPhaseAIController>().executeRiverRaise(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.ClosingRiver:
				     yield return StartCoroutine( GetComponent<BBRiverPhaseAIController>().executeRiverRaise(_BBGlobalDefinitions.localPlayer) );
	                 break;
				   }
			break;
		    case "buttonGameCHECK":
			       switch(_BBGlobalDefinitions.gamePhaseDetail) {
			         case BBGlobalDefinitions.GamePhaseDetail.Flop:
				      // yield return StartCoroutine( GetComponent<BBFlopPhaseAIController>().executeFlopCheck(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.Turn:
				        yield return StartCoroutine( GetComponent<BBTurnPhaseAIController>().executeTurnCheck(_BBGlobalDefinitions.localPlayer) );
	                 break;
			         case BBGlobalDefinitions.GamePhaseDetail.River:
				        yield return StartCoroutine( GetComponent<BBRiverPhaseAIController>().executeRiverCheck(_BBGlobalDefinitions.localPlayer) );
	                 break;
				  }

			break;
		    case "buttonGameALLIN":
			        switch(_BBGlobalDefinitions.gamePhaseDetail) {
		               case BBGlobalDefinitions.GamePhaseDetail.Flop:
			           case BBGlobalDefinitions.GamePhaseDetail.Turn:
			           case BBGlobalDefinitions.GamePhaseDetail.River:
				          yield return StartCoroutine( GetComponent<BBAllInController>().executeAllIN(_BBGlobalDefinitions.localPlayer) );
		               break;
		            }
		    break;
		 }

		waitingForManualButtonResponce = false;

        yield return new WaitForEndOfFrame();

	}

	public IEnumerator startExecuteShowDownPhase() {
	   
		Debug.Log("*******************************[BBGameController][startExecute*** SHOW DOWN ***Phase]**************************");
		_BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.ShowDown;
		_BBGuiInterface.TextGamePhase.text = "Phase : " + _BBGlobalDefinitions.gamePhaseDetail.ToString();
		_BBGuiInterface.TextGamePhase.color = Color.red;
		_BBGuiInterface.TextGamePhaseInfo.text = "Starting ShowDown...";
		_BBGuiInterface.TextGamePhaseInfo.color = Color.red;

		GetComponent<BBShowDownResultController>().executeFinalShowDown();

		 yield break;
	}

	public IEnumerator startExecuteRiverPhase() {
		Debug.Log("*******************************[BBGameController][startExecute*** RIVER ***Phase]**************************");

		_BBGuiInterface.activateDealer();
		_BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.River;
		_BBGuiInterface.TextGamePhase.text = "Phase : " + _BBGlobalDefinitions.gamePhaseDetail.ToString();
		_BBGuiInterface.TextGamePhaseInfo.text = "Starting River...";

		yield return new WaitForSeconds(1);

		yield return StartCoroutine( giveOneCard(_BBGlobalDefinitions.currentActivedealer,cardsDiscardPosition,true) );

		yield return new WaitForSeconds(1);

		yield return StartCoroutine( giveOneCard(_BBGlobalDefinitions.currentActivedealer,GameObject.Find("openCard_RIVER").transform,false) );
		riverCard = currentGivenCard;

		StartCoroutine( GetComponent<BBPlayerAIController>().executeRiverResult() );
	 yield break;
	}

	public IEnumerator startExecuteTurnPhase() {

		Debug.Log("*******************************[BBGameController][startExecute***TURN***Phase]**************************");

		_BBGuiInterface.activateDealer();
		_BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.Turn;
		_BBGuiInterface.TextGamePhase.text = "Phase : " + _BBGlobalDefinitions.gamePhaseDetail.ToString();
		_BBGuiInterface.TextGamePhaseInfo.text = "Starting Turn...";

		yield return new WaitForSeconds(1);

		yield return StartCoroutine( giveOneCard(_BBGlobalDefinitions.currentActivedealer,cardsDiscardPosition,true) );

		yield return new WaitForSeconds(1);

		yield return StartCoroutine( giveOneCard(_BBGlobalDefinitions.currentActivedealer,GameObject.Find("openCard_TURN").transform,false) );
		turnCard = currentGivenCard;

		StartCoroutine( GetComponent<BBPlayerAIController>().executeTurnResult() );
	}

	public IEnumerator startExecuteFlopPhase() {
		Debug.Log("*******************************[BBGameController][startExecuteFlopPhase]**************************");
		_BBGuiInterface.activateDealer();
		_BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.Flop;
		_BBGuiInterface.TextGamePhase.text = "Phase : " + _BBGlobalDefinitions.gamePhaseDetail.ToString();
		_BBGuiInterface.TextGamePhaseInfo.text = "Starting FLOP...";

		yield return new WaitForSeconds(1);
		yield return StartCoroutine( giveOneCard(_BBGlobalDefinitions.currentActivedealer,cardsDiscardPosition,true) );

		yield return new WaitForSeconds(1);
		yield return StartCoroutine( giveOneCard(_BBGlobalDefinitions.currentActivedealer,GameObject.Find("openCard_1").transform,false) );
		flopCardList[0] = currentGivenCard;
		
		yield return new WaitForSeconds(1);
		yield return StartCoroutine( giveOneCard(_BBGlobalDefinitions.currentActivedealer,GameObject.Find("openCard_2").transform,false) );
		flopCardList[1] = currentGivenCard;

		yield return new WaitForSeconds(1);
		yield return StartCoroutine( giveOneCard(_BBGlobalDefinitions.currentActivedealer,GameObject.Find("openCard_3").transform,false) );
		flopCardList[2] = currentGivenCard;
		Debug.Log("*******************************[BBGameController][startExecuteFlopPhase]************************** cards : " + 
			flopCardList[0] + " : " + flopCardList[1] + " : " + flopCardList[2]);

        StartCoroutine( GetComponent<BBPlayerAIController>().executeFlopResult() );
       
	}

   void setNewGameData() {

         BBStaticData.cardsHandProgressive++;

           float[] currentPlayerscash = new float[playerDataList.Count];
           for(int x = 0;x < playerDataList.Count;x++) {currentPlayerscash[x] = playerDataList[x].currentPlayerTotalMoney;}
           _BBGlobalDefinitions.savePlayersCashDuringGame(currentPlayerscash);


		   if( BBStaticData.currentActivePlayer == 9 ) BBStaticData.currentActivePlayer = 0; 
		    else BBStaticData.currentActivePlayer++;

		   if( BBStaticData.currentActivedealer == 9 ) BBStaticData.currentActivedealer = 0; 
		    else BBStaticData.currentActivedealer++;

		switch(BBStaticData.currentActivedealer) {
		case 0:
			BBStaticData.smallBlindPlayerId = 1;
		    BBStaticData.bigBlindPlayerId = 2;
         break;
		case 1:
			BBStaticData.smallBlindPlayerId = 2;
		    BBStaticData.bigBlindPlayerId = 3;
         break;
		case 2:
			BBStaticData.smallBlindPlayerId = 3;
		    BBStaticData.bigBlindPlayerId = 4;
         break;
		case 3:
			BBStaticData.smallBlindPlayerId = 4;
		    BBStaticData.bigBlindPlayerId = 5;
         break;
		case 4:
			BBStaticData.smallBlindPlayerId = 5;
		    BBStaticData.bigBlindPlayerId = 6;
         break;
		case 5:
			BBStaticData.smallBlindPlayerId = 6;
		    BBStaticData.bigBlindPlayerId = 7;
         break;
		case 6:
			BBStaticData.smallBlindPlayerId = 7;
		    BBStaticData.bigBlindPlayerId = 8;
         break;
		case 7:
			BBStaticData.smallBlindPlayerId = 8;
		    BBStaticData.bigBlindPlayerId = 9;
         break;
		case 8:
			BBStaticData.smallBlindPlayerId = 9;
		    BBStaticData.bigBlindPlayerId = 0;
         break;
		case 9:
			BBStaticData.smallBlindPlayerId = 0;
		    BBStaticData.bigBlindPlayerId = 1;
         break;
		}

   }


   void gotGenericButtonClick(GameObject _go) {
       switch(_go.name) {
		case "ButtonNewGame":
		  setNewGameData();
#if USE_UNITY_ADV
#if UNITY_ANDROID || UNITY_IOS
		BLab.GetCoinsSystem.UnityAdvertisingController.ShowAdPlacement( BLab.GetCoinsSystem.UnityAdvertisingController.zoneIdVideo);
#endif
#endif
		  SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		break;
		case "ButtonHidePanel":
		    GetComponent<BBShowDownResultController>().finalResultPanel.SetActive(false);
		break;
		case "ButtonShowPanel":
			GetComponent<BBShowDownResultController>().finalResultPanel.SetActive(true);
		break;
		case "ButtonMainMenu":
#if USE_UNITY_ADV
#if UNITY_ANDROID || UNITY_IOS
		BLab.GetCoinsSystem.UnityAdvertisingController.ShowAdPlacement( BLab.GetCoinsSystem.UnityAdvertisingController.zoneIdVideo);
#endif
#endif
			SceneManager.LoadScene("MainMenu");
		break;
		case "ButtonZoom":
			BBStaticData.executeCameraZoom();
		break;
		case "ButtonCloseOnYouWonPanel":
			GetComponent<BBShowDownResultController>().PanelLocalPlayerWon.SetActive(false);
			GetComponent<BBShowDownResultController>().finalResultPanel.SetActive(true);
		break;
		case "ButtonCloseYouLosePanel":
		    PanelLocalPlayerLose.SetActive(false);
			GetComponent<BBShowDownResultController>().finalResultPanel.SetActive(true);
			UIMoveingController.SetActive(true);
			Transform bng =  UIMoveingController.transform.Find("ButtonNewGame");
         		   if(bng != null)	bng.gameObject.SetActive(false);
		break;
		case "ButtonGetBestCards":
				List<BBAllInController.PlayersCValue> cardsV = GetComponent<BBAllInController>().getCardsResultDuringGameHand();
				foreach(BBAllInController.PlayersCValue cv in cardsV) {
					Debug.Log("Get Best Five List PlayersCValue -> : " + " pos : " + cv.playerPosID + " : " + cv.bestFive[0] + " : " + cv.bestFive[1] + " : " + cv.bestFive[2] + " : " + cv.bestFive[3] + " : " + cv.bestFive[4]);
				}
		break;
       }
   }

	void gotGameActionButton(GameObject _go) {
	    
		Debug.Log("[BBGameController][gotGameActionButton]------> : " + _go.name + "\n _BBGlobalDefinitions.playerToTalk : " + _BBGlobalDefinitions.playerToTalk + 
			"\n phase : " + _BBGlobalDefinitions.gamePhaseDetail + "\n waitingForManualButtonResponce : " + waitingForManualButtonResponce +
				"\n _go : " + _go.name);
		StartCoroutine( ie_gotGameActionButton(_go) );
		_BBGuiInterface.setGameButtons(false,false,false,false,false);

		if(_BBGlobalDefinitions.playerToTalk == _BBGlobalDefinitions.localPlayer) {
		 _BBGuiInterface.setActionButtonPosition(false);
		}

	}


	void gotBetButton(GameObject _go) {

	  float valToSet = 0;

		 switch(_go.name) {
			case "ImageCHIP_5": valToSet = 5; break;
			case "ImageCHIP_25": 
			    //valToSet = _BBGlobalDefinitions.smallBlindValue;
			      // valToSet = 25;
			      valToSet = BBStaticData.cardsHandProgressive * 25; 
			 break;
			case "ImageCHIP_50": 
			    //valToSet = _BBGlobalDefinitions.bigBlindValue;
			    //valToSet = 50; 
			    valToSet = BBStaticData.cardsHandProgressive * 50; 
			break;
			case "ImageCHIP_100": valToSet = 100; break;
		 }

		int currentActivePlayer = GetComponent<BBGameController>()._BBGlobalDefinitions.currentActivePlayer;
		_BBGuiInterface.setPlayerMoneyValue(valToSet);
		StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(valToSet,currentActivePlayer,GetComponent<BBMoveingObjectsController>().playersChipEndingPoint[currentActivePlayer]) );
	}

	public void setNextPlayerActive() {

	      if(_BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.SmallBlind) {
				_BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.BigBlind;
				_BBGuiInterface.TextGamePhase.text = "Phase : " + _BBGlobalDefinitions.gamePhaseDetail.ToString();

				if(_BBGlobalDefinitions.currentActivePlayer == 9) {
					_BBGlobalDefinitions.currentActivePlayer = 0;
				 } else { 
				    _BBGlobalDefinitions.currentActivePlayer++;
				 }

				_BBGuiInterface.TextGamePhaseInfo.text = playerDataList[_BBGlobalDefinitions.currentActivePlayer].playerName + " : " + BBStaticData.phaseMessageInfoMakeYourBigBlindBet; 
				_BBGuiInterface.activatePlayer();
				_BBGlobalDefinitions.playerToTalk = (_BBGlobalDefinitions.currentActivePlayer + 1);
	      }

	}

	public void dealerStartGiveCards() {
		_BBGuiInterface.activateDealer();
		_BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.GiveingCards;
		_BBGuiInterface.TextGamePhase.text = "Phase : " + _BBGlobalDefinitions.gamePhaseDetail.ToString();
		_BBGuiInterface.TextGamePhaseInfo.text = BBStaticData.phaseMessageInfoDealerGiveingCards; 
		InitializeDeck();
		StartCoroutine(startGiveCards());
	}

	IEnumerator giveOneCard(int startPosPlayerId, Transform destPos, bool covered) {

		currentGivenCard = GlobalDeck[lastGivenCardIdx];
		GameObject cartToGive = null;
		bool wantRotate = covered;
		cartToGive = getCard(GlobalDeck[lastGivenCardIdx],wantRotate,true);

		cartToGive.transform.position = GetComponent<BBMoveingObjectsController>().playersChipStartingPoint[startPosPlayerId].position;
		yield return new WaitForEndOfFrame();
		yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(cartToGive, destPos) );

		lastGivenCardIdx++;
	}

	IEnumerator startGiveCards() {

	   int startingPos = _BBGlobalDefinitions.currentActivedealer + 1;

		if(startingPos == 10) startingPos = 0;

		Debug.Log("[BBGameController][startGiveCards] startingPos : " + startingPos);

	    GameObject cartToGive = null;
		shuffledCardsList = getGiveCardsList(startingPos);

	    int tmpCounter = 0;
		int tmpProgressivePlayer = 0;
	   for(int x = 0;x < (playerDataList.Count * 2);x++) {
					
			         string[] data = shuffledCardsList[x].transform.name.Split(new char[] { '_' }); 
					 Debug.Log(shuffledCardsList[x].transform.name);
			         int posVal = int.Parse(data[0]);
			          bool wantRotate = (posVal != _BBGlobalDefinitions.localPlayer);
				      cartToGive = getCard(GlobalDeck[x],wantRotate);

				   tmpCounter++;
					Debug.Log(tmpCounter);
					Debug.Log(posVal);
					Debug.Log(x);
			       if(tmpCounter == 1) {
				      playerDataList[posVal].card_1_Value = GlobalDeck[x];
				      playerDataList[posVal].transform_card_1 = cartToGive.transform;
				      if(_BBGlobalDefinitions.playersStateIsOutDuringOpenGame[posVal]) {
					     Destroy( cartToGive );
				         goto Jump;
				      }
			       }
			       if(tmpCounter == 2) {
				      playerDataList[posVal].card_2_Value = GlobalDeck[x];
				      playerDataList[posVal].transform_card_2 = cartToGive.transform;
				      tmpCounter = 0;
				      tmpProgressivePlayer++;
				      if(_BBGlobalDefinitions.playersStateIsOutDuringOpenGame[posVal]) {
					     Destroy( cartToGive );
				         goto Jump;
				      }
			       }

				    cartToGive.transform.position = GetComponent<BBMoveingObjectsController>().playersChipStartingPoint[_BBGlobalDefinitions.currentActivedealer].position;
				    GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().pickCard);
				    yield return new WaitForEndOfFrame();
			        yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(cartToGive, shuffledCardsList[x].transform,posVal) );
Jump:
			    yield return new WaitForEndOfFrame();
			    lastGivenCardIdx = x;
	   }

       lastGivenCardIdx++;

		Debug.Log("[BBGameController][startGiveCards] giveing cards DONE");

		_BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.FirstBettingRound;

		executeFirstBettingRound();

	   yield break;

	}

   void executeFirstBettingRound() {

		_BBGuiInterface.TextGamePhase.text = "Phase : " + _BBGlobalDefinitions.gamePhaseDetail.ToString();
		_BBGuiInterface.TextGamePhaseInfo.text = BBStaticData.phaseMessageInfoFirstBettingRound; 

        _BBGuiInterface.activatePlayerToTalk();

		StartCoroutine( GetComponent<BBPreFlopPhaseAIController>().executePreFlopFirstBetRound(_BBGlobalDefinitions.playerToTalk) );

   }


		// Initializes the mainDeck
		void InitializeDeck(){
			mainDeck.Clear();
			GlobalDeck.Clear();
			for(int deckValue = 1; deckValue <= 13; deckValue++){
				for(int deckSuit = 1; deckSuit <= 4; deckSuit++){
					mainDeck.Add(new Vector2(deckSuit, deckValue));
				}
			}
			Debug.Log("Deck Initialized. Size: " + mainDeck.Count);
			//ableToShuffle = true;
			
			DisplayMainDeck();
		}
		
		// Shuffles the deck based on the public variable "timesToShuffle"
		void ShuffleDeck(int times){
			
			// Makes it so the player can't press 'Space' to shuffle until shuffling is complete
			//ableToShuffle = false;
			
			for(int i = 0; i < times; i++){
				//Debug.Log("Shuffling " + (i+1) + " time(s).");
				
				// Initializes a temporary deck to shuffle into
				List<Vector2> tempDeck1 = new List<Vector2>();
				for(int j = 0; j < mainDeck.Count; j++){
					tempDeck1.Add(mainDeck[j]);
				}
				
				// Initializes an indexCard and a second temporary deck
				List<Vector2> tempDeck2 = new List<Vector2>();
				int indexCard;
				
				// Adds indexCard to tempDeck2 in a random order
				while(0 < tempDeck1.Count){
					int minimum = 0;
					int maximum = tempDeck1.Count;
					indexCard = UnityEngine.Random.Range(minimum, maximum);
					
					tempDeck2.Add(tempDeck1[indexCard]);
					tempDeck1.RemoveAt(indexCard);
				}
				
				// Adds cards back into mainDeck and prints to console
				mainDeck.Clear();
				for(int k = 0; k < tempDeck2.Count; k++){
					mainDeck.Add(tempDeck2[k]);
				}
				tempDeck2.Clear();
				//Debug.Log("Done shuffling " + (i+1) + ".");
			}
			//ableToShuffle = true;
		}
		
		// Displays the contents of mainDeck
		void DisplayMainDeck(){
		  ShuffleDeck(UnityEngine.Random.Range(5,50));
		  if(simulateAllCardsUseInspectorData) {
		    GetComponent<BBTestSimulationController>().popolateGlobalCardsDeck();
		  } 
		  else if(simulateAllCardsUseSavedData) {
			GetComponent<BBTestSimulationController>().popolateGlobalCardsDeckWhitSavedData();
		  }
		  else if(_BBGlobalDefinitions.useLastCardsHandSavedData) {
		    if( !(_BBLastCardsHand.playersCardsList[0] == Vector2.zero) ) {
		      popolateDeckWithLastSavedData();
		    } else {
			  GlobalDeck.AddRange(mainDeck);
		    }
		  }
		  else {
		    GlobalDeck.AddRange(mainDeck);
		  }
			Debug.Log("Deck size: " + GlobalDeck.Count);
			saveLastcardshand();

		}

	GameObject getCard(Vector2 vCard, bool wantRotate, bool excludeGiveAllCardOpen) {
     
        string type = vCard.x.ToString();
	    string card = vCard.y.ToString();
	    string val = type + "_" + card;	
        
	    GameObject  c = Instantiate(Resources.Load(val)) as GameObject;	
        c.name = val;
		c.GetComponent<BBCard>().v2_value = vCard;
        c.GetComponent<BBCard>().setCardImage(vCard);

		
        if(excludeGiveAllCardOpen) {
          if(wantRotate) {
				c.transform.rotation = rotatedCardRotation;
          } else {
				c.transform.rotation = rotatedCardRotation180;
		  }
        }

        return c;
     }
  
	GameObject getCard(Vector2 vCard, bool wantRotate) {
     
        string type = vCard.x.ToString();
	    string card = vCard.y.ToString();
	    string val = type + "_" + card;	
        
	    GameObject  c = Instantiate(Resources.Load(val)) as GameObject;	
        c.name = val;

		c.GetComponent<BBCard>().v2_value = vCard;
		c.GetComponent<BBCard>().setCardImage(vCard);

        if(wantRotate) {
            if(giveAllCardsOpen) {
            } else {
               c.transform.rotation = rotatedCardRotation;
            }
         } else {
			if(giveAllCardsOpen) {
            } else {
               c.transform.rotation = rotatedCardRotation180;
            }
		 }

        return c;
     }


     Transform[] getGiveCardsList(int startPos) {

	     Transform[] tmpRes = new Transform[playerDataList.Count * 2];

		int idx = startPos;
        int counter = 1;
        int lastpos = (playerDataList.Count) - 1;


		for(int x = 0;x < tmpRes.Length; x++) {
	                if(counter == 1) {
                       tmpRes[x] = playersCardPositionsRoot.Find(idx.ToString() + "_" + counter.ToString()); 
                       counter++;         	                   
		            } else {
				      tmpRes[x] = playersCardPositionsRoot.Find(idx.ToString() + "_" + counter.ToString()); 
				      counter = 1;
				      idx++;       
				      if(idx == lastpos+1) idx = 0; 	                   
		            }
		}

		 return tmpRes;
     }


	public void saveLastcardshand() {

		for(int x = 0;x < _BBLastCardsHand.playersCardsList.Length;x++) {

	     if(x == 25) {
	        break;
	     } else {
	       _BBLastCardsHand.playersCardsList[x] = GlobalDeck[x];
	     }

	  }

	}

	void popolateDeckWithLastSavedData() {

		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[0]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[1]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[2]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[3]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[4]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[5]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[6]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[7]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[8]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[9]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[10]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[11]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[12]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[13]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[14]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[15]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[16]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[17]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[18]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[19]);
		// players cards end
		// to burn
		 GlobalDeck.Add(new Vector2(1,1));
        // flop
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[20]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[21]);
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[22]);
		// to burn
		 GlobalDeck.Add(new Vector2(1,2));
		// turn
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[23]);
		// to burn
		 GlobalDeck.Add(new Vector2(1,3));
		// river
		 GlobalDeck.Add( _BBLastCardsHand.playersCardsList[24]);

	}

	public void checkForActivePlayersNumberAfterFold(int playerID, out int activePlayers) {

		 int outCounter = 10;
		 activePlayers = 0;

		 foreach(BBPlayerData bbpd in playerDataList) {
		   if(bbpd.isOutOfGame || bbpd.runOutOfMoney) {
		     outCounter--;
		   }
		 }

		 activePlayers = outCounter;

	}

	public Vector2 getAllFoldAndRunOutPlayers() {

		int foldPlayers = 0;
		int runOutPlayers = 0;

		 foreach(BBPlayerData bbpd in playerDataList) {
		   if(bbpd.isOutOfGame) foldPlayers++;
			if(bbpd.runOutOfMoney) runOutPlayers++;
		 }

		 Vector2 tempRes =  new Vector2((float)foldPlayers,(float)runOutPlayers);

		 return tempRes;

	}
}
}