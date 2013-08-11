using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JFLCSharp
{
	public partial class JFLExpression : JFLProperty {
		//Semi-wrapper function that kicks off recursive walking of the provided JSON tree
		private JToken RefineJson(JToken json) {
			if (json.Type == JTokenType.Object)
				return RefineJsonObject((JObject)json, this);
			else if (json.Type == JTokenType.Array)
				return RefineJsonArray((JArray)json, this);
			//Otherwise, the json provided isn't valid by the JSON specification
			return null;
		}

		//A recursive "router" function that determines what the current JSON should be handled as
		private JToken RefineJsonComponent(JToken jsonComponent, JFLProperty currJFLScope) {
			if (currJFLScope != null) {
				if (jsonComponent.Type == JTokenType.Array)
					return RefineJsonArray((JArray)jsonComponent, currJFLScope);
				if (currJFLScope.ContainsNestedProperties && 
				    jsonComponent.Type == JTokenType.Object) {
						return RefineJsonObject((JObject)jsonComponent, currJFLScope);
				} else {
					return jsonComponent;
				}
			}
			//Then the current property should not been included in the final JSON
			return null;
		}

		private JObject RefineJsonObject(JObject jsonObject, JFLProperty currJFLScope) {
			JObject returnObj = new JObject();

			foreach (JProperty property in jsonObject.Properties()) {
				JFLProperty currJFLProperty = currJFLScope.GetProperty(property.Name);
				JToken refinedPropertyValue = RefineJsonComponent(property.Value, currJFLProperty);

				if (refinedPropertyValue != null)
					returnObj.Add(property.Name, refinedPropertyValue);
			}

			return returnObj;
		}

		private JArray RefineJsonArray(JArray jsonArray, JFLProperty currJFLScope) {
			JArray returnArr = new JArray();

			foreach (JToken element in jsonArray) {
				JToken refinedItemValue = null;
				if (currJFLScope.FilterMatches (element)) {
					if (currJFLScope.ContainsNestedProperties)
						refinedItemValue = RefineJsonComponent (element, currJFLScope);
					else
						refinedItemValue = element;
				}

				if (refinedItemValue != null)
					returnArr.Add(refinedItemValue);
			}

			return returnArr;
		}
	}
}

