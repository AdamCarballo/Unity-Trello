/*
 * TrelloSend.cs
 * Script that holds keys and allows to send Trello cards.
 *  
 * by Àdam Carballo under MIT license.
 * https://github.com/AdamCarballo/Unity-Trello
 */

using System.Collections;
using UnityEngine;

namespace Trello {
    public class TrelloSend : MonoBehaviour {

        [Header("Trello Auth")]
        [SerializeField]
        private string _key;
        [SerializeField]
        private string _token;

        [Header("Trello Settings")]
        [SerializeField]
        private string _defaultBoard;
        [SerializeField]
        private string _defaultList;

        public string Key {
            set => _key = value;
        }

        public string Token {
            set => _token = value;
        }
        
        private void Start() {
            if (string.IsNullOrEmpty(_key) || string.IsNullOrEmpty(_token)) {
                throw new TrelloException("The Trello API key or token are missing!");
            }
        }

        /// <summary>
        /// Sends a given Trello card using the authorization settings.
        /// </summary>
        /// <param name="card">Trello card to send.</param>
        /// <param name="list">Overrides default list.</param>
        /// <param name="board">Overrides default board.</param>
        public void SendNewCard(TrelloCard card, string list = null, string board = null) {

            if (board == null) {
                board = _defaultBoard;
            }
            if (list == null) {
                list = _defaultList;
            }

            StartCoroutine(Send_Internal(card, list, board));
        }

        private IEnumerator Send_Internal(TrelloCard card, string list, string board) {

            // Create an API instance
            var api = new TrelloAPI(_key, _token);

            // Wait for the Trello boards
            yield return api.PopulateBoards();
            api.SetCurrentBoard(board);

            // Wait for the Trello lists
            yield return api.PopulateLists();
            api.SetCurrentList(list);

            // Set the current ID of the selected list
            card.idList = api.GetCurrentListId();

            // Upload to the server
            yield return api.UploadCard(card);
        }
    }
}