using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if USE_PHOTON
using Photon;
#endif

namespace BLabTexasHoldEmProject {

#if USE_PHOTON
public class BBMoveingObjectsController : Photon.MonoBehaviour {
#else
public class BBMoveingObjectsController : MonoBehaviour {
#endif
    public Transform[] playersChipStartingPoint;
	public Transform[] playersChipEndingPoint;
	public Transform[] PlayersChipRaiseBetPosition;

	public Transform removeChipAndCardsPosition;

	public struct MultiObjectMove {
	  public Transform toMove;
	  public Transform destination;
	  public Vector3 toMoveV3;
	  public Vector3 destinationV3;
	};

	public List <MultiObjectMove> multiObjectMoveList = new List<MultiObjectMove>();
	[HideInInspector]
	public bool moveMultiObjectMoveList = false;

    bool isMultiplayer = false;

    void Awake() {
			isMultiplayer = SceneManager.GetActiveScene().name.Contains("Multiplayer");
    }

	// Use this for initialization
	void Start () {
	
	}

	public IEnumerator cleanTable() {
	       lastChipH = 0;
	       lastChipX = 0;
			yield return StartCoroutine(removeAllChips());
			yield return new WaitForSeconds(1);
			yield return StartCoroutine(removeAllCards());

			  GameObject allInImage = GameObject.Find("ImageGameAllIn");
		      if(allInImage) {
		       Image img = allInImage.GetComponent<Image>();
		        Color c = img.color;
		        c.a = 0.2f;
		        img.color = c;
	          }

	}

	public IEnumerator removeAllCards() {
	  GameObject[] allChip = GameObject.FindGameObjectsWithTag("cardToRemove");
	   multiObjectMoveList.Clear();

        for(int x = 0;x < allChip.Length;x++) {
			MultiObjectMove mom = new MultiObjectMove();
			mom.toMove = allChip[x].transform;
			mom.destination = removeChipAndCardsPosition;
			multiObjectMoveList.Add(mom);
			Destroy(allChip[x],6);
        }

			GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().cleanCards);

			moveMultiObjectMoveList = true;

			yield return new WaitUntil(() => moveMultiObjectMoveList == false);

	}


	public IEnumerator removeAllChips() {
	   GameObject[] allChip = GameObject.FindGameObjectsWithTag("chip");
	   multiObjectMoveList.Clear();

        for(int x = 0;x < allChip.Length;x++) {
			MultiObjectMove mom = new MultiObjectMove();
			mom.toMove = allChip[x].transform;
			mom.destination = removeChipAndCardsPosition;
			multiObjectMoveList.Add(mom);
			Destroy(allChip[x],6);
        }

			moveMultiObjectMoveList = true;

			GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().cleanChips);

			yield return new WaitUntil(() => moveMultiObjectMoveList == false);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(moveMultiObjectMoveList) {
			  
				float step = 5 * Time.deltaTime;
			   int notStillMove = multiObjectMoveList.Count;
			    for(int x = 0; x < multiObjectMoveList.Count;x++) {

				  if(multiObjectMoveList[x].destination == null) {
					//Vector3 _from = Vector3.zero;
			        Vector3 _to = Vector3.zero;
					//_from = multiObjectMoveList[x].toMoveV3;
					_to = multiObjectMoveList[x].destinationV3;
					multiObjectMoveList[x].toMove.position = Vector3.MoveTowards(multiObjectMoveList[x].toMove.position,  _to, step);
					Debug.Log("moveMultiObjectMoveList-------moveing-------->>" + multiObjectMoveList[x].toMove.name + " : " + _to + " : " + multiObjectMoveList[x].toMove.position);

					float dist = Vector3.Distance(multiObjectMoveList[x].toMove.position, _to);
				        if(dist < 0.01f) { 
						  notStillMove--;
					    }
				  } else {
					  multiObjectMoveList[x].toMove.position = Vector3.MoveTowards(multiObjectMoveList[x].toMove.position,  multiObjectMoveList[x].destination.position, step);
						float dist = Vector3.Distance(multiObjectMoveList[x].toMove.position, multiObjectMoveList[x].destination.position);
				        if(dist < 0.01f) { 
						  notStillMove--;
					    }
				  }


				}
			if(notStillMove < 1) {
			       if(isMultiplayer) {
						#if USE_PHOTON
						// if(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.SmallBlind) {
						// }
						// else if(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.BigBlind) {
						// }
						// else if(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.GiveingCards) {
						// }
						#endif
			       } else {
						if(GetComponent<BBGameController>()._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.SmallBlind) {
							GetComponent<BBGameController>().setNextPlayerActive();
						}
						else if(GetComponent<BBGameController>()._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.BigBlind) {
							GetComponent<BBGameController>().dealerStartGiveCards();
						}
						else if(GetComponent<BBGameController>()._BBGlobalDefinitions.gamePhaseDetail == BBGlobalDefinitions.GamePhaseDetail.GiveingCards) {
						}
				  }
				moveMultiObjectMoveList = false;
				multiObjectMoveList.Clear();
			}
		}

	}

