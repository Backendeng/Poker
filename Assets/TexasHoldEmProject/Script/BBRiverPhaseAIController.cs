using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BLabTexasHoldEmProject {

public class BBRiverPhaseAIController : MonoBehaviour {

	BBGameController BBGC;
#if USE_PHOTON_BB
	BBGameControllerInSceneMultiplayer BBGC_M;
#endif
	bool isMultiplayer = false;

	void Awake() {
		isMultiplayer = (SceneManager.GetActiveScene().name.Contains("Multiplayer"));

		if(isMultiplayer) {
			#if USE_PHOTON_BB
			BBGC_M = GetComponent<BBGameControllerInSceneMultiplayer>();
			#endif
		} else {
			BBGC = GetComponent<BBGameController>();
		}
	}

	public IEnumerator executeShowRiverResponceRequest(int underGunPlayer) {
			#if USE_PHOTON_BB
			int playerCounter = underGunPlayer;
			if(BBGC_M.playerDataList[playerCounter].playerName == PhotonNetwork.player.name) {
					BBGC_M.UpadatePlayerGameState(playerCounter, BBGlobalDefinitions.GamePhaseDetail.River);
					GetComponent<BBGuiInterfaceMultiplayer>().activatePlayerToTalk(playerCounter);
					GetComponent<BBGuiInterfaceMultiplayer>().setGameButtons(true,true,true,false,false);
				    BBGC_M.waitingForManualButtonResponce = true;
				    yield return new WaitForEndOfFrame();
					yield return new WaitUntil(() => BBGC_M.waitingForManualButtonResponce == false);
				}
			#endif
			yield break;
	}


	public IEnumerator startExecuteRiverResult() {

		BBGC._BBGlobalDefinitions.roundRaiseCounter = 0;
	    GetComponent<BBGuiInterface>().TextGameRAISECounter.text = "Round Raise : " + BBGC._BBGlobalDefinitions.roundRaiseCounter.ToString();
	    BBGC._BBGlobalDefinitions.lastBet = 0;
	    GetComponent<BBGuiInterface>().setLastBetValue(0);

	    int maxCardVal = 0;

		int playerCounter = (int)BBGC._BBGlobalDefinitions.firstLastPlayerToTalkOnTable.x;

		for(int x = 0; x < BBGC.playerDataList.Count;x++) {

			if(!(BBGC.playerDataList[playerCounter].isOutOfGame) && !(BBGC._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[playerCounter]) ) {

				           GetComponent<BBGuiInterface>().activatePlayerToTalk(playerCounter);

					       if(playerCounter == BBGC._BBGlobalDefinitions.localPlayer) {
					           yield return StartCoroutine( executeManualOnRiverAction(playerCounter) );
					           if(BBStaticData.debugRiverController) Debug.Log("--------startExecuteRiverResult------------***DONE MANUAL***------------------->> startExecuteRiverResult playerToExecute : " + playerCounter);
					       } else {
					            BBGC.playerDataList[playerCounter].coeffCardsValOnFlopPhase = getCardsPointCoefficientOnRiverPhase(playerCounter, out maxCardVal);

						        yield return StartCoroutine( executeAIOnRiverAction(playerCounter) );
					                   
						   }
					
			} else {
				if(BBStaticData.debugRiverController) Debug.Log("ZZZZZZZZzzzzzzzzzzzzzzzzzzzzzz--------isOutOfGame------------***startExecuteRiverResult***------------------->> startExecuteRiverResult playerToExecute : " + playerCounter);
			}

             playerCounter++;
			 if(playerCounter == 8) playerCounter = 0;

			               if(BBGC.playerDataList[playerCounter].isOutOfGame)
					           yield return new WaitForSeconds(BBStaticData.waitForPlayerCheckOut);
                           else 
				               yield return new WaitForSeconds(BBStaticData.waitForPlayerCheck);
		}

		yield return StartCoroutine( executeFinishRiverRound() );

		yield break;
	}

	IEnumerator executeManualOnRiverAction(int playerID) {
	  
		GetComponent<BBGuiInterface>().activatePlayerToTalk(playerID);

		if(BBGC._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.River) {
		   GetComponent<BBGuiInterface>().setGameButtons(true,true,true,true,true);
		}
		else if(BBGC._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.ClosingRiver) {
		   GetComponent<BBGuiInterface>().setGameButtons(true,true,false,false,false);
		}

		BBGC.waitingForManualButtonResponce = true;

		yield return new WaitUntil(() => BBGC.waitingForManualButtonResponce == false);
	}


	int getCardsPointCoefficientOnRiverPhase(int playerID,out int maxCardValue ) {
	   int tmpRetVal = 0;
	   maxCardValue = 0;

	   BBPlayerCardsResultValueController.CardsValues retVal = GetComponent<BBPlayerCardsResultValueController>().getCardsResult(playerID,out maxCardValue);

		if(BBStaticData.debugRiverController) Debug.Log("getCardsPointCoefficientOnRiverPhase--> retVal : " + retVal + " maxCardValue : " + maxCardValue + " playerID : " + playerID);

		switch(retVal) {
		    case BBPlayerCardsResultValueController.CardsValues.RoyalFlush: tmpRetVal = 1; break;
			case BBPlayerCardsResultValueController.CardsValues.StraightFlush: tmpRetVal = 2; break;
			case BBPlayerCardsResultValueController.CardsValues.Poker: tmpRetVal = 3; break;
			case BBPlayerCardsResultValueController.CardsValues.FullHouse: tmpRetVal = 4; break;
			case BBPlayerCardsResultValueController.CardsValues.Flush: tmpRetVal = 5; break;
			case BBPlayerCardsResultValueController.CardsValues.Straight: tmpRetVal = 6; break;
			case BBPlayerCardsResultValueController.CardsValues.ThreeOfAkind: tmpRetVal = 7; break;
			case BBPlayerCardsResultValueController.CardsValues.TwoPair: tmpRetVal = 8; break;
		    case BBPlayerCardsResultValueController.CardsValues.Pair: tmpRetVal = 9; break;
		    case BBPlayerCardsResultValueController.CardsValues.HighCard: tmpRetVal = 10; break;
		}

 	  	return tmpRetVal;

	}

	IEnumerator executeAIOnRiverAction(int playerID) {

	 BBGC._BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.River;
	 int cardValCoeff = BBGC.playerDataList[playerID].coeffCardsValOnFlopPhase;
	 int cardMaxVal = BBGC.playerDataList[playerID].maxCardValOnFlopPhase;

	 string action = "CALL";

	 int useBluff = UnityEngine.Random.Range(1,5);

	  if(useBluff == 2) {
			action = "Raise";
	  } 
	  else if(useBluff == 3) {
			action = "Check";
	  }
	  else {
			switch(cardValCoeff) {
		        case 1: action = "Raise"; break;
				case 2: action = "Raise"; break;
				case 3: action = "Raise"; break;
				case 4: action = "Raise"; break;
				case 5: action = "Raise"; break;
				case 6: action = "Check"; break;
				case 7: action = "Check"; break;
				case 8: action = "Check"; break;
				case 9: action = "Check"; break;
				case 10: action = "Check"; break; 
	        }
	  }


      if(GetComponent<BBTestSimulationController>().useSimulate) {
			if(GetComponent<BBTestSimulationController>().simulateAiPhasePlayers_Flop[playerID] == BBGlobalDefinitions.GamePhaseDetail.Turn) {
				switch(GetComponent<BBTestSimulationController>().simulateAiCommandPlayers_Flop[playerID]) {
				   case BBTestSimulationController.TestSimulateAiCommand.Raise: action = "Raise"; break;
				   case BBTestSimulationController.TestSimulateAiCommand.Check: action = "Check"; break;
				}
			}
      }


		if(BBGC.allInInAction && playerID != BBGC.playerDataList[playerID].playerPosition) {
			if( (action == "Raise") || (action == "Raise") ) {
				if(GetComponent<BBAllInController>().checkIfCanAllInForAIPlayers(playerID,BBGC.playerDataList[playerID].currentPlayerTotalMoney,BBGC.allInInActionValue)) {
	             } else {
					 StartCoroutine(executeRiverFold(playerID));
	             }
	        } else {
				StartCoroutine(executeRiverFold(playerID));
	        }
             yield break;
       } else {
       }

		if(BBStaticData.debugRiverController) Debug.Log("BBPlayerAIController------> executeAIOnTurnAction -----> cardValCoeff : " + cardValCoeff + " action : " + action + " cardMaxVal : " + cardMaxVal);

		if(action == "Raise") {
			GetComponent<BBGuiInterface>().setPlayerBetType(playerID,action);
			StartCoroutine(executeRiverRaise(playerID));
		}
		else if(action == "Check") {
		   GetComponent<BBGuiInterface>().setPlayerBetType(playerID,action);
		   GetComponent<BBGuiInterface>().activatePlayerToTalk(playerID);
		}

	

	   yield break;
	}

	IEnumerator executeFinishRiverRound() {

	    BBGC._BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.ClosingRiver;
		GetComponent<BBGuiInterface>().TextGamePhase.text = "Phase : " + BBGC._BBGlobalDefinitions.gamePhaseDetail;

		float[] valList = new float[BBGC.playerDataList.Count]; for(int x = 0; x < valList.Length;x++) valList[x] = BBGC.playerDataList[x].currentMoneyOnTable;

		float valueToCover = Mathf.Max(valList);

		int playerCounter = (int)BBGC._BBGlobalDefinitions.firstLastPlayerToTalkOnTable.x;

		for(int x = 0; x < BBGC.playerDataList.Count;x++) {

			if(!(BBGC.playerDataList[playerCounter].isOutOfGame) && !(BBGC._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[playerCounter]) ) {

				            GetComponent<BBGuiInterface>().activatePlayerToTalk(playerCounter);

				           if(playerCounter == BBGC._BBGlobalDefinitions.localPlayer) {
					          yield return StartCoroutine( executeManualOnRiverAction(playerCounter) );
					         if(BBStaticData.debugFlopController) Debug.Log("---------executeFinishFlopRound-----------***DONE MANUAL***------------------->> executeFlopResult playerToExecute : " + playerCounter + " x : " + x);
					         } else {
					             float tmpVal = BBGC.playerDataList[playerCounter].currentMoneyOnTable;
				                 float tmpToCover = valueToCover - tmpVal;
					             GetComponent<BBGuiInterface>().setPlayerMoneyValue(tmpToCover,playerCounter);
					             GetComponent<BBGuiInterface>().setGreenOnPlayer(playerCounter);
					             yield return StartCoroutine( executeAIOnFinishRiverAction(playerCounter,tmpToCover) );
			                }

			}

			playerCounter++;
			if(playerCounter == 8) playerCounter = 0;

			               if(BBGC.playerDataList[playerCounter].isOutOfGame)
					           yield return new WaitForSeconds(BBStaticData.waitForPlayerCheckOut);
                           else 
				               yield return new WaitForSeconds(BBStaticData.waitForPlayerCheck);

			               GetComponent<BBGuiInterface>().setPlayerBetType(playerCounter,"");
			           
		}

		Debug.Log("-------------------------------->> executeFinishRiverRound playerToExecute : " + playerCounter);

		if(BBGC.checkIfLocalplayerWonBeforeShowDown()) yield break;

		if(BBGC.allInInAction) {
		     BBGC._BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.ShowDownAllInOnTurn;
			 GetComponent<BBGuiInterface>().TextGamePhase.text = BBGC._BBGlobalDefinitions.gamePhaseDetail.ToString(); 
		     GetComponent<BBAllInController>().executeShowDowmAllin();
		} else {
		  StartCoroutine( BBGC.startExecuteShowDownPhase() );
		}

	    yield break;
	}

	IEnumerator executeAIOnFinishRiverAction(int playerID, float valToCover) {

		if(BBGC.allInInAction && (playerID != BBGC.allInInActionForPlayer)) {

		   yield return new WaitForEndOfFrame();

			 if(GetComponent<BBAllInController>().checkIfCanAllInForAIPlayers(playerID,BBGC.playerDataList[playerID].currentPlayerTotalMoney,BBGC.allInInActionValue)) {
             } else {
				 StartCoroutine(executeRiverFold(playerID));
             }

				yield break;
       }

		StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(valToCover,playerID,GetComponent<BBMoveingObjectsController>().playersChipEndingPoint[playerID]) );
	    
	    yield break;
	}

	public IEnumerator executeRiverRaise(int playerID) {

       if(isMultiplayer) {
				#if USE_PHOTON_BB
				float raiseBetVal = 0;
			      BBGC_M.photonView.RPC("RPCUpdateRaiseCounter",PhotonTargets.AllViaServer,false);

				raiseBetVal = BBGC_M._BBGlobalDefinitions.roundRaiseCounter * BBGC_M._BBGlobalDefinitions.smallBlindValue;

				raiseBetVal = raiseBetVal + BBGC_M._BBGlobalDefinitions.lastBet;

				GetComponent<BBGuiInterfaceMultiplayer>().setLastBetValue(raiseBetVal);

				GetComponent<BBGuiInterfaceMultiplayer>().setPlayerMoneyValue(raiseBetVal,playerID);

				if(BBStaticData.debugTurnController) Debug.Log("executeRiverRaise Player id : " + playerID + " :BBPlayerAIController------> executeRiverRaise -----> raiseBetVal : " + raiseBetVal);

				BBGC_M.photonView.RPC("RPCFlopMoveChip",PhotonTargets.All, raiseBetVal,playerID);
				#endif
       } else {
			float raiseBetVal = 0;
			BBGC._BBGlobalDefinitions.roundRaiseCounter++;
			GetComponent<BBGuiInterface>().TextGameRAISECounter.text = "Round Raise : " + BBGC._BBGlobalDefinitions.roundRaiseCounter.ToString();
			raiseBetVal = BBGC._BBGlobalDefinitions.roundRaiseCounter * BBGC._BBGlobalDefinitions.smallBlindValue;
			GetComponent<BBGuiInterface>().setLastBetValue(raiseBetVal);
			GetComponent<BBGuiInterface>().setPlayerMoneyValue(raiseBetVal,playerID);
			yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(raiseBetVal,playerID,GetComponent<BBMoveingObjectsController>().PlayersChipRaiseBetPosition[playerID]) );
	   }
	}

	public IEnumerator executeRiverCall(int playerID) {

          if(isMultiplayer) {
				#if USE_PHOTON_BB
				if(BBStaticData.debugRiverController) Debug.Log("executeRiverCall---- player : " + playerID + " phase : " + BBGC_M._BBGlobalDefinitions.gamePhaseDetail);
					float toBet = BBGC_M._BBGlobalDefinitions.lastBet;
					GetComponent<BBGuiInterfaceMultiplayer>().setPlayerMoneyValue(toBet,playerID);
					if(BBStaticData.debugFlopController) Debug.Log("executeFlopCall--2-- player : " + playerID + " toBet : " + toBet);
				     GetComponent<PhotonView>().RPC("RPC_moveChip",PhotonTargets.All ,toBet,playerID);
				#endif
          } else {
			   if(BBGC._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.River) {
					float toBet = BBGC._BBGlobalDefinitions.lastBet;
					GetComponent<BBGuiInterface>().setPlayerMoneyValue(toBet,playerID);
				    yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(toBet,playerID,null) );
			   } else {
					float[] valList = new float[BBGC.playerDataList.Count]; for(int x = 0; x < valList.Length;x++) valList[x] = BBGC.playerDataList[x].currentMoneyOnTable;
				    float valueToCover = Mathf.Max(valList);
					float tmpVal = BBGC.playerDataList[playerID].currentMoneyOnTable;
				    float tmpToCover = valueToCover - tmpVal;
					GetComponent<BBGuiInterface>().setPlayerMoneyValue(tmpToCover,playerID);
					Debug.Log("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@executeRiverCall---- player : " + playerID + " tmpToCover : " + tmpToCover);

					yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(tmpToCover,playerID,GetComponent<BBMoveingObjectsController>().PlayersChipRaiseBetPosition[playerID]) );

					    if(BBGC._BBGlobalDefinitions.gameType == BBGlobalDefinitions.GameType.NoLimit) {
						   bool allInCovered = false; for(int x = 0;x < BBGC.playerDataList.Count;x++) { if(BBGC.playerDataList[x].underAllin == true) allInCovered = true;} 
						   if(!allInCovered) GetComponent<BBAllInController>().cleanAllInRequest();
						}

			   }
	     }
	}


	public IEnumerator executeRiverCheck(int playerID) {
      yield break;
	}

	public IEnumerator executeRiverFold(int playerID) {

	   if(isMultiplayer) {
				#if USE_PHOTON_BB
				BBGC_M.photonView.RPC("RPCExecuteFold",PhotonTargets.AllViaServer,playerID);
				#endif
	   } else {
				int activePlayer = 0;
		        BBGC.checkForActivePlayersNumberAfterFold(playerID,out activePlayer);
		        if(activePlayer < 3) yield break;

		    yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(BBGC.playerDataList[playerID].transform_card_1.gameObject, BBGC.cardsDiscardPosition) );
		    yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(BBGC.playerDataList[playerID].transform_card_2.gameObject, BBGC.cardsDiscardPosition) );
		         GetComponent<BBGuiInterface>().setPlayerOUTGame(playerID);
		         BBGC.playerDataList[playerID].isOutOfGame = true;
	   }
	}


}
}