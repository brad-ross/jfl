# JSON Filtering Language (JFL)

*A simple, yet powerful syntax for specifying minimized, data-efficient JSON*

## Table of Contents

- [Why JFL?](#why-jfl)
- [A Quick Tour](#a-quick-tour)
- [Potential Use Cases](#potential-use-cases)
- [C# Library](#c-library)
	- [Downloads](#downloads)
	- [API Documentation](#api-documentation)
	- [Tests](#Tests)
	- [Source Notes](#source-notes)
	- [Limitations](#limitations)
- [Conclusion](#conclusion)

### Why JFL?

Often, JSON contains more information than the client requesting it will end up using. In addition, mobile data networks are restricted in data-transfer speed, necessitating smaller data packages to maximize responsiveness. Rather than be hampered by the various costs of dealing with extra data, clients should only need to deal with information that they specifically need. Some languages (such as [JSONPath](http://goessner.net/articles/JsonPath/)) allow clients to query existing JSON (a la SQL) and retrieve specific data from it--however, in doing so they lose the original JSON structure. Inspired by Xobni's internal filtering syntax--CAQL--and the Google API's ["fields" parameter](https://developers.google.com/google-apps/calendar/performance#partial), JFL allows clients to minimize the data carried by JSON by applying a custom pattern.

### A Quick Tour

In order to take a look at JFL's features, here is a sample JSON object that will be referenced as the object we are filtering in examples. Meet Tom:

```json
{
	"name": "Tom",
	"age": 13,
	"alive": true,
	"zoo": "San Francisco",
	"species": {
		"name": "cat",
		"kingdom": "Animal",
		"extinct": false,
		"ancestor": "Saber-Tooth Tiger"
	},
	"tagged !": true,
	"diet": [
		"fish",
		"mice",
		"cat food"
	],
	"children": [
		{
			"name": "Jerry",
			"age": 6,
			"alive": false,
			"zoo": null,
			"tagged !": false,
			"species": {
				"name": "cat",
				"kingdom": "Animal",
				"extinct": false,
				"ancestor": "Saber-Tooth Tiger"
			},
			"children": [
				{
					"name": "Jim",
					"age": 1,
					"alive": true,
					"zoo": "San Francisco",
					"species": {
						"name": "cat",
						"kingdom": "Animal",
						"extinct": false,
						"ancestor": "Saber-Tooth Tiger"
					}
				}
			]
		},
		{
			"name": "Larry",
			"age": 4,
			"alive": true,
			"tagged !": true,
			"zoo": "San Francisco",
			"species": {
				"name": "cat",
				"kingdom": "Animal",
				"extinct": false,
				"ancestor": "Saber-Tooth Tiger"
			}
		}
	],
	"habitats": {
		"Kitchen": {
			"name": "Kitchen",
			"score": 10
		},
		"LivingRoom": {
			"name": "Living Room",
			"score": 7
		}
	}
}
```

JFL expressions consist of blocks (enclosed by curly braces), evocative of JSON:

***JFL Expression:***

`{name,age,alive,species,diet}`

***Resultant JSON:***

```json
{
	"name": "Tom",
	"age": 13,
	"alive": true,
	"species": {
		"name": "cat",
		"kingdom": "Animal",
		"extinct": false,
		"ancestor": "Saber-Tooth Tiger"
	},
	"diet": [
		"fish",
		"mice",
		"cat food"
	]
}
```

This JFL expression reads as "return name, age, alive, species, and diet for this JSON object." Order of key names doesn't matter, so `{species,alive,name,diet,age}` would return the same filtered JSON object. Blocks can contain any key, whether its value is a primitive (boolean, number, string, etc.), an object, or an array.

If you include a key name in your JFL expression that is not present in the JSON object you are filtering, then JFL will gracefully ignore it:

**JFL Expression:**

`{test,testTwo}`

**Resultant JSON:**

```json
{}
```
Since you asked for only the values of `test` and `testTwo`, neither of which exist in Tom, an empty object is returned.

If you want to filter an object contained within a key, you can specify a nested block, like so:

**JFL Expression:**

`{name,species:{name,extinct}}`

**Resultant JSON:**

```json
{
	"name": "Tom",
	"species": {
		"name": "cat",
		"extinct": false
	}
}
```

What if you want to include every property, but you want to specify a specific nested block for just one key? Just use the `*` shortcut as a key name to signify the rest of the keys in the object:

**JFL Expression:**

`{species:{name,extinct},*}`

**Resultant JSON:**

```json
{
	"name": "Tom",
	"age": 13,
	"alive": true,
	"zoo": "San Francisco",
	"species": {
		"name": "cat",
		"extinct": false
	},
	...
}
```

This way, you don't have to list every key just to filter one nested object.

In addition, you can specify properties you don't want to include by attaching a `!` to the front of the property name:

**JFL Expression:**

`{*,!children,!habitats}`

**Resultant JSON:**

```json
{
	"name": "Tom",
	"age": 13,
	"alive": true,
	"zoo": "San Francisco",
	"species": {
		"name": "cat",
		"kingdom": "Animal",
		"extinct": false,
		"ancestor": "Saber-Tooth Tiger"
	},
	"tagged !": true,
	"diet": [
		"fish",
		"mice",
		"cat food"
	]
}
```

This is great shorthand for fetching all keys except for certain ones.

If a JSON key contains characters used or ignored in JFL (numbers, whitespace, `,`, `:`, `!`, `*`, etc.), just enclose the key name in single quotes--called an "escaped property":

**JFL Expression:**

`{name,'tagged !'}`

**Resultant JSON:**

```json
{
	"name": "Tom",
	"tagged !": true
}
```

What about filtering each object in an array of objects? JFL has you covered:

**JFL Expression:**

`{children:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Jerry",
			"age": 6
		},
		{
			"name": "Larry",
			"age": 4
		}
	]
}
```

JFL knows that the value contained within the `children` key is an array, and applies the nested block to every item in the array. You can think of this JFL expression as "return name and age of each object within children."

So far, JFL has only been useful for specifying a minimized structure for JSON objects. But what if we only want to see Tom's living children in our filtered JSON? How about only Tom's children who are older than 5? Only Tom's children that have children of their own? Once again, JFL has you covered.

JFL allows you to specify filters for keys that contain arrays. The resultant array will only contain items that match the criteria specified in the filter. For example:

**JFL Expression:**

`{children[age>5]:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Jerry",
			"age": 6
		}
	]
}
```

**JFL Expression:**

`{children[name="Jerry"]:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Jerry",
			"age": 6
		}
	]
}
```

**JFL Expression:**

`{children[zoo=null]:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Jerry",
			"age": 6
		}
	]
}
```

**JFL Expression:**

`{children[alive=true]:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Larry",
			"age": 4
		}
	]
}
```

Boolean expressions within array filters are similar to C-style languages. Just use `=` (or `!=`, `<`, `>=`, etc.) to compare values. The default scope of the variables in array filters is that of each object within the array. You can read the first expression as "return name and age of objects within children where age of the object is less than five," and the second as "return name and age of objects within children where name of the object is "Jerry"." Filtering doesn't have to come with nested blocks--just filter an array and leave the whole object: `{children[name="Jerry"]}`.

You can also compare keys with string values to regular expressions by using `=` and enclosing the regex in `/`s:

**JFL Expression:**

`{children[name=/^J/]:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Jerry",
			"age": 6
		}
	]
}
```

You can use keys that contain booleans as boolean values, like so:

**JFL Expression:**

`{children[!alive]:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Jerry",
			"age": 6
		}
	]
}
```

To check whether a key exists within each object, attach the "existential operator"--`?` (inspired by [Coffeescript](coffeescript.org))--to the end of a property name:

**JFL Expression:**

`{children[children?]:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Jerry",
			"age": 6
		}
	]
}
```

Here, only Jerry contains a key for `children`, so he is the only object not removed.

Use dot notation to access nested object values:

**JFL Expression:**

`{children[!species.extinct]:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Jerry",
			"age": 6
		},
		{
			"name": "Larry",
			"age": 4
		}
	]
}
```

"Escaped properties" (see above) can also be used to access properties whose names need to be escaped:

**JFL Expression:**

`{children['tagged !']:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Larry",
			"age": 4
		}
	]
}
```

Escaped properties also work in conjunction with the dot operator--`{children[species.'test ~'!=null]:{name,age}}` would be valid assuming that the `test ~` property existed within any child's `species` property.

You can also chain boolean expressions by using `&` (AND), `|` (OR), and parentheses, just like C-style boolean expressions.

**JFL Expression:**

`{children[(age>5&alive=false)|name!="Jerry"]:{name,age}}`

**Resultant JSON:**

```json
{
	"children": [
		{
			"name": "Jerry",
			"age": 6
		},
		{
			"name": "Larry",
			"age": 4
		}
	]
}
```

Sometimes, objects can have variable key names, such as Tom's `habitats` key. Each key in the `habitats` object is mapped to the name of the object it contains. If a different animal had different habitats, then that habitats object would have different keys. How can we gain consistent access to these nested objects, and how can we filter objects with variable keys as we did with arrays? JFL provides the ability to specify regular expressions (within `/`s) to match key names:

**JFL Expression:**

`{habitats:{/^K/}}`

**Resultant JSON:**

```json
{
	"habitats": {
		"Kitchen": {
			"name": "Kitchen",
			"score": 10
		}
	}
}
```

In the above example JFL, the nested block within habitats only returned key-value pairs whose key name matched the regular expression. This makes filtering objects contained within variable key names a breeze:

**JFL Expression:**

`{habitats:{/Ki*/:{score}}}`

**Resultant JSON:**

```json
{
	"habitats": {
		"Kitchen": {
			"score": 10
		}
	}
}
```

### Potential Use Cases

The envisioned use of this library is similar to [Google's fields parameter](https://developers.google.com/google-apps/calendar/performance#partial). When a client makes a request to an API--say `www.foo.com`--they would include a query string parameter with their JFL, e.g.

`www.foo.com/bar/42?jfl={children[(alive&!(children?))|(species.name=""cat""&zoo!=null)|test=5]:{name},*,habitats:{/^K/:{score}},!diet}`

`www.foo.com`'s server would then apply the JFL to its default response to `/bar/42` and return that filtered response to the requesting client.

This being said, there could be other applications of JFL waiting to be discovered.

### C# Library

Initially, I have built a C# JFL library that allows users to leverage *most* of JFL's intended functionality in a native environment. While this implementation is usable, its lack of complete feature parity and performance issues--both explained in detail [below](#limitations)--might render it as more of a "proof-of-concept" to some. Be aware of these issues if you decide to include this library in your project.

####Downloads

To download the compiled library (`JFLCSharp.dll`) along with its dependencies (`Antlr3.Runtime.dll` and `Newtonsoft.Json.dll`), grab the latest release [here](https://github.com/brad-ross-35/jfl/releases/tag/v0.1.0).

To use the source, download the code from this repository and refer to the [Source Notes](#source-notes) section below for compilation instructions and more.

#### API Documentation

The C# library uses a `JFLExpression` object to represent JFL expressions. Pass a valid JFL string to the constructor to create a new expression:

```csharp
var expression = new JFLExpression("{name,age,species:{name}}");
```
	
You can then get a string of filtered JSON using `JFLExpression.GetRefinedJsonString`, which can take either a JSON string, or any C# object:

```csharp
string tomString = @"
	{
		'name': 'Tom',
		'age': 13,
		'species': {
			'name': 'cat',
			'kingdom': 'Animal',
			'extinct': false,
			'ancestor': 'Saber-Tooth Tiger'
		}
	}
";
													
var tomObject = new Animal {
	name = "Tom",
	age = 13,
	species = new Species {
		name = "cat",
		kingdom = "Animal",
		extinct = false,
		ancestor = "Saber-Tooth Tiger"
	}
}
													
string filteredJsonStringOne = expression.GetRefinedJsonString(tomString);
string filteredJsonStringTwo = expression.GetRefinedJsonString(tomObject);

/* Both calls will output:
	{
		"name": "Tom",
		"age": 13,
		"species": {
			"name": "cat"
		}
	}
*/
```
	
In addition, the library provides a static method `JFLExpression.GetRefinedJsonStringFromJFL` that takes a JFL expression and either a JSON string or a C# object and returns a filtered JSON string:

```csharp
string filteredJsonThree = 
			JFLExpression.GetRefinedJsonStringFromJFL("{name,age,species:{name}}", tomString);																													
string filteredJsonFour = 
			JFLExpression.GetRefinedJsonStringFromJFL("{name,age,species:{name}}", tomObject);
																																
/* Both calls will output:
	{
		"name": "Tom",
		"age": 13,
		"species": {
			"name": "cat"
		}
	}
*/
```

The above methods comprise the complete public API. However, the C# library also contains an error reporting system for invalid inputs. All exceptions thrown by JFL inherit from `JFLException`.

If you pass a JFL function an empty or null JFL or JSON string or a null C# object, a `JFLArgumentException` will be thrown:

```csharp
try {
	var expressionOne = new JFLExpression("");
	var expressionTwo = new JFLExpression(null);
	
	string filteredJsonStrOne = expressionOne.GetFilteredJsonString("");
	string filteredJsonStrTwo = expressionTwo.GetFilteredJsonString(null);
	
	string staticJsonStrOne = JFLExpression.GetFilteredJsonStringFromJFL("", "");
	string staticJsonStrTwo = JFLExpression.GetFilteredJsonStringFromJFL(null, null);
} catch (JFLArgumentException e) {
	Console.WriteLine(e.Message);
}
```
	
If you try to pass invalid JFL or JSON into a library function, either a `JFLInvalidJFLException` or a `JFLInvalidJsonException` will be thrown. Both inherit from `JFLInvalidTextException`, which contains a line number and a line position where in the string the error occurred:

```csharp
try {
	var expression = new JFLExpression("{name,age");
	string filteredJson = expression.GetFilteredJsonString("{"name": "Tom" "age": 13"});
} catch (JFLInvalidTextException e) {
	Console.WriteLine("Error: " + e.LineNumber + ":" + e.LinePosition);
}
```
	
Finally, If you try to include an invalid regular expression in your JFL, a `JFLInvalidRegexException` will be thrown with a message explaining the error:

```csharp
try {
	var expression = new JFLExpression("{children[name=/^J/],habitats{/^K/}}");
} catch (JFLInvalidRegexException e) {
	Console.WriteLine(e.Message);
}
```

#### Tests

Amateurishly, I did not know that testing software like NUnit existed when I first built JFL. As a result, I cobbled together my own test rig at the time and wrote a relatively exhaustive bunch of test cases using it. You can find my testing code in the `test` folder. Currently, I don't have time to learn NUnit and write a new set of tests, but I hope to get this done in the future.

#### Source Notes

Currently, the build scripts included in `./build` are only Bash. As a result, you will have to build the source using [Mono](http://www.mono-project.com) on a Linux-based operating system if you want the build process to automatically generate the parser and create a zip containing all of the required `.dll`s.

Both the `.sln` and `.csproj` files are included in the source, which should allow you to use [Monodevelop](http://monodevelop.com/) to build automatically if you prefer an IDE.

If you would like to use a different version of one or both of JFLCSharp's dependencies--e.g. when you are already using Newtonsoft.Json and/or Antlr3.Runtime in your code--you will have to download the source, remove the old dependencies from the project's References folder, add your new ones, and build. Currently, JFLCSharp uses **Newtonsoft.Json version [version]** and **Antlr3.Runtime version [version]**.

As of now, tests have not been included in the csharp project itself so they aren't compiled into the JFLCSharp library. Until a way to prevent this presents itself, you will have to add the file(s) within the `test` folder to the project manually when you download the source, build the source as an executable rather than as a library, and run it manually. The test results will be printed to the console.

#### Limitations

As of the original release, the C# library does not support escaped properties (see above) or numbers in key names due to some strange issues with the [ANTLR](http://www.antlr.org/) parser generator. Hopefully it will be fixed in a future version of JFL.

In addition, this library suffers from performance issues when filtering large JSON inputs. This dip is due to how the filtering algorithm is structured. Try it, and if it doesn't suit your needs wait for other libraries to be built, or contribute your own.

### Conclusion

JFL would not have been possible without the support of Frank Cort, Francois Richard, Peter Monaco, and the rest of the Xobni team. I cannot thank them enough for their generous support of the project. Also, big thanks to Ryan Gerard, the creator CAQL, Xobni's homegrown filtering language, for inspiring my work on JFL.

While many hours have gone into this project, it is by no means a finished product. The success of JFL depends on other developers building libraries in other languages and integrating it into their APIs. While I want to continue overseeing JFL, it remains to be seen how much more work I can put into this project myself. Constructive suggestions are encouraged, but others who are willing to continue championing the project are even more welcome. I look forward to seeing where others take JFL in the future. Contact me with suggestions or send me links to new implementations or improvements at brad.ross.35@gmail.com.