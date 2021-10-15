/*
 * TrelloAPI.cs
 * Interact directly with the Trello API using MiniJSON and uploads cards. 
 * 
 * Original by bfollington
 * https://github.com/bfollington/Trello-Cards-Unity
 * 
 * by Àdam Carballo under MIT license.
 * https://github.com/AdamCarballo/Unity-Trello
 */

using System.Collections.Generic;
using UnityEngine;
using MiniJSON;
using UnityEngine.Networking;

namespace Trello {
	public class TrelloAPI {
		
		private const string MemberBaseUrl = "https://api.trello.com/1/members/me";
		private const string BoardBaseUrl = "https://api.trello.com/1/boards/";
		private const string CardBaseUrl = "https://api.trello.com/1/cards/";
		
		private readonly string _token;
		private readonly string _key;
		
		private List<object> _boards;
		private List<object> _lists;
		private string _currentBoardId;
		private string _currentListId;


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
		/// Checks if a UnityWebRequest object returned an error, and if so throws an exception.
		/// </summary>
		/// <param name="errorMessage">Error message to display.</param>
		/// <param name="uwr">The UnityWebRequest object.</param>
		private void CheckWebRequestStatus(string errorMessage, UnityWebRequest uwr) {
			switch (uwr.result) {
				case UnityWebRequest.Result.ConnectionError:
				case UnityWebRequest.Result.ProtocolError:
				case UnityWebRequest.Result.DataProcessingError:
					throw new TrelloException($"{errorMessage}: {uwr.error} ({uwr.downloadHandler.text})");
			}
		}

		/// <summary>
		/// Download the list of available boards for the user and store them.
		/// </summary>
		/// <returns>Downloaded boards.</returns>
		public List<object> PopulateBoards() {
			_boards = null;
			var uwr = UnityWebRequest.Get($"{MemberBaseUrl}?key={_key}&token={_token}&boards=all");
			var operation = uwr.SendWebRequest();

			// Wait for request to return
			while (!operation.isDone) {
				CheckWebRequestStatus("The Trello servers did not respond.", uwr);
			}

			if (!(Json.Deserialize(uwr.downloadHandler.text) is Dictionary<string, object> boardsDict)) {
				throw new TrelloException("No boards found or something went wrong.");
			}
			
			_boards = (List<object>) boardsDict["boards"];
			return _boards;
		}
		
		/// <summary>
		/// Sets the given board to search for lists in.
		/// </summary>
		/// <param name="name">Name of the board.</param>
		public void SetCurrentBoard(string name) {

			if (_boards == null) {
				throw new TrelloException("There are no boards available. Either the user does not have access to a board or PopulateBoards() wasn't called.");
			}
			
			for (var i = 0; i < _boards.Count; i++) {
				var board = (Dictionary<string, object>) _boards[i];
				if ((string)board["name"] != name) continue;
				
				_currentBoardId = (string) board["id"];
				return;
			}
			
			_currentBoardId = null;
			throw new TrelloException($"A board with the name {name} was not found.");
		}

		/// <summary>
		/// Download all the lists of the selected board and store them.
		/// </summary>
		/// <returns>Downloaded list.</returns>
		public List<object> PopulateLists() {

			_lists = null;
			
			if (_currentBoardId == null) {
				throw new TrelloException("Cannot retrieve the lists, there isn't a selected board yet.");
			}

			var uwr = UnityWebRequest.Get($"{BoardBaseUrl}{_currentBoardId}?key={_key}&token={_token}&lists=all");
			var operation = uwr.SendWebRequest();

			// Wait for request to return
			while (!operation.isDone) {
				CheckWebRequestStatus("Connection to the Trello servers was not possible.", uwr);
			}

			if (!(Json.Deserialize(uwr.downloadHandler.text) is Dictionary<string, object> listsDict)) {
				throw new TrelloException("No lists found or something went wrong.");
			}

			_lists = (List<object>) listsDict["lists"];
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

			for (var i = 0; i < _lists.Count; i++) {
				var list = (Dictionary<string, object>) _lists[i];
				if ((string)list["name"] != name) continue;
				
				_currentListId = (string) list["id"];
				return;
			}
			
			_currentListId = null;
			throw new TrelloException($"A list with the name {name} was not found.");
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

		/*/// <summary>
		/// Given an exception object, a TrelloCard is created and populated with the relevant information from the exception. This is then uploaded to the Trello server.
		/// </summary>
		/// <returns>The exception card.</returns>
		/// <param name="e">E.</param>
		public TrelloCard uploadExceptionCardd(Exception e) {

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

			var post = new WWWForm();
			post.AddField("name", card.name);
			post.AddField("desc", card.desc);
			post.AddField("due", card.due);
			post.AddField("idList", card.idList);
			post.AddField("urlSource", card.urlSource);
			if (card.fileSource != null && card.fileName != null) {
				post.AddBinaryData("fileSource", card.fileSource, card.fileName);
			}

			var uwr = UnityWebRequest.Post($"{CardBaseUrl}?key={_key}&token={_token}", post);
			var operation = uwr.SendWebRequest();

			// Wait for request to return
			while (!operation.isDone) {
				CheckWebRequestStatus("Could not upload the Trello card.", uwr);
			}

			Debug.Log($"Trello card sent!\nResponse {uwr.responseCode}");
			return card;
		}
	}
}