/*
 * TrelloException.cs
 * Custom Exception for Trello scripts.
 * 
 * Original by bfollington
 * https://github.com/bfollington/Trello-Cards-Unity
 * 
 * by Adam Carballo under GPLv3 license.
 * https://github.com/AdamEC/Unity-Trello
 */

using System;

namespace Trello {
	public class TrelloException : Exception {

        /// <summary>
        /// Throw an Exception.
        /// </summary>
        public TrelloException()
            : base() {
        }

        /// <summary>
        /// Throw an Exception.
        /// </summary>
        public TrelloException(string message)
            : base(message) {		
		}

        /// <summary>
        /// Throw an Exception.
        /// </summary>
        public TrelloException(string format, params object[] args)
            : base(string.Format(format, args)) {		
		}

        /// <summary>
        /// Throw an Exception.
        /// </summary>
        public TrelloException(string message, Exception innerException)
            : base(message, innerException) {		
		}

        /// <summary>
        /// Throw an Exception.
        /// </summary>
        public TrelloException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) {	
		}
	}
}