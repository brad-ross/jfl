tree grammar JFLWalker;

options {
language='CSharp3';
tokenVocab=Jfl;
ASTLabelType=CommonTree;
}

@namespace{JFLCSharp}

public walk[JFLProperty jfl]
	:	block[jfl]
	;
	
block[JFLProperty currScope]
	:	^(BLOCK property[currScope]*)
	;
	
property[JFLProperty parentScope]
@init {
	JFLProperty newProperty = new JFLProperty();
}
	:	^(propertyName not='!'? filter[newProperty]? block[newProperty]?) {
			if ($not.text == "!")
				newProperty.ToggleInclude();
			if ($propertyName.isRegex)
				parentScope.AddRegexProperty($propertyName.name, newProperty);
			else
				parentScope.AddProperty($propertyName.name, newProperty);
		}
	;
	
propertyName returns [string name, bool isRegex]
	:	ID {
			$isRegex = false;
			$name=$ID.text;
			}
	|	ESC_ID {
			$isRegex = false;
			string stringText = $ESC_ID.text;
			$name = stringText.Substring(1,stringText.Length - 2);
			}
	|	REGEX {
			$isRegex = true;
			string regexText = $REGEX.text;
			$name = regexText.Substring(1,regexText.Length - 2);
			}
	|	'*' {
			$isRegex = false;
			$name = "*";
			}
	;
	
filter[JFLProperty currProperty]
	:	^(FILTER filterExpr) {
					currProperty.AddFilter($filterExpr.filterNode);
					}
	;

filterExpr returns [FilterTreeNode filterNode]
@init {
	List<string> propertySegments = new List<string>();
}
	:	^(comparator a=filterExpr b=filterExpr)
			{
				Comparator newComparator;
				switch ($comparator.returnValue) {
					case "=":
						newComparator = Comparator.Equals;
						break;
					case "!=":
						newComparator = Comparator.NotEquals;
						break;
					case ">":
						newComparator = Comparator.Greater;
						break;
					case "<":
						newComparator = Comparator.Less;
						break;
					case ">=":
						newComparator = Comparator.GreaterOrEqual;
						break;
					case "<=":
						newComparator = Comparator.LessOrEqual;
						break;
					case "&":
						newComparator = Comparator.And;
						break;
					case "|":
						newComparator = Comparator.Or;
						break;
					default:
						newComparator = Comparator.Equals;
						break;
				}
				$filterNode = new BoolExpression($a.filterNode, newComparator, $b.filterNode);
			}
	|	^(EXPR f=filterExpr inverse='!'?)
			{
				FilterTreeNode newNode = $f.filterNode;
				if ($inverse.text == "!")
					newNode.ToggleInversed();
				$filterNode = newNode;
			}
	|	^(EXPR  propertyChain[propertySegments] inverse='?'? not='!'?)
			{
				FilterTreeNode newProperty = new Property(propertySegments);
				if ($inverse.text == "?")
					$filterNode = new BoolExpression(newProperty, Comparator.Equals, new Existence());
				else {
					if ($not.text == "!")
						newProperty.ToggleInversed();
					$filterNode = newProperty;
				}
			}
	|	STRING
			{
				string stringText = $STRING.text;
				stringText = stringText.Substring(1,stringText.Length - 2);
				$filterNode = new Value(stringText);
			}
	|	REGEX
			{
				string regexText = $REGEX.text;
				regexText = regexText.Substring(1,regexText.Length - 2);
				$filterNode = new RegExp(regexText);
			}
	|	NUMBER
			{
				$filterNode = new Value(System.Double.Parse($NUMBER.text));
			}
	|	TRUE
			{
				$filterNode = new Value(true);
			}
	|	FALSE
			{
				$filterNode = new Value(false);
			}
	|	NULL
			{
				$filterNode = new Value(null);
			}
	;
	
propertyChain[List<string> propertySegments]
	:	^(ID
		{ propertySegments.Add($ID.text); }
		propertyChain[propertySegments])
	|	^(ESC_ID
		{ propertySegments.Add($ESC_ID.text); }
		propertyChain[propertySegments])
	;
	
comparator returns [string returnValue]
	:	CHAIN_OPERATOR { $returnValue = $CHAIN_OPERATOR.text; }
	|	COMPARATOR { $returnValue = $COMPARATOR.text; }
	;