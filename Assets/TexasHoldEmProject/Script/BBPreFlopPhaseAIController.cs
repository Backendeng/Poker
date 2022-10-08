using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BLabTexasHoldEmProject {

public class BBPreFlopPhaseAIController : MonoBehaviour {

	BBGameController BBGC;
#if USE_PHOTON_BB
	BBGameControllerInSceneMultiplayer BBGC_M;
#endif
	bool isMultiplayer = false;


	void Awake () {

		isMultiplayer = (SceneManager.GetActiveScene().name.Contains("Multiplayer"));

		if(isMultiplayer) {
			#if USE_PHOTON_BB
			BBGC_M = GetComponent<BBGameControllerInSceneMultiplayer>();
			#endif
		} else {
			BBGC = GetComponent<BBGameController>();
		}

	}


	public void executeSmallBlindBet() {

	  if(isMultiplayer) {
			#if USE_PHOTON_BB
			BBGC_M._BBGlobalDefinitions.currentActivePlayer = BBGC_M._BBGlobalDefinitions.smallBlindPlayerId;
			BBGC_M.playerDataList[BBGC_M._BBGlobalDefinitions.currentActivePlayer].gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.SmallBlind;
			if(BBStaticData.debugPreFlopController) Debug.Log("[BBPreFlopPhaseAIController][executeSmallBlindBet] smallBlindPlayerId : " + BBGC_M._BBGlobalDefinitions.smallBlindPlayerId);
			GetComponent<BBGuiInterfaceMultiplayer>().simulateOnClickButton("BET_25");
			GetComponent<BBGuiInterfaceMultiplayer>().setPlayerBetType(BBGC_M._BBGlobalDefinitions.currentActivePlayer,"SmallBlind");
			#endif
       } else {
			BBGC._BBGlobalDefinitions.currentActivePlayer = BBGC._BBGlobalDefinitions.smallBlindPlayerId;
			if(BBStaticData.debugPreFlopController) Debug.Log("[BBPreFlopPhaseAIController][executeSmallBlindBet] smallBlindPlayerId : " + BBGC._BBGlobalDefinitions.smallBlindPlayerId);
			GetComponent<BBGuiInterface>().simulateOnClickButton("BET_25");
			GetComponent<BBGuiInterface>().setPlayerBetType(BBGC._BBGlobalDefinitions.currentActivePlayer,"SmallBlind");
	  }
	}

	public void executeBigBlindBet() {

		if(isMultiplayer) {
		  #if USE_PHOTON_BB
		  BBGC_M._BBGlobalDefinitions.currentActivePlayer = BBGC_M._BBGlobalDefinitions.bigBlindPlayerId;
		  BBGC_M.playerDataList[BBGC_M._BBGlobalDefinitions.currentActivePlayer].gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.BigBlind;
		  if(BBStaticData.debugPreFlopController) Debug.Log("[BBPreFlopPhaseAIController][executeSmallBlindBet] bigBlindPlayerId : " + BBGC_M._BBGlobalDefinitions.bigBlindPlayerId);
		  GetComponent<BBGuiInterfaceMultiplayer>().simulateOnClickButton("BET_50");
		  GetComponent<BBGuiInterfaceMultiplayer>().setPlayerBetType(BBGC_M._BBGlobalDefinitions.currentActivePlayer,"BigBlind");
		  #endif
		} else {
		  BBGC._BBGlobalDefinitions.currentActivePlayer = BBGC._BBGlobalDefinitions.bigBlindPlayerId;
		  if(BBStaticData.debugPreFlopController) Debug.Log("[BBPreFlopPhaseAIController][executeSmallBlindBet] bigBlindPlayerId : " + BBGC._BBGlobalDefinitions.bigBlindPlayerId);
		  GetComponent<BBGuiInterface>().simulateOnClickButton("BET_50");
		  GetComponent<BBGuiInterface>().setPlayerBetType(BBGC._BBGlobalDefinitions.currentActivePlayer,"BigBlind");
		}
	}



	public IEnumerator executePreFlopFirstBetRound(int underGunPlayer) {

	 int playerCounter = underGunPlayer;
		
		   if(isMultiplayer) {
				#if USE_PHOTON_BB
				if(BBGC_M.playerDataList[playerCounter].playerName == PhotonNetwork.player.name) {
					GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().genBip);
					GetComponent<BBGuiInterfaceMultiplayer>().setGameButtons(true,true,true,false,false);
					GetComponent<BBGuiInterfaceMultiplayer>().setActionButtonPosition(true);
				    BBGC_M.waitingForManualButtonResponce = true;
				    yield return new WaitForEndOfFrame();
					yield return new WaitUntil(() => BBGC_M.waitingForManualButtonResponce == false);
				}
				#endif
		   } else {
			     for(int x = 0; x < BBGC.playerDataList.Count;x++) {
				        if(BBStaticData.debugPreFlopController) Debug.Log(playerCounter + ")+++++++++++++++++++ [BBPreFlopPhaseAIController] after [executePreFlopFirstBetRound] playersStateIsOutDuringOpenGame : " + BBGC._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[playerCounter]); 
			        if(BBGC._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[playerCounter]) {

			         } else {	       
					             GetComponent<BBGuiInterface>().activatePlayerToTalk(playerCounter);

					             if(playerCounter == BBGC._BBGlobalDefinitions.localPlayer) {
						                    GetComponent<BBGuiInterface>().setGameButtons(true,true,true,false,false);
								            BBGC.waitingForManualButtonResponce = true;
				                            yield return new WaitUntil(() => BBGC.waitingForManualButtonResponce == false);
					             } else {

						               yield return StartCoroutine(executePhaseOnAIPlayer(playerCounter));
						               if(BBStaticData.debugPreFlopController) Debug.Log("[BBPreFlopPhaseAIController] after [executePhaseOnAIPlayer] playerCounter : " + playerCounter + " talker : " + BBGC._BBGlobalDefinitions.playerToTalk); 
					             }
				     }
				             playerCounter++;
				             if(playerCounter == 8) playerCounter = 0; 
					               if(BBGC.playerDataList[playerCounter].isOutOfGame)
							           yield return new WaitForSeconds(BBStaticData.waitForPlayerCheckOut);
		                           else 
						               yield return new WaitForSeconds(BBStaticData.waitForPlayerCheck);
			      }

				if(BBStaticData.debugPreFlopController) Debug.Log("#################### DONE READY FOR LAST CECK ##################[BBPreFlopPhaseAIController] after [executePhaseOnAIPlayer] playerCounter : " + playerCounter + " talker : " + BBGC._BBGlobalDefinitions.playerToTalk); 
				yield return StartCoroutine( executePreFlopLastBetChecking() );

		   }



	}

	IEnumerator executePreFlopLastBetChecking() {

		BBGC._BBGlobalDefinitions.gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.ClosingPreFlop;
		GetComponent<BBGuiInterface>().TextGamePhase.text = "Phase : " + BBGC._BBGlobalDefinitions.gamePhaseDetail.ToString();

	 float maxPlayerVal = 0;
     foreach (BBPlayerData pd in BBGC.playerDataList) {
       if(pd.currentMoneyOnTable >= maxPlayerVal ) maxPlayerVal = pd.currentMoneyOnTable;
	 }

		if(BBStaticData.debugPreFlopController) Debug.Log("[BBPreFlopPhaseAIController][executePreFlopLastBetChecking] phase : " + BBGC._BBGlobalDefinitions.gamePhaseDetail + " maxPlayerVal : " + maxPlayerVal);

		int playerCounter = (int)BBGC._BBGlobalDefinitions.firstLastPlayerToTalkOnTable.x;

		for(int x = 0; x < BBGC.playerDataList.Count;x++) {

			   if(BBStaticData.debugPreFlopController) Debug.Log("[BBPreFlopPhaseAIController][executePreFlopLastBetChecking] >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> : " +
				BBGC.playerDataList[playerCounter].isOutOfGame + " : " + BBGC._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[playerCounter] + " PID : " + playerCounter);

			  if(!(BBGC.playerDataList[playerCounter].isOutOfGame) && !(BBGC._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[playerCounter]) ) {

				     GetComponent<BBGuiInterface>().activatePlayerToTalk(playerCounter);
				     GetComponent<BBGuiInterface>().setPlayerBetType(playerCounter,"");

				    float diff = maxPlayerVal - BBGC.playerDataList[playerCounter].currentMoneyOnTable;
				    if(BBStaticData.debugPreFlopController) Debug.Log("[BBPreFlopPhaseAIController][executePreFlopLastBetChecking] currentActivedealer diff : " + diff + " player : " + playerCounter);

							if(playerCounter == BBGC._BBGlobalDefinitions.localPlayer) {

							       GetComponent<BBGuiInterface>().setGameButtons(true,true,false,false,false);
						           BBGC.waitingForManualButtonResponce = true;

		                           yield return new WaitUntil(() => BBGC.waitingForManualButtonResponce == false);
						         
						         GetComponent<BBGuiInterface>().setPlayerMoneyValue(diff,playerCounter);
							      yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(diff,playerCounter,null) );
							} else {
						       GetComponent<BBGuiInterface>().setPlayerMoneyValue(diff,playerCounter);
							   yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(diff,playerCounter,null) );
							}
			 } 
                    playerCounter++;
			        if(playerCounter == 8) playerCounter = 0;

			               if(BBGC.playerDataList[playerCounter].isOutOfGame)
					           yield return new WaitForSeconds(BBStaticData.waitForPlayerCheckOut);
                           else 
				               yield return new WaitForSeconds(BBStaticData.waitForPlayerCheck);


	   }

		
					yield return StartCoroutine( BBGC.startExecuteFlopPhase() );

	}


	IEnumerator executePhaseOnAIPlayer(int playerID) {

		int coeffvalue = BBStaticData.getCardsPoint( BBGC.playerDataList[playerID].card_1_Value, BBGC.playerDataList[playerID].card_2_Value);

		int useBluff = 0;
		if(GetComponent<BBTestSimulationController>().useSimulate) {
			if(GetComponent<BBTestSimulationController>().simulateAiPhasePlayers_FirstBettingRound[playerID] == BBGlobalDefinitions.GamePhaseDetail.FirstBettingRound) {
				switch(GetComponent<BBTestSimulationController>().simulateAiCommandPlayers_FirstBettingRound[playerID]) {
				   case BBTestSimulationController.TestSimulateAiCommand.Raise: 
					StartCoroutine(executeRaise(playerID));
					GetComponent<BBGuiInterface>().setPlayerBetType(playerID,"Raise");
				    break;
				   case BBTestSimulationController.TestSimulateAiCommand.Call: 
					StartCoroutine(executeCall(playerID));
					GetComponent<BBGuiInterface>().setPlayerBetType(playerID,"Call");
				    break;
				   case BBTestSimulationController.TestSimulateAiCommand.Fold: 
					StartCoroutine(executeFold(playerID));
					GetComponent<BBGuiInterface>().setPlayerBetType(playerID,"Fold");
				    break;
				}
			}
        } else {

						  useBluff = UnityEngine.Random.Range(1,7);
						  string tmpBetType = "";

						  if(useBluff == 5) {
							        tmpBetType = "Call";
						  } 
						  else if(useBluff == 4) {
							        tmpBetType = "Raise";
						  }
						  else if(useBluff == 2) {
							       tmpBetType = "Fold";
						  }
						  else {
								switch(coeffvalue) {
								case 0:
								tmpBetType = "Fold";
								break;
								case 5:
								tmpBetType = "Call";
								break;
								case 10:
								tmpBetType = "Raise";
								break;
								case 20:
								tmpBetType = "Raise";
								break;
								default:
								tmpBetType = "Call";
								break;
								}
				          }

				          switch(tmpBetType) {
						    case "Call":
				               yield return StartCoroutine(executeCall(playerID));
							break;
						    case "Fold":
				               yield return StartCoroutine(executeFold(playerID));
							break;
							case "Raise":
				              yield return StartCoroutine(executeRaise(playerID));
							break;
				          }
			            GetComponent<BBGuiInterface>().setPlayerBetType(playerID,tmpBetType);
                  }


		if(BBStaticData.debugPreFlopController) Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*********executePhaseOnAIPlayer**************BBPreFlopPhaseAIController coeffvalue : " + coeffvalue + " playerPos : " + 
			playerID + " phase : " + BBGC._BBGlobalDefinitions.gamePhaseDetail + " useBluf : " + useBluff);

                  yield break;
	}

