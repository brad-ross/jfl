using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace JFLCSharp
{
	public class JFLProperty {
		private Dictionary<string, JFLProperty> NestedProperties = new Dictionary<string, JFLProperty>();

		private Dictionary<string, JFLProperty> RegexProperties = new Dictionary<string, JFLProperty>();

		private FilterTreeNode Filter = null;

		public bool Include { get; private set; }

		public bool ContainsNestedProperties {
			get {
				return NestedProperties.Count > 0 || 
					RegexProperties.Count > 0;
			}
		}

		public bool HasFilter {
			get {
				return Filter != null;
			}
		}

		public JFLProperty(bool include = true) {
			Include = include;
		}

		public JFLProperty GetProperty(string name) {
			if (NestedProperties.ContainsKey(name)) {
				var property = NestedProperties[name];
				if (property.Include)
					return property;
				else
				//Include is false, so the property shouldn't be provided
					return null;
			}

			foreach (KeyValuePair<string, JFLProperty> pair in RegexProperties) {
				if (Regex.IsMatch(name, pair.Key)) {
					var property = RegexProperties[pair.Key];
					if (property.Include)
						return property;
					else
					//Include is false, so the property shouldn't be provided
						return null;
				}
			}

			if (NestedProperties.ContainsKey("*")) {
				return NestedProperties["*"];
			}

			//The property has not been found
			return null;
		}

		public void AddProperty(string name, JFLProperty newProperty) {
			if (!NestedProperties.ContainsKey(name))
				NestedProperties.Add(name, newProperty);
		}

		public void ToggleInclude() {
			Include = (Include ? false : true);
		}

		public void AddRegexProperty(string regex, JFLProperty newProperty) {
			if (!NestedProperties.ContainsKey(regex)) {
				/* A way of testing validity of regex input. If there is no exception caught when
				Creating a new Regex, then it is a valid regex expression */
				try {
					Regex testValidity = new Regex(regex);
					RegexProperties.Add(regex, newProperty);
				} catch (ArgumentException invalidRegex) {
					throw new JFLInvalidRegexException(invalidRegex.Message);
				}
			}
		}

		public void AddFilter(FilterTreeNode newFilter) {
			Filter = newFilter;
		}

		public bool FilterMatches(JToken json) {
			if (Filter != null)
				return Filter.DoesMatch(json);
			//There is no filter in this case, so anything should match
			return true;
		}
	}
}

