/*
 * TrelloCard.cs
 * Base class for a Trello card.
 * 
 * Original by bfollington
 * https://github.com/bfollington/Trello-Cards-Unity
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
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