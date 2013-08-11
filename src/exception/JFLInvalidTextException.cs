using System;

namespace JFLCSharp
{
	public class JFLInvalidTextException : JFLException {
		public int LineNumber { get; private set; }
		public int LinePosition { get; private set; }

		public JFLInvalidTextException() : base() {}

		public JFLInvalidTextException(string message) : base(message) {}

		public JFLInvalidTextException(string message, Exception inner) : base(message, inner) {}

		public JFLInvalidTextException(int LineNumber, int LinePosition, string textType) : 
			base("Invalid " + textType + " at " + LineNumber + ":" + LinePosition + ".") {
			this.LineNumber = LineNumber;
			this.LinePosition = LinePosition;
		}
	}
}

