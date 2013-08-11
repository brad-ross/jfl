using System;
using System.IO;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace JFLCSharp
{
	public partial class JFLExpression : JFLProperty {
		private void ParseJFLString(string JFLString) {
			Stream s = GenerateStreamFromString (JFLString);
			try {
				ANTLRInputStream input = new ANTLRInputStream (s);
				JFLLexer lexer = new JFLLexer (input);
				CommonTokenStream tokens = new CommonTokenStream (lexer);
				JFLParser parser = new JFLParser (tokens);
				var result = parser.jfl ();

				CommonTree t = (CommonTree)result.Tree;
				CommonTreeNodeStream nodes = new CommonTreeNodeStream (t);

				JFLWalker walker = new JFLWalker (nodes);
				walker.walk(this);
			} catch (RecognitionException e) {
				throw new JFLInvalidJFLException(e.Line, e.CharPositionInLine);
			}
		}

		private Stream GenerateStreamFromString(string str) {
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(str);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}

