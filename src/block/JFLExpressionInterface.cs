using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JFLCSharp
{
	public partial class JFLExpression : JFLProperty {
		public JFLExpression(string JFLString) : base() {
			if (JFLString == null || JFLString == "")
				throw new JFLArgumentException("JFL string is null or empty.");
			else
				ParseJFLString(JFLString);
		}

		public string GetRefinedJsonString(string jsonString) {
			if (jsonString == null || jsonString == "") {
				throw new JFLArgumentException("JSON string is null or empty.");
			} else {
				JToken json;
				try {
					json = JToken.Parse(jsonString);
				} catch (JsonReaderException invalidJson) {
					throw new JFLInvalidJsonException(invalidJson.LineNumber, invalidJson.LinePosition);
				}
				return RefineJson(json).ToString();
			}
		}

		public string GetRefinedJsonString(Object obj) {
			if (obj == null) {
				throw new JFLArgumentException("Object to filter is null.");
			} else {
				var json = JToken.FromObject(obj);
				return RefineJson(json).ToString();
			}
		}

		public static string GetRefinedJsonStringFromJFL(string JFL, string jsonString) {
			var JFLObject = new JFLExpression(JFL);
			return JFLObject.GetRefinedJsonString(jsonString);
		}

		public static string GetRefinedJsonStringFromJFL(string JFL, Object jsonObj) {
			var JFLObject = new JFLExpression(JFL);
			return JFLObject.GetRefinedJsonString(jsonObj);
		}
	}
}