#if USE_PHOTON_BB
	[PunRPC]
	void RPCexecuteFold(int playerID) {
		StartCoroutine(ie_RPCexecuteFold(playerID));
	}
#endif

	IEnumerator ie_RPCexecuteFold(int playerID) {
			#if USE_PHOTON_BB
			yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(BBGC_M.playerDataList[playerID].transform_card_1.gameObject, BBGC_M.cardsDiscardPosition) );
			yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(BBGC_M.playerDataList[playerID].transform_card_2.gameObject, BBGC_M.cardsDiscardPosition) );
		    GetComponent<BBGuiInterfaceMultiplayer>().setPlayerOUTGame(playerID);
		    BBGC_M.photonView.RPC("setPlayerOutOfGame",PhotonTargets.AllViaServer,playerID,true);
		    GetComponent<BBGuiInterfaceMultiplayer>().setPlayerBetType(playerID,"Fold");
		    #endif
		    yield break;
	}

	public IEnumerator executeFold(int playerID) {


	  int activePlayer = 0;

	  if(isMultiplayer) {
		  #if USE_PHOTON_BB
		  GetComponent<PhotonView>().RPC("RPCexecuteFold",PhotonTargets.AllViaServer,playerID);
		  #endif
	  } else {
		      BBGC.checkForActivePlayersNumberAfterFold(playerID,out activePlayer);
		      if(activePlayer < 3) yield break;

				yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(BBGC.playerDataList[playerID].transform_card_1.gameObject, BBGC.cardsDiscardPosition) );
				yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveCard(BBGC.playerDataList[playerID].transform_card_2.gameObject, BBGC.cardsDiscardPosition) );
				         GetComponent<BBGuiInterface>().setPlayerOUTGame(playerID);
				         BBGC.playerDataList[playerID].isOutOfGame = true;
				         GetComponent<BBGuiInterface>().setPlayerBetType(playerID,"Fold");
      }

	}

