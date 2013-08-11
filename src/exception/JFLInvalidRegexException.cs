using System;

namespace JFLCSharp
{
	public class JFLInvalidRegexException : JFLException {
		public JFLInvalidRegexException() : base() {}

		public JFLInvalidRegexException(string message) : base(message) {}

		public JFLInvalidRegexException(string message, Exception inner) : base(message, inner) {}
	}
}

