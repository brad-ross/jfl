using System;
using Newtonsoft.Json.Linq;

namespace JFLCSharp
{
	public abstract class FilterTreeNode {
		public abstract Object GetValue(JToken scope);

		public bool DoesMatch(JToken scope) {
			try {
				var value = GetValue(scope);
				if (value != null)
					return (bool)value;
				return false;
			} catch (InvalidCastException) {
				return false;
			}
		}

		public enum NodeType {
			Value,
			Property,
			Regex,
			Bool,
			Existence
		}

		public NodeType ExprType { get; protected set; }

		public bool Inversed { get; protected set; }

		public FilterTreeNode(NodeType type) {
			Inversed = false;
			ExprType = type;
		}

		public void ToggleInversed() {
			Inversed = (Inversed ? false : true);
		}
	}
}

