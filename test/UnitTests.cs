using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JFLCSharp
{
	public static class UnitTests {
		public static void Main() {
			Tests();
		}

		private static void Tests() {
			string sampleJsonString = 
				@"{
										'name': 'Tom',
										'age': 13,
										'alive': true,
										'species': {
											'name': 'cat',
											'kingdom': 'Animal',
											'extinct': false,
											'ancestor': 'Sabre-Tooth Tiger'
										},
										'tagged !': true,
										'zoo': null,
										'diet': [
											'fish',
											'mice',
											'cat food'
										],
										'children': [
											{
												'name': 'Jerry',
												'age': 6,
												'alive': false,
												'tagged !': false,
												'zoo': null,
												'species': {
													'name': 'cat',
													'kingdom': 'Animal',
													'extinct': false,
													'ancestor': 'Sabre-Tooth Tiger'
												},
												children: [{
													'name': 'Jim',
													'age': 1,
													'alive': true,
													'species': {
														'name': 'cat',
														'kingdom': 'Animal',
														'extinct': false,
														'ancestor': 'Sabre-Tooth Tiger'
													}
												}]
											},
											{
												'name': 'Larry',
												'age': 4,
												'alive': true,
												'tagged !': true,
												'zoo': 'San Francisco',
												'species': {
													'name': 'cat',
													'kingdom': 'Animal',
													'extinct': false,
													'ancestor': 'Sabre-Tooth Tiger'
												}
											}
										],
										'habitats': {
											'Kitchen': {
												'name': 'Kitchen',
												'score': 10
											},
											'LivingRoom': {
												'name': 'Living Room',
												'score': 7
											}
										}
									}";

			var tom = JsonConvert.DeserializeObject<Cat>(sampleJsonString);

			TestJFL("Code Calibration",
			        @"{children[(alive&!(children?))|(species.name=""cat""&zoo!=null)|test=5]:{name},*,habitats:{/^K/:{score}},!diet}",
			        sampleJsonString,
			        @"{
						  ""name"": ""Tom"",
						  ""age"": 13,
						  ""alive"": true,
						  ""species"": {
						    ""name"": ""cat"",
						    ""kingdom"": ""Animal"",
						    ""extinct"": false,
						    ""ancestor"": ""Sabre-Tooth Tiger""
						  },
						  ""zoo"": null,
						""tagged !"": true,
						  ""children"": [
						    {
						      ""name"": ""Larry""
						    }
						  ],
						  ""habitats"": {
						    ""Kitchen"": {
						      ""score"": 10
						    }
						  }
						}"
			        );

			PrintTestGroupHeader ("Basic Functionality");

			TestJFL("Block",
			        @"{name,age,alive}",
			        sampleJsonString,
			        @"{'name': 'Tom','age': 13,'alive': true}"
			        );

			TestJFL("Sub-Block",
			        @"{species:{name,extinct}}",
			        sampleJsonString,
			        @"{'species': {
						'name': 'cat',
						'extinct': false
					}}"
			        );

			TestJFL("Star",
			        @"{*}",
			        sampleJsonString,
			        sampleJsonString
			        );

			TestJFL("Exclude",
			        @"{*,!children}",
			        sampleJsonString,
			        @"{
						'name': 'Tom',
						'age': 13,
						'alive': true,
						'species': {
							'name': 'cat',
							'kingdom': 'Animal',
							'extinct': false,
							'ancestor': 'Sabre-Tooth Tiger'
						},
						'tagged !': true,
						'zoo': null,
						'diet': [
							'fish',
							'mice',
							'cat food'
						],
						'habitats': {
							'Kitchen': {
								'name': 'Kitchen',
								'score': 10
							},
							'LivingRoom': {
								'name': 'Living Room',
								'score': 7
							}
						}
					}"
			        );

			TestJFL("Star And Specified Property",
			        @"{*,species:{name}}",
			        sampleJsonString,
			        @"{
						'name': 'Tom',
						'age': 13,
						'alive': true,
						'species': {
							'name': 'cat',
						},
						'tagged !': true,
						'zoo': null,
						'diet': [
							'fish',
							'mice',
							'cat food'
						],
						'children': [
							{
								'name': 'Jerry',
								'age': 6,
								'alive': false,
								'tagged !': false,
								'zoo': null,
								'species': {
									'name': 'cat',
									'kingdom': 'Animal',
									'extinct': false,
									'ancestor': 'Sabre-Tooth Tiger'
								},
								children: [{
									'name': 'Jim',
									'age': 1,
									'alive': true,
									'species': {
										'name': 'cat',
										'kingdom': 'Animal',
										'extinct': false,
										'ancestor': 'Sabre-Tooth Tiger'
									}
								}]
							},
							{
								'name': 'Larry',
								'age': 4,
								'alive': true,
								'tagged !': true,
								'zoo': 'San Francisco',
								'species': {
									'name': 'cat',
									'kingdom': 'Animal',
									'extinct': false,
									'ancestor': 'Sabre-Tooth Tiger'
								}
							}
						],
						'habitats': {
							'Kitchen': {
								'name': 'Kitchen',
								'score': 10
							},
							'LivingRoom': {
								'name': 'Living Room',
								'score': 7
							}
						}
					}"
			        );

			TestJFL("Escaped Property Names",
			        @"{'tagged !'}",
			        sampleJsonString,
			        @"{'tagged !': true}"
			        );

			TestJFL("Array Of Objects",
			        @"{children:{name,age}}",
			        sampleJsonString,
			        @"{'children': [
							{
								'name': 'Jerry',
								'age': 6
							},
							{
								'name': 'Larry',
								'age': 4
							}
						]}"
			        );

			TestJFL("Regex Property",
			        @"{habitats:{/^K/}}",
			        sampleJsonString,
			        @"{'habitats': {
							'Kitchen': {
								'name': 'Kitchen',
								'score': 10
							}
						}}"
			        );

			TestJFL("Regex Property With Sub-Block",
			        @"{habitats:{/^K/:{score}}}",
			        sampleJsonString,
			        @"{'habitats': {
							'Kitchen': {
								'score': 10
							}
						}}"
			        );

			PrintTestGroupHeader ("Array Filters");

			TestJFL("String Comparison 1",
			        @"{children[name=""Jerry""]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("String Comparison 2",
			        @"{children[name!=""Jerry""]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Property Dot Operator Chain",
			        @"{children[species.name=""cat""]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										},
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Regex Comparison 1",
			        @"{children[name=/^J/]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Regex Comparison 2",
			        @"{children[name!=/^J/]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Bool 1",
			        @"{children[alive]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Bool 2",
			        @"{children[!alive]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Escaped Property",
			        @"{children['tagged !']:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Bool Value Comparison 1",
			        @"{children[alive=true]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Bool Value Comparison 2",
			        @"{children[alive=false]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Null Value Comparison",
			        @"{children[zoo=null]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Inverse Bool Value",
			        @"{children[!alive]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Number Equality",
			        @"{children[age=6.0]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Number Inequality",
			        @"{children[age!=6e0]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Number Greater",
			        @"{children[age>5E0]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Number Less",
			        @"{children[age<5.45]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Number Greater or Equal",
			        @"{children[age>=6]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Number Less or Equal",
			        @"{children[age<=6]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										},
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Existential Operator",
			        @"{children[children?]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("And 1",
			        @"{children[!alive&children?]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("And 2",
			        @"{children[!alive&children?&species.name=""cat""]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Or 1",
			        @"{children[alive|children?]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										},
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Or 2",
			        @"{children[alive|children?|species.name=""cat""]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										},
										{
											'name': 'Larry'
										}
									]}"
			        );

			TestJFL("Parentheses and Order Of Operations 1",
			        @"{children[(alive&children?)|(species.name=""cat""&zoo=null)]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Jerry'
										}
									]}"
			        );

			TestJFL("Parentheses and Order Of Operations 2(With Inverse)",
			        @"{children[(alive&!(children?))|(species.name=""cat""&zoo!=null)]:{name}}",
			        sampleJsonString,
			        @"{'children': [
										{
											'name': 'Larry'
										}
									]}"
			        );

			PrintTestGroupHeader ("Errors (FAILURE is SUCCESS)");

			TestJFL("Non-Existent Filter Property",
			        @"{children[!test]:{name}}",
			        sampleJsonString,
			        @"{'children': []}"
			        );

			TestJFL("Non-Existent Property",
			        @"{test}",
			        sampleJsonString,
			        @"{}"
			        );

			TestJFL("Invalid Json",
			        @"{test,testTwo}",
			        @"{'foo' 'bar'}",
			        @"{}"
			        );

			TestJFL("Invalid JFL 1",
			        @"{test1,test2",
			        sampleJsonString,
			        @"{}"
			        );

			TestJFL("Invalid JFL 2",
			        @"{children[}",
			        sampleJsonString,
			        @"{}"
			        );

			TestJFL("Invalid JFL 3",
			        @"{children[]:}",
			        sampleJsonString,
			        @"{}"
			        );

			TestJFL("Invalid JFL 4",
			        @"{test:{name}}",
			        sampleJsonString,
			        @"{}"
			        );

			TestJFL("Invalid JFL 5",
			        @"{children[5]:{name}}",
			        sampleJsonString,
			        @"{'children':[]}"
			        );

			TestJFL("Invalid JFL 6",
			        @"{children[test=5]}",
			        sampleJsonString,
			        @"{'children':[]}"
			        );

			TestJFL("Null Arguments",
			        null,
			        null,
			        @"{}"
			        );

			TestJFL("Empty Json",
			        "{name,age}",
			        "",
			        @"{}"
			        );

			TestJFL("Empty JFL",
			        "",
			        sampleJsonString,
			        @"{}"
			        );

			TestJFL("Invalid Filter Regex",
			        "{children[name=/]:{}}",
			        sampleJsonString,
			        @"{}"
			        );

			TestJFL("Invalid Property Regex",
			        "{habitats:{/:{score}}}",
			        sampleJsonString,
			        @"{}"
			        );

			PrintTestGroupHeader ("Stress");

			string fullDuplicateObjectList = "[";
			for (int i = 0; i < 199; i++)
				fullDuplicateObjectList += sampleJsonString + ", ";
			fullDuplicateObjectList += sampleJsonString + "]";

			string reducedObject = @"{
										'name': 'Tom',
										'age': 13,
										'alive': true,
										'species': {
											'name': 'cat'
										},
										'diet': [
											'fish',
											'mice',
											'cat food'
										],
										'children': [
											{
												'name': 'Larry',
												'age': 4,
												'alive': true
											}
										],
										'habitats': {
											'Kitchen': {
												'score': 10
											}
										}
									}";
			string reducedDuplicateObjectList = "[";
			for (int i = 0; i < 199; i++)
				reducedDuplicateObjectList += reducedObject + ", ";
			reducedDuplicateObjectList += reducedObject + "]";

			TestJFL("200 Duplicate Object Filtering 1",
			        "{*}",
			        fullDuplicateObjectList,
			        fullDuplicateObjectList
			        );

			TestJFL("200 Duplicate Object Filtering 2",
			        "{name,age,alive,species:{name},diet,children[alive]:{name,age,alive},habitats:{/^K/:{score}}}",
			        fullDuplicateObjectList,
			        reducedDuplicateObjectList
			        );

			var duplicateObjectList = new List<Cat>();
			for (int i = 0; i < 200; i++) {
				duplicateObjectList.Add(tom);
			}

			TestJFL("200 Duplicate Object Filtering 3",
			        "{name,age,alive,species:{name},diet,children[alive]:{name,age,alive},habitats:{/^K/:{score}}}",
			        duplicateObjectList,
			        reducedDuplicateObjectList
			        );

			TestJFL("200 Duplicate Object Filtering 4",
			        "{*}",
			        duplicateObjectList,
			        JToken.FromObject(duplicateObjectList).ToString()
			        );
		}

		/* Version of TestJFL that accepts a JSON string to filter */
		private static void TestJFL(string testTitle, string jflString, string jsonStringToTest, string expected) {
			string refinedJsonString = "";
			Stopwatch timer = new Stopwatch();
			string errorMessage = null;

			try {
				timer.Start();
				refinedJsonString = JFLExpression.GetRefinedJsonStringFromJFL(jflString, jsonStringToTest);
				timer.Stop();
			} catch (JFLException e) {
				errorMessage = e.GetMessage();
			}

			bool didMatch = false;
			if (errorMessage == null) {
				var expectedJson = JToken.Parse(expected);
				var refinedJson = JToken.Parse(refinedJsonString);
				didMatch = JToken.DeepEquals(expectedJson, refinedJson);
			}

			PrintTestResult(testTitle, didMatch, errorMessage, refinedJsonString, timer.Elapsed.TotalMilliseconds);
		}

		/* Version of TestJFL that accepts a C# object to filter */
		private static void TestJFL(string testTitle, string JFL, Object jsonToTest, string expected) {
			string refinedJsonString = "";
			Stopwatch timer = new Stopwatch();
			string errorMessage = null;

			try {
				timer.Start();
				refinedJsonString = JFLExpression.GetRefinedJsonStringFromJFL(JFL, jsonToTest);
				timer.Stop();
			} catch (JFLException e) {
				errorMessage = e.GetMessage();
			}

			bool didMatch = false;
			if (errorMessage == null) {
				var expectedJson = JToken.Parse(expected);
				var refinedJson = JToken.Parse(refinedJsonString);
				didMatch = JToken.DeepEquals(expectedJson, refinedJson);
			}

			PrintTestResult(testTitle, didMatch, errorMessage, refinedJsonString, timer.Elapsed.TotalMilliseconds);
		}

		private static void PrintTestResult(string testTitle, bool didMatch, string errorMessage, string refinedJsonString, double timeToComplete) {
			Console.Write("\"{0}\":" +
			              "\n\t{1};", testTitle, didMatch ? "SUCCESS" : "FAILURE");
			if (!didMatch) {
				if (errorMessage != null) {
					Console.WriteLine("\n\tError: \"{0}\";", errorMessage);
				} else {
					Console.WriteLine(";\n\tActual Output: \n{0};", refinedJsonString);
				}
			} else
				Console.WriteLine("\n\tcompleted in {0} ms;", timeToComplete);

			Console.WriteLine();
		}

		private static void PrintTestGroupHeader(string headerName) {
			Console.WriteLine(headerName +
			                  "\n===========\n");
		}
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class Cat {
		[JsonProperty()]
		public string name { get; set; }

		[JsonProperty()]
		public int age { get; set; }

		[JsonProperty()]
		public bool alive { get; set; }

		[JsonProperty()]
		public Species species { get; set; }

		[JsonProperty("tagged !")]
		public bool tagged { get; set; }

		[JsonProperty()]
		public string zoo { get; set; }

		[JsonProperty()]
		public List<string> diet { get; set; }

		[JsonProperty()]
		public List<Cat> children { get; set; }

		[JsonProperty()]
		public Dictionary<string, Habitat> habitats { get; set; }

		public Cat() {}
	}

	public class Species {
		public string name { get; set; }
		public string kingdom { get; set; }
		public bool extinct { get; set; }
		public string ancestor { get; set; }

		public Species() {}
	}

	public class Habitat {
		public string name { get; set; }
		public int score { get; set; }

		public Habitat() {}
	}
}