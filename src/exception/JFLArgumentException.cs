using System;

namespace JFLCSharp
{
	public class JFLArgumentException : JFLException {
		public JFLArgumentException() : base() {}

		public JFLArgumentException(string message) : base(message) {}

		public JFLArgumentException(string message, Exception inner) : base(message, inner) {}
	}
}

