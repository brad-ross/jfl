using System;
using Newtonsoft.Json.Linq;

namespace JFLCSharp
{
	public class BoolExpression : FilterTreeNode {
		private FilterTreeNode LeftOperand { get; set; }

		private Comparator BoolComparator { get; set; }

		private FilterTreeNode RightOperand { get; set; }

		public BoolExpression(FilterTreeNode leftOperand, Comparator comparator, FilterTreeNode rightOperand) : base(NodeType.Bool) {
			LeftOperand = leftOperand;
			BoolComparator = comparator;
			RightOperand = rightOperand;
		}

		public override Object GetValue(JToken scope) {
			bool returnValue = Evaluate(scope);
			if (Inversed)
				returnValue = !returnValue;
			return returnValue;
		}

		private bool Evaluate(JToken scope) {
			if (BoolComparator == Comparator.Equals) {
				return EvaluateOperandEquality (scope);
			} else if (BoolComparator == Comparator.NotEquals) {
				return EvaluateOperandNotEquality (scope);
			} else if (LeftOperand.ExprType != NodeType.Regex &&
			           RightOperand.ExprType != NodeType.Regex) {
				if (BoolComparator == Comparator.Greater) {
					return EvaluateGreaterThan(scope);
				} else if (BoolComparator == Comparator.Less) {
					return EvaluateLessThan(scope);
				} else if (BoolComparator == Comparator.GreaterOrEqual) {
					return EvaluateGreaterThanOrEquals(scope);
				} else if (BoolComparator == Comparator.LessOrEqual) {
					return EvaluateLessThanOrEquals(scope);
				} else if ((LeftOperand.ExprType == NodeType.Bool ||
				            LeftOperand.ExprType == NodeType.Property) && 
				           (RightOperand.ExprType == NodeType.Bool ||
				 			RightOperand.ExprType == NodeType.Property)) {
					if (BoolComparator == Comparator.And) {
						return EvaluateAnd(scope);
					} else if (BoolComparator == Comparator.Or) {
						return EvaluateOr(scope);
					}
				}
			}
			//The boolean expression hasn't matched any of the valid cases, and therefore nothing will match it
			return false;
		}

		private bool EvaluateOperandEquality(JToken scope) {
			if (RightOperand.ExprType == NodeType.Regex)
				return ((RegExp)RightOperand).DoesMatch ((string)LeftOperand.GetValue (scope));
			else if (LeftOperand.ExprType == NodeType.Property &&
			         RightOperand.ExprType == NodeType.Existence)
				return ((Property)LeftOperand).Exists(scope);
			else if (LeftOperand.ExprType == NodeType.Regex)
				return ((RegExp)LeftOperand).DoesMatch ((string)RightOperand.GetValue (scope));
			else if (LeftOperand.ExprType != NodeType.Regex &&
			         RightOperand.ExprType != NodeType.Regex) 
				return ComparisonUtility.IsEqual(LeftOperand.GetValue(scope), RightOperand.GetValue(scope));

			return false;
		}

		private bool EvaluateOperandNotEquality(JToken scope) {
			return !EvaluateOperandEquality(scope);
		}

		private bool EvaluateGreaterThan(JToken scope) {
			return ComparisonUtility.IsGreater(LeftOperand.GetValue(scope), RightOperand.GetValue(scope));
		}

		private bool EvaluateLessThan(JToken scope) {
			return ComparisonUtility.IsLess(LeftOperand.GetValue(scope), RightOperand.GetValue(scope));
		}

		private bool EvaluateGreaterThanOrEquals(JToken scope) {
			return ComparisonUtility.IsGreaterOrEqual (LeftOperand.GetValue(scope), RightOperand.GetValue(scope));
		}

		private bool EvaluateLessThanOrEquals(JToken scope) {
			return ComparisonUtility.IsLessOrEqual(LeftOperand.GetValue(scope), RightOperand.GetValue(scope));
		}

		private bool EvaluateAnd(JToken scope) {
			var leftValue = LeftOperand.GetValue(scope);
			var rightValue = RightOperand.GetValue(scope);
			if (leftValue.GetType() == typeof(bool) &&
			    rightValue.GetType() == typeof(bool))
				return (bool)leftValue && (bool)rightValue;
			return false;
		}

		private bool EvaluateOr(JToken scope) {
			var leftValue = LeftOperand.GetValue(scope);
			var rightValue = RightOperand.GetValue(scope);
			if (leftValue != null && rightValue != null &&
				leftValue.GetType() == typeof(bool) &&
			    rightValue.GetType() == typeof(bool))
				return (bool)leftValue || (bool)rightValue;
			return false;
		}
	}
}