	public IEnumerator moveChipFromTo(float val, Vector3 _from, Vector3 _to) {

			Debug.Log("****************** moveChipFromTo **************** val : " + val + " _from : " + _from + " _to : " + _to);

		if(val < 1) yield break;

		GameObject chipRED = Resources.Load("playerChipRed") as GameObject; // 5
		GameObject chipGREEN = Resources.Load("playerChipGreen") as GameObject; // 25
		GameObject chipBLUE = Resources.Load("playerChipBlue") as GameObject; // 50
		GameObject chipBLACK = Resources.Load("playerChipBlack") as GameObject; // 100

		int splitted = 0;
		if(isMultiplayer) {
				#if USE_PHOTON
				// splitted = (int)val / (int)GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.limitedLow;
				#endif
		} else {
			splitted = (int)val / (int)GetComponent<BBGameController>()._BBGlobalDefinitions.limitedLow;
		}


		if(splitted == 1) {
				if(isMultiplayer) {
					#if USE_PHOTON
					// yield return StartCoroutine( createAndMoveChipFromTo(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.smallBlindValue,chipGREEN,_from,_to) );
					#endif
				} else {
					yield return StartCoroutine( createAndMoveChipFromTo(GetComponent<BBGameController>()._BBGlobalDefinitions.smallBlindValue,chipGREEN,_from,_to) );
				}

		} else if(splitted == 2) {
				if(isMultiplayer) {
					#if USE_PHOTON
					// yield return StartCoroutine( createAndMoveChipFromTo(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.bigBlindValue,chipBLUE,_from,_to) );
					#endif
				} else {
					yield return StartCoroutine( createAndMoveChipFromTo(GetComponent<BBGameController>()._BBGlobalDefinitions.bigBlindValue,chipBLUE,_from,_to) );
				}
		} else if(splitted == 3) {
				if(isMultiplayer) {
					#if USE_PHOTON
					// yield return StartCoroutine( createAndMoveChipFromTo(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.bigBlindValue,chipBLUE,_from,_to) );
					// yield return StartCoroutine( createAndMoveChipFromTo(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.smallBlindValue,chipGREEN,_from,_to) );
					#endif
				} else {
					yield return StartCoroutine( createAndMoveChipFromTo(GetComponent<BBGameController>()._BBGlobalDefinitions.bigBlindValue,chipBLUE,_from,_to) );
					yield return StartCoroutine( createAndMoveChipFromTo(GetComponent<BBGameController>()._BBGlobalDefinitions.smallBlindValue,chipGREEN,_from,_to) );
				}
		} else if(splitted == 4) {
				yield return StartCoroutine( createAndMoveChipFromTo(100,chipBLACK,_from,_to) );
		} else {
		    float tmpVal = val;

		    if(val >= 500) {
RepFor500:
				 tmpVal -= 500;
					yield return StartCoroutine( createAndMoveChipFromTo(500,chipRED,_from,_to) );
				 if(tmpVal >= 500) {
					goto RepFor500; 
				 } else {
                    if(tmpVal >= 100) {
RepFor100:
						tmpVal -= 100;
							yield return StartCoroutine( createAndMoveChipFromTo(100,chipBLACK,_from,_to) );
						if(tmpVal >= 100) {
							goto RepFor100;
						} else {
							 if(tmpVal >= 50) {
RepFor50:					
								  tmpVal -= 50;
									yield return StartCoroutine( createAndMoveChipFromTo(50,chipBLACK,_from,_to) );
                                  if(tmpVal >= 50) {
                                   goto RepFor50;
                                  } else {
										yield return StartCoroutine( createAndMoveChipFromTo(25,chipGREEN,_from,_to) );
                                  }
                             }
						}
                    }
				 }
		       
		    }
		    else if(val >= 100) { 
RepFor100_2:
				tmpVal -= 100;
					yield return StartCoroutine( createAndMoveChipFromTo(100,chipBLACK,_from,_to) );
				 if(tmpVal >= 100) {
					goto RepFor100_2; 
				 } else {
RepFor50_2:
				       if(tmpVal >= 50) {
				         tmpVal -= 50;
							yield return StartCoroutine( createAndMoveChipFromTo(50,chipBLUE,_from,_to) );
						   if(tmpVal >= 50) {
							goto RepFor50_2;
						   } else {
						     if(tmpVal >= 25)
									yield return StartCoroutine( createAndMoveChipFromTo(25,chipGREEN,_from,_to) );
						   }
				       } else {
						 if(tmpVal >= 25)
								yield return StartCoroutine( createAndMoveChipFromTo(25,chipGREEN,_from,_to) );
				       }
				 }
			}
           else {
		   }

		}
      
	}


