using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BLabTexasHoldEmProject {

[System.Serializable]
public class BBPlayerData {

 public GameObject playerGameObject;

 public BBGlobalDefinitions.GamePhaseDetail gamePhaseDetail = BBGlobalDefinitions.GamePhaseDetail.None;

 public string playerName;
 public int playerPosition;
 public bool isLocalPlayer = false;
 #if USE_PHOTON
//  public  NewResultEngine.CompleteResultStruct completeResultStruct;
 #endif
 public bool isOutOfGame = false;
 public bool runOutOfMoney = false;
 public bool underAllin = false;
 public bool isObserver = false;


 public Image playerAvatarImage;
 [HideInInspector]
 public string playerAvatarImageIdx = "0";
 public string playerCountryCode;
 public Image playerActiveImage;
 public Image AllInImage;


 public Text T_playerName;
 public Text T_PlayerMoneyOnTable;
 public Text T_PlayerMoneyTotal;
 public Text T_TextPlayerBetType;



 public float currentMoneyOnTable = 0;
 public float currentPlayerTotalMoney = 0;

 public GameObject PlayerDealerImage;

 public Vector2 card_1_Value;
 public Vector2 card_2_Value;

 public Transform transform_card_1;
 public Transform transform_card_2;

 public int coeffCardsValOnFlopPhase = 0;
 public int maxCardValOnFlopPhase = 0;
 //public int maxCardValueOnShowDown = 0;

	    Vector4 _TwoPairData = Vector4.zero;
		public Vector4 TwoPairData {
		  get { return this._TwoPairData;}
		  set {this._TwoPairData = value;}
		}

		int[] _PairData = new int[5];
	      public int[] PairData {
		   get { return this._PairData;}
		   set {this._PairData = value;}
		}

	    int[] _HighCardData = new int[5];
	     public int[] HighCardData {
		   get { return this._HighCardData;}
		   set {this._HighCardData = value;}
		}

	    int[] _ThreeOfAkindCardData = new int[3];
	       public int[] ThreeOfAkindCardData {
		    get { return this._ThreeOfAkindCardData;}
		    set {this._ThreeOfAkindCardData = value;}
		}

	    public int _FlushMaxCardDataValue = 0;
	        public int FlushMaxCardDataValue {
		     get { return this._FlushMaxCardDataValue;}
		     set {this._FlushMaxCardDataValue = value;}
		}

	    public int _FlushMaxCardDataValueKicker = 0;
	       public int FlushMaxCardDataValueKicker {
		     get { return this._FlushMaxCardDataValueKicker;}
		     set {this._FlushMaxCardDataValueKicker = value;}
		}

	    public int _FlushCardDataSeed = 0;
	         public int FlushCardDataSeed {
	 	       get { return this._FlushCardDataSeed;}
		       set {this._FlushCardDataSeed = value;}
		}

	    int[] _FullHouseCardData = new int[2];
	       public int[] FullHouseCardData {
		    get { return this._FullHouseCardData;}
		    set {this._FullHouseCardData = value;}
		}
	    int _PokerCardDataValue = 0;
	        public int PokerCardDataValue {
		      get { return this._PokerCardDataValue;}
		      set {this._PokerCardDataValue = value;}
		}
	    int _StraightFlushMaxCardDataValue = 0;
	       public int StraightFlushMaxCardDataValue {
		    get { return this._StraightFlushMaxCardDataValue;}
		    set {this._StraightFlushMaxCardDataValue = value;}
		}



}
}