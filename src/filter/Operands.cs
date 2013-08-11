using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace JFLCSharp
{
	public class Property : FilterTreeNode {
		private List<string> PropertyChain = new List<string>();

		public Property(List<string> propertyChain) : base(NodeType.Property) {
			PropertyChain = propertyChain;
		}

		public override Object GetValue(JToken scope) {
			Object returnVal = scope;
			foreach (string property in PropertyChain) {
				if (returnVal.GetType() == typeof(JObject)) {
					var currProperty =((JObject)returnVal).Property(property);
					if (currProperty != null)
						returnVal = currProperty.Value;
					else
						return null;
				} else
					return null;
			}

			if (returnVal.GetType() == typeof(JValue)) {
				var jReturnVal = (JValue)returnVal;
				if (jReturnVal.Type == JTokenType.Boolean && Inversed)
					return !((bool)jReturnVal.Value);
				return jReturnVal.Value;
			}
			return returnVal;
		}

		public bool Exists(JToken scope) {
			return GetValue(scope) != null;
		}
	}

	public class Value : FilterTreeNode {
		private JValue Val { get; set; }

		public Value(Object obj) : base(NodeType.Value) {
			Val = new JValue(obj);
			if (Val.Type == JTokenType.Boolean)
				ExprType = NodeType.Bool;
		}

		public override Object GetValue(JToken scope) {
			return Val.Value;
		}
	}

	public class RegExp : FilterTreeNode {
		private Regex Regexp { get; set; }

		public RegExp(string regex) : base(NodeType.Regex) {
			try {
				Regexp = new Regex(regex);
			} catch (ArgumentException invalidRegex) {
				throw new JFLInvalidRegexException(invalidRegex.Message);
			}
		}

		public override Object GetValue(JToken scope) {
			return null;
		}

		public bool DoesMatch(string value) {
			return value != null && 
				Regexp.IsMatch(value);
		}
	}

	public class Existence : FilterTreeNode {
		public Existence() :base(NodeType.Existence) {}

		public override Object GetValue (JToken scope) {
			return null;
		}
	}
}