	public IEnumerator moveChip(float val, int playerID, Transform position) {

		//Debug.Log("****************** moveChip **************** player : " + playerID + " isMulti : " + isMultiplayer + " val : " + val);

		if(val < 1) yield break;

        if(isMultiplayer) {
        } else {
			if( GetComponent<BBGameController>()._BBGlobalDefinitions.playersStateIsOutDuringOpenGame[playerID] ) {
			 yield break;
			}
	    }

		if(isMultiplayer) {
		} else {
			if( GetComponent<BBGameController>().playerDataList[playerID].isOutOfGame )
			{
			  yield break;
			}
		}

		if(isMultiplayer) {
		} else {
	      if( !GetComponent<BBMoneyController>().checkForBetPossibility(playerID,val) ) {
			 GetComponent<BBMoneyController>().setPlayerOutForNoMoreMoney(playerID,val);
			 yield break;
	      }
	    }

		if(position == null) {
			position = playersChipEndingPoint[playerID];
		}

		GameObject chipRED = Resources.Load("playerChipRed") as GameObject; // 5
		GameObject chipGREEN = Resources.Load("playerChipGreen") as GameObject; // 25
		GameObject chipBLUE = Resources.Load("playerChipBlue") as GameObject; // 50
		GameObject chipBLACK = Resources.Load("playerChipBlack") as GameObject; // 100

		int splitted = 0;
		if(isMultiplayer) {
				#if USE_PHOTON
				// splitted = (int)val / (int)GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.limitedLow;
				#endif
		} else {
			splitted = (int)val / (int)GetComponent<BBGameController>()._BBGlobalDefinitions.limitedLow;
		}


		if(splitted == 1) {
				if(isMultiplayer) {
					#if USE_PHOTON
					// yield return StartCoroutine( createAndMoveChip(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.smallBlindValue,chipGREEN,playerID,position) );
					#endif
				} else {
					yield return StartCoroutine( createAndMoveChip(GetComponent<BBGameController>()._BBGlobalDefinitions.smallBlindValue,chipGREEN,playerID,position) );
				}

		} else if(splitted == 2) {
				if(isMultiplayer) {
					#if USE_PHOTON
					// yield return StartCoroutine( createAndMoveChip(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.bigBlindValue,chipBLUE,playerID,position) );
					#endif
				} else {
					yield return StartCoroutine( createAndMoveChip(GetComponent<BBGameController>()._BBGlobalDefinitions.bigBlindValue,chipBLUE,playerID,position) );
				}
		} else if(splitted == 3) {
				if(isMultiplayer) {
					#if USE_PHOTON
					// yield return StartCoroutine( createAndMoveChip(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.bigBlindValue,chipBLUE,playerID,position) );
					// yield return StartCoroutine( createAndMoveChip(GetComponent<NewGameControllerMultiplayer>()._BBGlobalDefinitions.smallBlindValue,chipGREEN,playerID,position) );
					#endif
				} else {
					yield return StartCoroutine( createAndMoveChip(GetComponent<BBGameController>()._BBGlobalDefinitions.bigBlindValue,chipBLUE,playerID,position) );
			        yield return StartCoroutine( createAndMoveChip(GetComponent<BBGameController>()._BBGlobalDefinitions.smallBlindValue,chipGREEN,playerID,position) );
				}
		} else if(splitted == 4) {
			yield return StartCoroutine( createAndMoveChip(100,chipBLACK,playerID,position) );
		} else {
		    float tmpVal = val;

		    if(val >= 500) {
RepFor500:
				 tmpVal -= 500;
				 yield return StartCoroutine( createAndMoveChip(500,chipRED,playerID,position) );
				 if(tmpVal >= 500) {
					goto RepFor500; 
				 } else {
                    if(tmpVal >= 100) {
RepFor100:
						tmpVal -= 100;
						yield return StartCoroutine( createAndMoveChip(100,chipBLACK,playerID,position) );
						if(tmpVal >= 100) {
							goto RepFor100;
						} else {
							 if(tmpVal >= 50) {
RepFor50:					
								  tmpVal -= 50;
						          yield return StartCoroutine( createAndMoveChip(50,chipBLACK,playerID,position) );
                                  if(tmpVal >= 50) {
                                   goto RepFor50;
                                  } else {
									yield return StartCoroutine( createAndMoveChip(25,chipGREEN,playerID,position) );
                                  }
                             }
						}
                    }
				 }
		       
		    }
		    else if(val >= 100) { 
RepFor100_2:
				tmpVal -= 100;
				 yield return StartCoroutine( createAndMoveChip(100,chipBLACK,playerID,position) );
				 if(tmpVal >= 100) {
					goto RepFor100_2; 
				 } else {
RepFor50_2:
				       if(tmpVal >= 50) {
				         tmpVal -= 50;
						 yield return StartCoroutine( createAndMoveChip(50,chipBLUE,playerID,position) );
						   if(tmpVal >= 50) {
							goto RepFor50_2;
						   } else {
						     if(tmpVal >= 25)
							   yield return StartCoroutine( createAndMoveChip(25,chipGREEN,playerID,position) );
						   }
				       } else {
						 if(tmpVal >= 25)
						   yield return StartCoroutine( createAndMoveChip(25,chipGREEN,playerID,position) );
				       }
				 }
			}
           else {
		   }

		}
      
	}

