/*
 * TrelloCard.cs
 * Base class for a Trello card.
 * 
 * Original by bfollington
 * https://github.com/bfollington/Trello-Cards-Unity
 * 
 * by Àdam Carballo under MIT license.
 * https://github.com/AdamCarballo/Unity-Trello
 */

namespace Trello {
    public class TrelloCard {
	
        public string name = "";
        public string desc = "";
        public string due = "null";
        public string idList = "";
        public string urlSource = "null";
        public byte[] fileSource = null;
        public string fileName = null;

        /// <summary>
        /// Base class for a Trello card.
        /// </summary>
        public TrelloCard() {
			
        }		
    }
}