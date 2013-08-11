using System;
using Newtonsoft.Json;

namespace JFLCSharp
{
	public class JFLInvalidJsonException : JFLInvalidTextException {
		public JFLInvalidJsonException() : base() {}

		public JFLInvalidJsonException(string message) : base(message) {}

		public JFLInvalidJsonException(string message, Exception inner) : base(message, inner) {}

		public JFLInvalidJsonException(int LineNumber, int LinePosition) : base(LineNumber, LinePosition, "JSON") {}
	}
}

