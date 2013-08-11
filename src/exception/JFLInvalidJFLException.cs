using System;

namespace JFLCSharp
{
	public class JFLInvalidJFLException : JFLInvalidTextException {
		public JFLInvalidJFLException() : base() {}

		public JFLInvalidJFLException(string message) : base(message) {}

		public JFLInvalidJFLException(string message, Exception inner) : base(message, inner) {}

		public JFLInvalidJFLException(int LineNumber, int LinePosition) : base(LineNumber, LinePosition, "JFL") {}
	}
}