	float lastChipH = 0;
	float lastChipX = 0;
	public IEnumerator createAndMoveChip(float chipVal,GameObject chip, int playerID, Transform position) {

		GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().chipHandle);

           GameObject newChip = Instantiate(chip,playersChipStartingPoint[playerID].position,Quaternion.identity) as GameObject;
		   chip.GetComponent<BBChipData>().chipOnwerPosition = playerID;
		   chip.GetComponent<BBChipData>().chipValue = chipVal;

				MultiObjectMove mc = new MultiObjectMove();
				mc.toMove = newChip.transform;
				Transform tmpT = position;
				if(lastChipH == 0) lastChipH = tmpT.position.y;else lastChipH += 0.01f;
			    if(lastChipX == 0) lastChipX = tmpT.position.x;else lastChipX = 0.02f;

			    tmpT.position = new Vector3(tmpT.position.x /* + lastChipX UnityEngine.Random.Range(0.03f,0.04f)*/,lastChipH,tmpT.position.z);
				mc.destination = tmpT; 
				multiObjectMoveList.Add(mc);

				moveMultiObjectMoveList = true;

				yield return new WaitUntil(() => moveMultiObjectMoveList == false);

		        yield return new WaitForEndOfFrame();

	}

	public IEnumerator createAndMoveChipFromTo(float chipVal,GameObject chip, Vector3 fromV3, Vector3 toV3) {

		Debug.Log("================== createAndMoveChipFromTo =================");

		GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().chipHandle);

		   GameObject newChip = Instantiate(chip,fromV3,Quaternion.identity) as GameObject;
		   chip.GetComponent<BBChipData>().chipOnwerPosition = 0;
		   chip.GetComponent<BBChipData>().chipValue = chipVal;

				MultiObjectMove mc = new MultiObjectMove();
				mc.toMove = newChip.transform;
				mc.toMoveV3 = fromV3;
				Transform tmpT = null;
				if(lastChipH == 0) lastChipH = tmpT.position.y;else lastChipH += 0.01f;
			    if(lastChipX == 0) lastChipX = tmpT.position.x;else lastChipX = 0.02f;

			  //  tmpT.position = new Vector3(tmpT.position.x /* + lastChipX UnityEngine.Random.Range(0.03f,0.04f)*/,lastChipH,tmpT.position.z);
				mc.destination = null;
				mc.destinationV3 = toV3; 
				multiObjectMoveList.Add(mc);

				moveMultiObjectMoveList = true;

				yield return new WaitUntil(() => moveMultiObjectMoveList == false);

		        yield return new WaitForEndOfFrame();

	}


	public IEnumerator moveCard(GameObject card, Transform dest) {
		
		//Debug.Log("moveCard-------moveCard-------->> : " + card.name + " dest : " + dest.gameObject.name);

		MultiObjectMove mc = new MultiObjectMove();
		mc.toMove = card.transform;
		mc.destination = dest;
		multiObjectMoveList.Add(mc);

		moveMultiObjectMoveList = true;
		GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().moveCard);
		yield return new WaitUntil(() => moveMultiObjectMoveList == false);
	}

	public IEnumerator moveCard(GameObject card, Transform dest,int playerID) {
		
		//Debug.Log("moveCard-------moveCard-------->> : " + card.name + " dest : " + dest.gameObject.name);

		MultiObjectMove mc = new MultiObjectMove();
		mc.toMove = card.transform;
		mc.destination = dest;
		card.GetComponent<BBCard>().bettingPosID = playerID;
		multiObjectMoveList.Add(mc);

		moveMultiObjectMoveList = true;
		GetComponent<BBSoundController>().playClip(GetComponent<BBSoundController>().moveCard);
		yield return new WaitUntil(() => moveMultiObjectMoveList == false);
	}

}
}