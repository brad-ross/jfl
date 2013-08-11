using System;

namespace JFLCSharp
{
	public class JFLException : Exception {
		protected string ErrorMessage { get; set; }

		public JFLException() : base() {}

		public JFLException(string message) : base(message) {
			ErrorMessage = message;
		}

		public JFLException(string message, Exception inner) : base(message, inner) {
			ErrorMessage = message;
		}

		public string GetMessage() {
			return ErrorMessage;
		}
	}
}

