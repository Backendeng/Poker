using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BLabTexasHoldEmProject {

public class BBStaticData : MonoBehaviour {

   // multiplayer
  //public static	int instantiatePlayerCounter = 0;

	public static bool debugGameControllerMultiplayer = true;
    public static bool debugPreFlopController = false;
	public static bool debugFlopController = false;
	public static bool debugTurnController = false;
	public static bool debugRiverController = false;

	public static float waitForPlayerCheckOut = 0.001f;
	public static float waitForPlayerCheck = 1.0f;


 public static int cardsHandProgressive = 1;

 public static int currentActivePlayer = 0;
 public static int currentActivedealer = 0;
 public static int localPlayer = 0;
 public static int smallBlindPlayerId = 1;
 public static int bigBlindPlayerId = 2;

/*
 public static int currentActivePlayer = 7;
 public static int currentActivedealer = 7;
 public static int localPlayer = 0;
 public static int smallBlindPlayerId = 8;
 public static int bigBlindPlayerId = 9;
*/

 public const string phaseMessageInfoWaitGameStarting = "Please Wait Game Starting...";
 public const string phaseMessageInfoMakeYourSmallBlindBet = "Please Make Your Small Blind Bet...";
 public const string phaseMessageInfoMakeYourBigBlindBet = "Please Make Your Big Blind Bet...";
 public const string phaseMessageInfoDealerGiveingCards = "Dealer Giveing Cards...";
 public const string phaseMessageInfoFirstBettingRound = "First Betting Round...";
 public const string phaseMessageInfoLocalPlayerMakeYourDeal = "Make Your Deal...";


 public const float gameLimitedStackValue = 2000;

 public static string getMoneyValue(float val) {
		// return String.Format("{0:0.0}", val + " $");

		return String.Format("{0:C}", val);
 }

  public static bool IsOdd(int value) {
	return value % 2 != 0;
  }

 public static	Vector2 getSequenceForNumer(int toCheck, int[] toCheckList) {

		if(toCheck > 10) return new Vector2(0,0);

		List<int> numberList = new List<int>();
		for(int j = 0;j < toCheckList.Length;j++) numberList.Add(toCheckList[j]);

		int checkForStarting = toCheck;
		int result = 0;
		checkForStarting = 1;
		int counter = 0;
		int maxVal = 0;

		for(int x = 0;x < toCheckList.Length;x++) {

			    result = numberList.Find(item => item == checkForStarting);

			    checkForStarting++;

			    if(checkForStarting == 14) {
			       checkForStarting = 1;
			    }


			    if(result != 0) {
			     counter++;
			     maxVal = result;
			    } else {
			       break;
			    }
			//Debug.Log("--> result : " + result + " checkForStarting : " + checkForStarting + " counter : " + counter);
		}
		return new Vector2(counter,maxVal);
    }

 /// <summary>
 /// Gets the sequence for numer. x = sequence counter y = max Card Value
 /// </summary>
 /// <returns>The sequence for numer.</returns>
 /// <param name="toCheck">To check.</param>
 /// <param name="numberList">Number list.</param>
 public static Vector2 getSequenceForNumer(int toCheck, List<int> numberList) {

		//Debug.Log("============================================1===================getSequenceForNumer===================================--> toCheck : " + toCheck);

		if(toCheck > 10) return new Vector2(0,0);

		int checkForStarting = toCheck;
		int result = 0;
		checkForStarting++;
		int counter = 1;
		int maxVal = 0;



		for(int x = 0;x < numberList.Count;x++) {

			    result = numberList.Find(item => item == checkForStarting);
			 //   Debug.Log("============================================1===================getSequenceForNumer===================================--> result : " + result + " checkForStarting : " + checkForStarting + " counter : " + counter);

			    checkForStarting++;

			    if(checkForStarting == 14) {
			       checkForStarting = 1;
			    }

			    if(result != 0) {
			     counter++;
			     maxVal = result;
			    } else {
			       break;
			    }
			//Debug.Log("-2-> result : " + result + " checkForStarting : " + checkForStarting + " counter : " + counter);
		}
		return new Vector2(counter,maxVal);
    }

	public static Vector2 newGetSequenceForNumer(int toCheck, List<int> numberList) {

		//Debug.Log("============================================1===================getSequenceForNumer===================================--> toCheck : " + toCheck);

		if(toCheck > 9) return new Vector2(0,0);

		int checkForStarting = toCheck;
		int result = 0;
		int counter = 0;
		int maxVal = 0;

		for(int x = 0;x < numberList.Count;x++) {
			    result = numberList.Find(item => item == checkForStarting);
			 //   Debug.Log("============================================1===================getSequenceForNumer===================================--> result : " + result + " checkForStarting : " + checkForStarting + " counter : " + counter);

			    checkForStarting++;

			    if(result != 0) {
			     counter++;
			     maxVal = result;
			    } else {
			       break;
			    }
			//Debug.Log("-2-> result : " + result + " checkForStarting : " + checkForStarting + " counter : " + counter);
		}
		return new Vector2(counter,maxVal);
    }


    private static float cameraDefaultValue = -1;
    private static float cameraZoomedValue = 2.16f;
    private static bool isZoommed = false;

    public static void executeCameraZoom() {

		if(cameraDefaultValue == -1) cameraDefaultValue = Camera.main.orthographicSize;

		if(isZoommed) {
		   Camera.main.orthographicSize = cameraDefaultValue;
		   isZoommed = false;
		} else {
				//if((UnityEngine.iOS.Device.generation.ToString()).IndexOf("iPad") > -1){
				//	Camera.main.orthographicSize = 2.75f;
                //} else {
					Camera.main.orthographicSize = cameraZoomedValue;
                //}
		   isZoommed = true;
		}
  

    }

 public static int getCardsPoint(Vector2 card_1,Vector2 card_2) {

//		Debug.Log("==============================>>> getCardsPoint : " + card_1 + " : " + card_2);

      int tmpValue = 0;

      if(card_1.y == card_2.y) {
          tmpValue += 10;

            if(card_1.y > 9 || card_1.y == 1) tmpValue += 10;

            return tmpValue;
      }

		if( (card_1.y > 5) || (card_2.y > 5) || (card_1.y == 1) || (card_2.y == 1) ) {
		  tmpValue += 5;
		}



      return tmpValue;


	}

	public static void setAllPlayersStateImgRed(List<Image> img, string calling) {

	  for(int x = 0;x < img.Count;x++) {
		 if(img[x] != null) {
			if( (img[x].color == Color.black) || (img[x].color == Color.blue) ) {

			} else {
				Color c = new Color();
				c.a = 0;				
				img[x].color = c;
			}
		 }
	  }

	}

	public static string getRatio(Camera cam) {

		if (Camera.main.aspect >= 1.7)
        {
			return "16:9";
        }
        else if (Camera.main.aspect >= 1.5)
        {
			return "3:2";
        }
        else
        {
			return "4:3";
        }


	}

	public static List<Vector2> getCardsDeckList(string data) {
			//1,3#4,11#1,6#3,12#4,4#1,10#3,3#1,5#2,3#3,8#2,10#2,13#4,7#1,9#4,5#4,8#3,13#1,12#2,7#3,7#3,10#
       List<Vector2> tmpV2List = new List<Vector2>();
       string[] dataOne = data.Split('#');

       for(int x = 0; x < dataOne.Length;x++) {
         if(dataOne[x].Length > 1) {
            string[] splitted = dataOne[x].Split(',');
		    Vector2 v = new Vector2(float.Parse(splitted[0]),float.Parse(splitted[1]));
		    tmpV2List.Add(v);
		 }
       }

       return tmpV2List;
	}

 public static	Texture2D getCardImage(Vector2 cardVal) {
	  Texture2D tmpret = null;
	  //int xVal = (int)cardVal.x;
	  //int yVal = (int)cardVal.y;

//			Debug.Log("**** getCardImage ****** : " + cardVal + " : " + xVal + " : " + yVal);

	   switch((int)cardVal.x) {
	    case 1:
	     switch((int)cardVal.y) {
		        case 1: tmpret = (Texture2D)Resources.Load("1_c");  break;
				case 2: tmpret = (Texture2D)Resources.Load("2_c");  break;
				case 3: tmpret = (Texture2D)Resources.Load("3_c");  break;
				case 4: tmpret = (Texture2D)Resources.Load("4_c");  break;
				case 5: tmpret = (Texture2D)Resources.Load("5_c");  break;
				case 6: tmpret = (Texture2D)Resources.Load("6_c");  break;
				case 7: tmpret = (Texture2D)Resources.Load("7_c");  break;
				case 8: tmpret = (Texture2D)Resources.Load("8_c");  break;
				case 9: tmpret = (Texture2D)Resources.Load("9_c");  break;
				case 10: tmpret = (Texture2D)Resources.Load("10_c");  break;
				case 11: tmpret = (Texture2D)Resources.Load("11_c");  break;
				case 12: tmpret = (Texture2D)Resources.Load("12_c");  break;
				case 13: tmpret = (Texture2D)Resources.Load("13_c");  break;
	     }
	    break;
		case 2:
	     switch((int)cardVal.y) {
		        case 1: tmpret = (Texture2D)Resources.Load("1_f");  break;
				case 2: tmpret = (Texture2D)Resources.Load("2_f");  break;
				case 3: tmpret = (Texture2D)Resources.Load("3_f");  break;
				case 4: tmpret = (Texture2D)Resources.Load("4_f");  break;
				case 5: tmpret = (Texture2D)Resources.Load("5_f");  break;
				case 6: tmpret = (Texture2D)Resources.Load("6_f");  break;
				case 7: tmpret = (Texture2D)Resources.Load("7_f");  break;
				case 8: tmpret = (Texture2D)Resources.Load("8_f");  break;
				case 9: tmpret = (Texture2D)Resources.Load("9_f");  break;
				case 10: tmpret = (Texture2D)Resources.Load("10_f");  break;
				case 11: tmpret = (Texture2D)Resources.Load("11_f");  break;
				case 12: tmpret = (Texture2D)Resources.Load("12_f");  break;
				case 13: tmpret = (Texture2D)Resources.Load("13_f");  break;
	     }
	    break;
		case 3:
	     switch((int)cardVal.y) {
		        case 1: tmpret = (Texture2D)Resources.Load("1_p");  break;
				case 2: tmpret = (Texture2D)Resources.Load("2_p");  break;
				case 3: tmpret = (Texture2D)Resources.Load("3_p");  break;
				case 4: tmpret = (Texture2D)Resources.Load("4_p");  break;
				case 5: tmpret = (Texture2D)Resources.Load("5_p");  break;
				case 6: tmpret = (Texture2D)Resources.Load("6_p");  break;
				case 7: tmpret = (Texture2D)Resources.Load("7_p");  break;
				case 8: tmpret = (Texture2D)Resources.Load("8_p");  break;
				case 9: tmpret = (Texture2D)Resources.Load("9_p");  break;
				case 10: tmpret = (Texture2D)Resources.Load("10_p");  break;
				case 11: tmpret = (Texture2D)Resources.Load("11_p");  break;
				case 12: tmpret = (Texture2D)Resources.Load("12_p");  break;
				case 13: tmpret = (Texture2D)Resources.Load("13_p");  break;
	     }
	    break;
		case 4:
	     switch((int)cardVal.y) {
		        case 1: tmpret = (Texture2D)Resources.Load("1_q");  break;
				case 2: tmpret = (Texture2D)Resources.Load("2_q");  break;
				case 3: tmpret = (Texture2D)Resources.Load("3_q");  break;
				case 4: tmpret = (Texture2D)Resources.Load("4_q");  break;
				case 5: tmpret = (Texture2D)Resources.Load("5_q");  break;
				case 6: tmpret = (Texture2D)Resources.Load("6_q");  break;
				case 7: tmpret = (Texture2D)Resources.Load("7_q");  break;
				case 8: tmpret = (Texture2D)Resources.Load("8_q");  break;
				case 9: tmpret = (Texture2D)Resources.Load("9_q");  break;
				case 10: tmpret = (Texture2D)Resources.Load("10_q");  break;
				case 11: tmpret = (Texture2D)Resources.Load("11_q");  break;
				case 12: tmpret = (Texture2D)Resources.Load("12_q");  break;
				case 13: tmpret = (Texture2D)Resources.Load("13_q");  break;
	     }
	    break;

	   }


	  return tmpret;
	}


}
}