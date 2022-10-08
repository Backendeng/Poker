using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BLabTexasHoldEmProject {

public class BBFlopPhaseAIController : MonoBehaviour {

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


	public IEnumerator executeShowFlopResponceRequest(int underGunPlayer) {
			#if USE_PHOTON_BB
			int playerCounter = underGunPlayer;
			if(BBGC_M.playerDataList[playerCounter].playerName == PhotonNetwork.player.name) {
					BBGC_M.UpadatePlayerGameState(playerCounter, BBGlobalDefinitions.GamePhaseDetail.Flop);
					GetComponent<BBGuiInterfaceMultiplayer>().activatePlayerToTalk(playerCounter);
					GetComponent<BBGuiInterfaceMultiplayer>().setGameButtons(true,true,true,false,false);
				    BBGC_M.waitingForManualButtonResponce = true;
				    yield return new WaitForEndOfFrame();
					yield return new WaitUntil(() => BBGC_M.waitingForManualButtonResponce == false);
				}
			 #endif
			 yield break;
	}

	public IEnumerator executeClosingFlopAllighMoneyValues(int underGunPlayer) {
#if USE_PHOTON_BB
	 int playerCounter = underGunPlayer;

			if(BBStaticData.debugPreFlopController) Debug.Log("[executeClosingFlopAllighMoneyValues][executeClosingFlopAllighMoneyValues] underGunPlayer : " + 
		                                        underGunPlayer + 
		                                        " phase : " + BBGC_M._BBGlobalDefinitions.gamePhaseDetail +
				                                " isLocalPlayer : " + BBGC_M.playerDataList[playerCounter].isLocalPlayer +
				                                 " name : " + BBGC_M.playerDataList[playerCounter].playerName + 
				                                 " net player name : " + PhotonNetwork.player.name +
				                                 " is Multiplayer : " + isMultiplayer
		                                        );

				if(BBGC_M.playerDataList[playerCounter].playerName == PhotonNetwork.player.name) {
				    GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().genBip);
						BBGC_M.UpadatePlayerGameState(playerCounter, BBGlobalDefinitions.GamePhaseDetail.ClosingFlop);
					    GetComponent<BBGuiInterfaceMultiplayer>().setActionButtonPosition(true);
						GetComponent<BBGuiInterfaceMultiplayer>().setGameButtons(true,true,false,false,false);
					    BBGC_M.waitingForManualButtonResponce = true;
					    yield return new WaitForEndOfFrame();
						yield return new WaitUntil(() => BBGC_M.waitingForManualButtonResponce == false);
				}
#endif
       yield break;
	}

    public IEnumerator startExecuteClosingFlopResult() {
#if USE_PHOTON_BB
			int playerCounter = BBGC_M._BBGlobalDefinitions.playerToTalk;
			GetComponent<BBGuiInterfaceMultiplayer>().TextGameRAISECounter.text = "Round Raise : " + BBGC_M._BBGlobalDefinitions.roundRaiseCounter.ToString();


			float maxPlayerVal = 0;
			foreach (BBPlayerData pd in BBGC_M.playerDataList) {
              if(pd.currentMoneyOnTable >= maxPlayerVal ) maxPlayerVal = pd.currentMoneyOnTable;
	        }

			if(BBGC_M.playerDataList[playerCounter].playerName == PhotonNetwork.player.name) {

		        if(BBGC_M.playerDataList[playerCounter].currentMoneyOnTable < maxPlayerVal) {
					BBGC_M.UpadatePlayerGameState(playerCounter, BBGlobalDefinitions.GamePhaseDetail.ClosingFlop);
					//GetComponent<BBGuiInterfaceMultiplayer>().activatePlayerToTalk(playerCounter);
					yield return StartCoroutine( executeManualOnFlopAction(playerCounter) );
		        } else {
					BBGC_M.UpadatePlayerGameState(playerCounter, BBGlobalDefinitions.GamePhaseDetail.ClosingFlop);
					yield return StartCoroutine( executeManualOnFlopAction(playerCounter) );
		        }

		    }
#endif
        yield break;
    }


	public IEnumerator startExecuteFlopResult() {

	 if(isMultiplayer) {
	 } else {

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
								      yield return StartCoroutine( executeManualOnFlopAction(playerCounter) );
							          if(BBStaticData.debugFlopController) Debug.Log("--------startExecuteFlopResult------------***DONE MANUAL***------------------->> executeFlopResult playerToExecute : " + playerCounter);
							       } else {
								        BBGC.playerDataList[playerCounter].coeffCardsValOnFlopPhase = getCardsPointCoefficientOnFlopPhase(playerCounter, out maxCardVal);
								        BBGC.playerDataList[playerCounter].maxCardValOnFlopPhase = maxCardVal;

								        yield return StartCoroutine( executeAIOnFlopAction(playerCounter) );
							                   
								   }
							
					} else {
						if(BBStaticData.debugFlopController) Debug.Log("--------isOutOfGame------------***startExecuteFlopResult***------------------->> executeFlopResult playerToExecute : " + playerCounter);
					}

		             playerCounter++;
					 if(playerCounter == 8) playerCounter = 0;

					               if(BBGC.playerDataList[playerCounter].isOutOfGame)
							           yield return new WaitForSeconds(BBStaticData.waitForPlayerCheckOut);
		                           else 
						               yield return new WaitForSeconds(BBStaticData.waitForPlayerCheck);
		        
				}


				yield return StartCoroutine( executeFinishFlopRound() );
	 }

        yield break; 

	}



	IEnumerator executeFinishFlopRound() {

		if(BBStaticData.debugFlopController) Debug.Log("------------executeFinishFlopRound ---------------");

	    BBGC._BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.ClosingFlop;
		GetComponent<BBGuiInterface>().TextGamePhase.text = "Phase : " + BBGC._BBGlobalDefinitions.gamePhaseDetail;

		float[] valList = new float[BBGC.playerDataList.Count]; for(int x = 0; x < valList.Length;x++) valList[x] = BBGC.playerDataList[x].currentMoneyOnTable;

		float valueToCover = Mathf.Max(valList);

		int playerCounter = (int)BBGC._BBGlobalDefinitions.firstLastPlayerToTalkOnTable.x;

		for(int x = 0; x < BBGC.playerDataList.Count;x++) {

			if(!(BBGC.playerDataList[playerCounter].isOutOfGame) && !(BBGC._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[playerCounter]) ) {

				            GetComponent<BBGuiInterface>().activatePlayerToTalk(playerCounter);

				           if(playerCounter == BBGC._BBGlobalDefinitions.localPlayer) {
					         yield return StartCoroutine( executeManualOnFlopAction(playerCounter) );
					         if(BBStaticData.debugFlopController) Debug.Log("---------executeFinishFlopRound-----------***DONE MANUAL***------------------->> executeFlopResult playerToExecute : " + playerCounter + " x : " + x);
					         } else {
					             float tmpVal = BBGC.playerDataList[playerCounter].currentMoneyOnTable;
				                 float tmpToCover = valueToCover - tmpVal;
					             GetComponent<BBGuiInterface>().setPlayerMoneyValue(tmpToCover,playerCounter);
					             GetComponent<BBGuiInterface>().setGreenOnPlayer(playerCounter);
					             yield return StartCoroutine( executeAIOnFinishFlopAction(playerCounter,tmpToCover) );
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

		if(BBStaticData.debugFlopController) Debug.Log("------------executeFinishFlopRound  ***DONE*** ");

		if(BBGC.checkIfLocalplayerWonBeforeShowDown()) yield break;

		if(BBGC.allInInAction) {
		     BBGC._BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.ShowDownAllInOnTurn;
			 GetComponent<BBGuiInterface>().TextGamePhase.text = BBGC._BBGlobalDefinitions.gamePhaseDetail.ToString(); 
		     GetComponent<BBAllInController>().executeShowDowmAllin();
		} else {
				StartCoroutine( BBGC.startExecuteTurnPhase() );
        }

	    yield break;
	}

	int getCardsPointCoefficientOnFlopPhase(int playerID,out int maxCardValue ) {
	   int tmpRetVal = 0;
	   maxCardValue = 0;

	   BBPlayerCardsResultValueController.CardsValues retVal = GetComponent<BBPlayerCardsResultValueController>().getCardsResult(playerID,out maxCardValue);

		if(BBStaticData.debugFlopController) Debug.Log("getCardsPointCoefficientOnFlopPhase--> retVal : " + retVal + " maxCardValue : " + maxCardValue + " playerID : " + playerID);

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
		    case BBPlayerCardsResultValueController.CardsValues.HighCard: 
		       if(maxCardValue > 10) tmpRetVal = 9;
             else tmpRetVal = 10; 
		    break;
		}

 	  	return tmpRetVal;

	}

	IEnumerator executeManualOnFlopAction(int playerID) {

	    if(isMultiplayer) {
				#if USE_PHOTON_BB
				GetComponent<BBGuiInterfaceMultiplayer>().activatePlayerToTalk(playerID);

				if(BBGC_M._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.Flop) {
					GetComponent<BBGuiInterfaceMultiplayer>().setGameButtons(true,true,true,true,true);
				}
				else if(BBGC_M._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.ClosingFlop) {
					GetComponent<BBGuiInterfaceMultiplayer>().setGameButtons(true,true,false,false,false);
				}

				BBGC_M.waitingForManualButtonResponce = true;

				yield return new WaitUntil(() => BBGC_M.waitingForManualButtonResponce == false);
				#endif
	     } else { 
				GetComponent<BBGuiInterface>().activatePlayerToTalk(playerID);

				if(BBGC._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.Flop) {
				   GetComponent<BBGuiInterface>().setGameButtons(true,true,true,true,true);
				}
				else if(BBGC._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.ClosingFlop) {
				   GetComponent<BBGuiInterface>().setGameButtons(true,true,false,false,false);
				}

				BBGC.waitingForManualButtonResponce = true;

				yield return new WaitUntil(() => BBGC.waitingForManualButtonResponce == false);
		}
	}

	IEnumerator executeAIOnFinishFlopAction(int playerID, float valToCover) {

		if(BBGC.allInInAction && (playerID != BBGC.allInInActionForPlayer)) {

		   yield return new WaitForEndOfFrame();

			 if(GetComponent<BBAllInController>().checkIfCanAllInForAIPlayers(playerID,BBGC.playerDataList[playerID].currentPlayerTotalMoney,BBGC.allInInActionValue)) {
             } else {
				 StartCoroutine(executeFlopFold(playerID));
             }

				yield break;
       }

		StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(valToCover,playerID,GetComponent<BBMoveingObjectsController>().playersChipEndingPoint[playerID]) );

	    
	    yield break;
	}



	IEnumerator executeAIOnFlopAction(int playerID) {

		yield return new WaitForEndOfFrame();

	 BBGC._BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.Flop;
	 int cardValCoeff = BBGC.playerDataList[playerID].coeffCardsValOnFlopPhase;
	 int cardMaxVal = BBGC.playerDataList[playerID].maxCardValOnFlopPhase;
	 string action = "Check";

	  int useBluff = UnityEngine.Random.Range(1,9);

	  if(useBluff == 2) {
			action = "Raise";
	  } 
	  else if(useBluff == 3) {
			action = "Check";
	  }
	  else if(useBluff == 4) {
			action = "Fold";
	  }
	  else if(useBluff == 5) {
			action = "Call";
	  }
	  else {
			switch(cardValCoeff) {
		        case 1: action = "Raise"; break;
				case 2: action = "Raise"; break;
				case 3: action = "Raise"; break;
				case 4: action = "Raise"; break;
				case 5: action = "Call"; break;
				case 6: action = "Call"; break;
				case 7: action = "Check"; break;
				case 8: action = "Check"; break;
				case 9: action = "Check"; break;
				case 10: action = "Fold"; break; 
	        }
	  }


      if(GetComponent<BBTestSimulationController>().useSimulate) {
			if(GetComponent<BBTestSimulationController>().simulateAiPhasePlayers_Flop[playerID] == BBGlobalDefinitions.GamePhaseDetail.Flop) {
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
					 StartCoroutine(executeFlopFold(playerID));
	             }
	        } else {
				StartCoroutine(executeFlopFold(playerID));
	        }
             yield break;
       } else {
       }

 
		if(BBStaticData.debugFlopController) Debug.Log("Player id : " + playerID + " :BBPlayerAIController------> executeOnFlopAction -----> cardValCoeff : " + 
		cardValCoeff + " action : " + action + " cardMaxVal : " + cardMaxVal + " useBluf : " + useBluff);

		if(action == "Raise") {
			yield return StartCoroutine(executeFlopRaise(playerID));
		}
		else if(action == "Check") {
		  // GetComponent<BBGuiInterface>().activatePlayerToTalk(playerID);
		}
		else if(action == "Call") {
			yield return StartCoroutine(executeFlopCall(playerID));
		}
		else if(action == "Fold") {
			yield return StartCoroutine( executeFlopFold(playerID) );
		}


		GetComponent<BBGuiInterface>().setPlayerBetType(playerID,action);

	   yield break;
	}


	public IEnumerator executeFlopFold(int playerID) {

	  if(isMultiplayer) {
				#if USE_PHOTON_BB
			      if(BBGC_M.playerDataList[playerID].transform_card_1 == null) yield break;
				  BBGC_M.photonView.RPC("RPCExecuteFold",PhotonTargets.AllViaServer,playerID);
				#endif
	  } else {
		    if(BBGC.playerDataList[playerID].transform_card_1 == null) yield break;

			int activePlayer = 0;
	        BBGC.checkForActivePlayersNumberAfterFold(playerID,out activePlayer);
	        if(activePlayer < 3) yield break;


		     yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(BBGC.playerDataList[playerID].transform_card_1.gameObject, BBGC.cardsDiscardPosition) );
		     yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(BBGC.playerDataList[playerID].transform_card_2.gameObject, BBGC.cardsDiscardPosition) );
		         GetComponent<BBGuiInterface>().setPlayerOUTGame(playerID);
		         BBGC.playerDataList[playerID].isOutOfGame = true;
	  }

	}



	public IEnumerator executeFlopRaise(int playerID) {

			if(isMultiplayer) {
				#if USE_PHOTON_BB
				float raiseBetVal = 0;
			      BBGC_M.photonView.RPC("RPCUpdateRaiseCounter",PhotonTargets.AllViaServer,false);

				raiseBetVal = BBGC_M._BBGlobalDefinitions.roundRaiseCounter * BBGC_M._BBGlobalDefinitions.smallBlindValue;

				raiseBetVal = raiseBetVal + BBGC_M._BBGlobalDefinitions.lastBet;

				GetComponent<BBGuiInterfaceMultiplayer>().setLastBetValue(raiseBetVal);

				GetComponent<BBGuiInterfaceMultiplayer>().setPlayerMoneyValue(raiseBetVal,playerID);

				if(BBStaticData.debugFlopController) Debug.Log("executeFlopRaise Player id : " + playerID + " :BBPlayerAIController------> executeOnFlopAction -----> raiseBetVal : " + raiseBetVal);

				BBGC_M.photonView.RPC("RPCFlopMoveChip",PhotonTargets.All, raiseBetVal,playerID);
                #endif
			} else {
				GetComponent<BBGuiInterface>().activatePlayerToTalk(playerID);

				float raiseBetVal = 0;
				BBGC._BBGlobalDefinitions.roundRaiseCounter++;
				GetComponent<BBGuiInterface>().TextGameRAISECounter.text = "Round Raise : " + BBGC._BBGlobalDefinitions.roundRaiseCounter.ToString();
				raiseBetVal = BBGC._BBGlobalDefinitions.roundRaiseCounter * BBGC._BBGlobalDefinitions.smallBlindValue;
				GetComponent<BBGuiInterface>().setLastBetValue(raiseBetVal);

				GetComponent<BBGuiInterface>().setPlayerMoneyValue(raiseBetVal,playerID);

				if(BBStaticData.debugFlopController) Debug.Log("executeFlopFold Player id : " + playerID + " :BBPlayerAIController------> executeOnFlopAction -----> raiseBetVal : " + raiseBetVal);


				yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(raiseBetVal,playerID,GetComponent<BBMoveingObjectsController>().PlayersChipRaiseBetPosition[playerID]) );
		  }
	}

