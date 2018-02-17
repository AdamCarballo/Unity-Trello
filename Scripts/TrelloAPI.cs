/*
 * TrelloAPI.cs
 * Interact directly with the Trello API using MiniJSON and uploads cards. 
 * 
 * Original by bfollington
 * https://github.com/bfollington/Trello-Cards-Unity
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
 */

using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

namespace Trello {
	public class TrelloAPI {
		
		private string _token;
		private string _key;
		private List<object> _boards;
		private List<object> _lists;
		private const string _memberBaseUrl = "https://api.trello.com/1/members/me";
		private const string _boardBaseUrl = "https://api.trello.com/1/boards/";
		private const string _cardBaseUrl = "https://api.trello.com/1/cards/";
		private string _currentBoardId = null;
		private string _currentListId = null;


		/// <summary>
		/// Generate new Trello API instance.
		/// </summary>
		/// <param name="key">Trello API key, keep it private.</param>
		/// <param name="token">Trello API token, keep it private.</param>
		public TrelloAPI(string key, string token) {

			_key = key;
			_token = token;
		}
		
		/// <summary>
		/// Checks if a WWW object returned an error, and if so throws an exception.
		/// </summary>
		/// <param name="errorMessage">Error message to display.</param>
		/// <param name="www">The WWW request object.</param>
		private void CheckWwwStatus(string errorMessage, WWW www) {

			if (!string.IsNullOrEmpty(www.error)) {
				throw new TrelloException(errorMessage + ": " + www.error);
			}
		}

		/// <summary>
		/// Download the list of available boards for the user and store them.
		/// </summary>
		/// <returns>Downloaded boards.</returns>
		public List<object> PopulateBoards() {

			_boards = null;
			WWW www = new WWW(string.Format("{0}?key={1}&token={2}&boards=all", _memberBaseUrl, _key, _token));
			
			// Wait for request to return
			while (!www.isDone) {
				CheckWwwStatus("The Trello servers did not respond.", www);
			}

			var dict = Json.Deserialize(www.text) as Dictionary<string,object>;

			_boards = (List<object>)dict["boards"];
			return _boards;
		}
		
		/// <summary>
		/// Sets the given board to search for lists in.
		/// </summary>
		/// <param name="name">Name of the board.</param>
		public void SetCurrentBoard(string name) {

			if (_boards == null)	{
				throw new TrelloException("There are no boards available. Either the user does not have access to a board or PopulateBoards() wasn't called.");
			}
			
			for (int i = 0; i < _boards.Count; i++) {
				var board = (Dictionary<string, object>)_boards[i];
				if ((string)board["name"] == name) {
					_currentBoardId = (string)board["id"];
					return;
				}
			}
			
			_currentBoardId = null;
			throw new TrelloException("A board with the name " + name + " was not found.");
		}

		/// <summary>
		/// Download all the lists of the selected board and store them.
		/// </summary>
		/// <returns>Downloaded list.</returns>
		public List<object> PopulateLists() {

			_lists = null;
			
			if (_currentBoardId == null) {
				throw new TrelloException("Cannot retreive the lists, there isn't a selected board yet.");
			}

			WWW www = new WWW(string.Format("{0}{1}?key={2}&token={3}&lists=all", _boardBaseUrl, _currentBoardId, _key, _token));
	
			// Wait for request to return
			while (!www.isDone)	{
				CheckWwwStatus("Connection to the Trello servers was not possible", www);
			}
			
			var dict = Json.Deserialize(www.text) as Dictionary<string,object>;

			_lists = (List<object>)dict["lists"];
			return _lists;
		}

		/// <summary>
		/// Sets the given list to upload cards to.
		/// </summary>
		/// <param name="name">Name of the list.</param>
		public void SetCurrentList(string name) {

			if (_lists == null) {
				throw new TrelloException("There are no lists available. Either the board does not contain lists or PopulateLists() wasn't called.");
			}

			for (int i = 0; i < _lists.Count; i++) {
				var list = (Dictionary<string, object>)_lists[i];
				if ((string)list["name"] == name) {
					_currentListId = (string)list["id"];
					return;
				}
			}
			
			_currentListId = null;
			throw new TrelloException("A list with the name " + name + " was not found.");
		}
		
		/// <summary>
		/// Returns the selected Trello list id.
		/// </summary>
		/// <returns>The list id.</returns>
		public string GetCurrentListId() {

			if (_currentListId == null) {
				throw new TrelloException("A list has not been selected. Call SetCurrentList() first.");
			}
			return _currentListId;
		}

		/// <summary>
		/// Given an exception object, a TrelloCard is created and populated with the relevant information from the exception. This is then uploaded to the Trello server.
		/// </summary>
		/// <returns>The exception card.</returns>
		/// <param name="e">E.</param>
		/*public TrelloCard uploadExceptionCardd(Exception e) {

			TrelloCard card = new TrelloCard();
			card.name = e.GetType().ToString();
			card.due = DateTime.Now.ToString();
			card.desc = e.Message;
			card.idList = _currentListId;
			
			return UploadCard(card);
		}*/

		/// <summary>
		/// Uploads a given TrelloCard object to the Trello server.
		/// </summary>
		/// <returns>Trello card uploaded.</returns>
		/// <param name="card">Trello card to upload.</param>
		public TrelloCard UploadCard(TrelloCard card) {

			WWWForm post = new WWWForm();
			post.AddField("name", card.name);
			post.AddField("desc", card.desc);
			post.AddField("due", card.due);
			post.AddField("idList", card.idList);
			post.AddField("urlSource", card.urlSource);
			if (card.fileSource != null && card.fileName != null) {
				post.AddBinaryData("fileSource", card.fileSource, card.fileName);
			}

			WWW www = new WWW(string.Format("{0}?key={1}&token={2}", _cardBaseUrl, _key, _token), post);
			
			// Wait for request to return
			while (!www.isDone) {
				CheckWwwStatus("Could not upload the Trello card.", www);
			}

			Debug.Log("Trello card sent!");	
			return card;
		}
	}
}