#if USE_PHOTON_BB
	[PunRPC]
	void RPCmoveChip(float toBet,int playerID) {
		StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(toBet,playerID,null) );
	}

    public IEnumerator executeFirstBetting(int playerID) {
			        float toBet = BBGC_M._BBGlobalDefinitions.bigBlindValue;
					GetComponent<BBGuiInterfaceMultiplayer>().setPlayerMoneyValue(toBet,playerID);
				    //yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(toBet,playerID,null) );
			        GetComponent<PhotonView>().RPC("RPCmoveChip",PhotonTargets.All,toBet,playerID);
			        yield break;
    }
#endif

	public IEnumerator executeCall(int playerID) {
		Debug.Log("BetType_test_executeCall");
	     if(isMultiplayer) {
#if USE_PHOTON_BB
				if(BBGC_M._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.FirstBettingRound) {
					float toBet = BBGC_M._BBGlobalDefinitions.lastBet;

					if(playerID == BBGC_M._BBGlobalDefinitions.smallBlindPlayerId) {
					  toBet -= BBGC_M._BBGlobalDefinitions.smallBlindValue;
					} 
					else if(playerID == BBGC_M._BBGlobalDefinitions.bigBlindPlayerId) {
						toBet -= BBGC_M._BBGlobalDefinitions.bigBlindValue;
					}

					GetComponent<BBGuiInterfaceMultiplayer>().setPlayerMoneyValue(toBet,playerID);
				    yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(toBet,playerID,null) );
				}
				else {
					float[] valList = new float[BBGC_M.playerDataList.Count]; for(int x = 0; x < valList.Length;x++) valList[x] = BBGC_M.playerDataList[x].currentMoneyOnTable;
				    float valueToCover = Mathf.Max(valList);
					float tmpVal = BBGC_M.playerDataList[playerID].currentMoneyOnTable;
				    float tmpToCover = valueToCover - tmpVal;
					GetComponent<BBGuiInterfaceMultiplayer>().setPlayerMoneyValue(tmpToCover,playerID);
				}

			    GetComponent<BBGuiInterfaceMultiplayer>().setPlayerBetType(playerID,"Call");
#endif
	     } else {
				if(BBGC._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.FirstBettingRound) {
					float toBet = BBGC._BBGlobalDefinitions.lastBet;

					if(playerID == BBGC._BBGlobalDefinitions.smallBlindPlayerId) {
					  toBet -= BBGC._BBGlobalDefinitions.smallBlindValue;
					} 
					else if(playerID == BBGC._BBGlobalDefinitions.bigBlindPlayerId) {
						toBet -= BBGC._BBGlobalDefinitions.bigBlindValue;
					}

					GetComponent<BBGuiInterface>().setPlayerMoneyValue(toBet,playerID);
				    yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(toBet,playerID,null) );
				}
				else {

					float[] valList = new float[BBGC.playerDataList.Count]; for(int x = 0; x < valList.Length;x++) valList[x] = BBGC.playerDataList[x].currentMoneyOnTable;
				    float valueToCover = Mathf.Max(valList);
					float tmpVal = BBGC.playerDataList[playerID].currentMoneyOnTable;
				    float tmpToCover = valueToCover - tmpVal;
					Debug.Log("[TOSAVEONFILE] AI executeCall player : " + playerID + " # " + " valueToCover : " + valueToCover + " # " + " tmpToCover : " + tmpToCover + " # gamePhaseDetail : " + BBGC._BBGlobalDefinitions.gamePhaseDetail.ToString());
					GetComponent<BBGuiInterface>().setPlayerMoneyValue(tmpToCover,playerID);
					yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(tmpToCover,playerID,GetComponent<BBMoveingObjectsController>().PlayersChipRaiseBetPosition[playerID]) );

				}

			    GetComponent<BBGuiInterface>().setPlayerBetType(playerID,"Call");
	   }
	}