#if USE_PHOTON_BB
	[PunRPC]
	void RPC_moveChip(float toAdd,int playerID) {
		StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(toAdd,playerID,GetComponent<BBMoveingObjectsController>().PlayersChipRaiseBetPosition[playerID]) );
	}
#endif

	public IEnumerator executeClosingFlopCall(int playerID) {
			#if USE_PHOTON_BB
			float maxPlayerVal = 0;
			foreach (BBPlayerData pd in BBGC_M.playerDataList) {
              if(pd.currentMoneyOnTable >= maxPlayerVal ) maxPlayerVal = pd.currentMoneyOnTable;
	        }
			Debug.Log("**********ie_gotGameActionButton**********************CONTROLLO CRASH*****7************ClosingMoneyAllign*****************");

			if(BBGC_M.playerDataList[playerID].playerName == PhotonNetwork.player.name) {
				Debug.Log("**********ie_gotGameActionButton**********************CONTROLLO CRASH*****8************ClosingMoneyAllign*****************");

				if(BBGC_M.playerDataList[playerID].currentMoneyOnTable < maxPlayerVal) {
					float toAdd = maxPlayerVal - BBGC_M.playerDataList[playerID].currentMoneyOnTable;
					BBGC_M.UpadatePlayerGameState(playerID, BBGlobalDefinitions.GamePhaseDetail.ClosingMoneyAllign);
					GetComponent<BBGuiInterfaceMultiplayer>().setPlayerMoneyValue(toAdd,playerID);

					if(isMultiplayer) {
						GetComponent<PhotonView>().RPC("RPC_moveChip",PhotonTargets.All ,toAdd,playerID);
	                } else {
					  yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(toAdd,playerID,GetComponent<BBMoveingObjectsController>().PlayersChipRaiseBetPosition[playerID]) );
					}

		        } else {
					BBGC_M.UpadatePlayerGameState(playerID, BBGlobalDefinitions.GamePhaseDetail.ClosingMoneyAllign);

		        }

		    }
		    #endif
		    yield break;
	}

	public IEnumerator executeFlopCall(int playerID) {

       if(isMultiplayer) {
				#if USE_PHOTON_BB
				    if(BBStaticData.debugFlopController) Debug.Log("executeFlopCall---- player : " + playerID + " phase : " + BBGC_M._BBGlobalDefinitions.gamePhaseDetail);
					float toBet = BBGC_M._BBGlobalDefinitions.lastBet;
					GetComponent<BBGuiInterfaceMultiplayer>().setPlayerMoneyValue(toBet,playerID);
					if(BBStaticData.debugFlopController) Debug.Log("executeFlopCall--2-- player : " + playerID + " toBet : " + toBet);

				     GetComponent<PhotonView>().RPC("RPC_moveChip",PhotonTargets.All ,toBet,playerID);
				#endif
       } else {
			   if(BBGC._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.Flop) {
					float toBet = BBGC._BBGlobalDefinitions.lastBet;
					GetComponent<BBGuiInterface>().setPlayerMoneyValue(toBet,playerID);
				    yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(toBet,playerID,null) );
			   } else {
					float[] valList = new float[BBGC.playerDataList.Count]; for(int x = 0; x < valList.Length;x++) valList[x] = BBGC.playerDataList[x].currentMoneyOnTable;
				    float valueToCover = Mathf.Max(valList);
					float tmpVal = BBGC.playerDataList[playerID].currentMoneyOnTable;
				    float tmpToCover = valueToCover - tmpVal;
					GetComponent<BBGuiInterface>().setPlayerMoneyValue(tmpToCover,playerID);
					if(BBStaticData.debugFlopController) Debug.Log("executeFlopCall---- player : " + playerID + " tmpToCover : " + tmpToCover);

					yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(tmpToCover,playerID,GetComponent<BBMoveingObjectsController>().PlayersChipRaiseBetPosition[playerID]) );

					    if(BBGC._BBGlobalDefinitions.gameType == BBGlobalDefinitions.GameType.NoLimit) {
						   bool allInCovered = false; for(int x = 0;x < BBGC.playerDataList.Count;x++) { if(BBGC.playerDataList[x].underAllin == true) allInCovered = true;} 
						   if(!allInCovered) GetComponent<BBAllInController>().cleanAllInRequest();
						}

			   }
	  }

	}


}
}