#if USE_PHOTON_BB
	[PunRPC]
	void RPCexecuteRaise(int playerID) {
			StartCoroutine(ie_RPCexecuteRaise(playerID));
	}

	IEnumerator ie_RPCexecuteRaise(int playerID) {
			BBGC_M._BBGlobalDefinitions.roundRaiseCounter++;
				GetComponent<BBGuiInterfaceMultiplayer>().TextGameRAISECounter.text = "Round Raise : " + BBGC_M._BBGlobalDefinitions.roundRaiseCounter.ToString();
				          float raiseBetVal = 0;
				          switch(BBGC_M._BBGlobalDefinitions.roundRaiseCounter) {
				             case 1: raiseBetVal = BBGC_M._BBGlobalDefinitions.smallBlindValue * 2;break;
				             case 2: raiseBetVal = BBGC_M._BBGlobalDefinitions.smallBlindValue * 3;break; 
				             case 3: raiseBetVal = BBGC_M._BBGlobalDefinitions.smallBlindValue * 4;break;
				             default: raiseBetVal = BBGC_M._BBGlobalDefinitions.smallBlindValue * 4;break;
				          }

				             BBGC_M._BBGlobalDefinitions.firstBettingRoundValueToPlay += raiseBetVal;
				              if(BBStaticData.debugPreFlopController) Debug.Log("buttonGameRAISE --------->>> raiseBetVal : " + raiseBetVal + " raiseCounter : " + BBGC_M._BBGlobalDefinitions.roundRaiseCounter + 
					          " smallBlindValue : " + BBGC_M._BBGlobalDefinitions.smallBlindValue + " talker : " + BBGC_M._BBGlobalDefinitions.playerToTalk);

				           if(playerID == BBGC_M._BBGlobalDefinitions.smallBlindPlayerId) {
					          raiseBetVal -= BBGC_M._BBGlobalDefinitions.smallBlindValue;
							} 
				           else if(playerID == BBGC_M._BBGlobalDefinitions.bigBlindPlayerId) {
					          raiseBetVal -= BBGC_M._BBGlobalDefinitions.bigBlindValue;
							}

				          GetComponent<BBGuiInterfaceMultiplayer>().setPlayerMoneyValue(raiseBetVal,playerID);

				            yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(raiseBetVal,playerID,GetComponent<BBMoveingObjectsController>().PlayersChipRaiseBetPosition[playerID]) );

				             BBGC_M._BBGlobalDefinitions.lastBet = raiseBetVal;
				             GetComponent<BBGuiInterfaceMultiplayer>().setLastBetValue(raiseBetVal);
				          //if(BBStaticData.debugPreFlopController) Debug.Log("=======================================================================buttonGameRAISE ----1----->>> raiseBetVal : " + raiseBetVal + " lastBet : " + BBGC._BBGlobalDefinitions.lastBet);

				GetComponent<BBGuiInterfaceMultiplayer>().setPlayerBetType(playerID,"Raise");
	}
#endif

	public IEnumerator executeRaise(int playerID) {
			if(isMultiplayer) {
				#if USE_PHOTON_BB
				GetComponent<PhotonView>().RPC("RPCexecuteRaise",PhotonTargets.AllViaServer,playerID);
				#endif
			} else {

				BBGC._BBGlobalDefinitions.roundRaiseCounter++;
				GetComponent<BBGuiInterface>().TextGameRAISECounter.text = "Round Raise : " + BBGC._BBGlobalDefinitions.roundRaiseCounter.ToString();
				          float raiseBetVal = 0;

					          switch(BBGC._BBGlobalDefinitions.roundRaiseCounter) {
					                    case 1: raiseBetVal = BBGC._BBGlobalDefinitions.smallBlindValue * 2;break;
										case 2: raiseBetVal = BBGC._BBGlobalDefinitions.smallBlindValue * 3;break; 
										case 3: raiseBetVal = BBGC._BBGlobalDefinitions.smallBlindValue * 4;break;
										default: raiseBetVal = BBGC._BBGlobalDefinitions.smallBlindValue * 4;break;
					          }


						  BBGC._BBGlobalDefinitions.firstBettingRoundValueToPlay += raiseBetVal;
				          if(BBStaticData.debugPreFlopController) Debug.Log("buttonGameRAISE --------->>> raiseBetVal : " + raiseBetVal + " raiseCounter : " + BBGC._BBGlobalDefinitions.roundRaiseCounter + 
					      " smallBlindValue : " + BBGC._BBGlobalDefinitions.smallBlindValue + " talker : " + BBGC._BBGlobalDefinitions.playerToTalk);

							if(playerID == BBGC._BBGlobalDefinitions.smallBlindPlayerId) {
					          raiseBetVal -= BBGC._BBGlobalDefinitions.smallBlindValue;
							} 
							else if(playerID == BBGC._BBGlobalDefinitions.bigBlindPlayerId) {
					          raiseBetVal -= BBGC._BBGlobalDefinitions.bigBlindValue;
							}

				          GetComponent<BBGuiInterface>().setPlayerMoneyValue(raiseBetVal,playerID);

				            yield return StartCoroutine( GetComponent<BBMoveingObjectsController>().moveChip(raiseBetVal,playerID,GetComponent<BBMoveingObjectsController>().PlayersChipRaiseBetPosition[playerID]) );

				             BBGC._BBGlobalDefinitions.lastBet = raiseBetVal;
				             GetComponent<BBGuiInterface>().setLastBetValue(raiseBetVal);
				          //if(BBStaticData.debugPreFlopController) Debug.Log("=======================================================================buttonGameRAISE ----1----->>> raiseBetVal : " + raiseBetVal + " lastBet : " + BBGC._BBGlobalDefinitions.lastBet);

				GetComponent<BBGuiInterface>().setPlayerBetType(playerID,"Raise");
		  }
	}




}
}