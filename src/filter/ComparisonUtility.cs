using System;

namespace JFLCSharp
{
	//ALL FUNCTIONS IN THIS UTILITY ONLY SUPPORT VALUES SUPPORTED BY THE JVALUE CLASS
	public static class ComparisonUtility {
		const double MAX_DIFFERENCE = .00001;

		public static bool IsEqual(Object leftOperand, Object rightOperand) {
			if (leftOperand != null && rightOperand != null) {
				if (leftOperand.GetType() == typeof(bool) &&
				    rightOperand.GetType() == typeof(bool)) {
					return (bool)leftOperand == (bool)rightOperand;
				} else if (leftOperand.GetType() == typeof(DateTime) &&
				           rightOperand.GetType() == typeof(DateTime)) {
					return DateTime.Compare((DateTime)leftOperand, (DateTime)rightOperand) == 0;
				} else if (leftOperand.GetType() == typeof(Guid) &&
				           rightOperand.GetType() == typeof(Guid)) {
					return ((Guid)leftOperand).CompareTo((Guid)rightOperand) == 0;
				} else if (leftOperand.GetType() == typeof(string) &&
				           rightOperand.GetType() == typeof(string)) {
					return (string)leftOperand == (string)rightOperand;
				} else if (leftOperand.GetType() == typeof(TimeSpan) &&
				           rightOperand.GetType() == typeof(TimeSpan)) {
					return TimeSpan.Compare((TimeSpan)leftOperand, (TimeSpan)rightOperand) == 0;
				} else if (leftOperand.GetType() == typeof(Uri) &&
				           rightOperand.GetType() == typeof(Uri)) {
					return ((Uri)leftOperand).Equals((Uri)rightOperand);
				} else if (leftOperand.IsNumber() && rightOperand.IsNumber()) {
					double leftNumber = 0;
					double rightNumber = 0;

					if (leftOperand.GetType() == typeof(double))
						leftNumber = (double)leftOperand;
					else if (leftOperand.GetType() == typeof(int))
						leftNumber = (int)leftOperand;
					else if (leftOperand.GetType() == typeof(Int64))
						leftNumber = (Int64)leftOperand;
					else if (leftOperand.GetType() == typeof(Single))
						leftNumber = (Single)leftOperand;
					else if (leftOperand.GetType() == typeof(UInt64))
						leftNumber = (UInt64)leftOperand;

					if (rightOperand.GetType() == typeof(double))
						rightNumber = (double)rightOperand;
					else if (rightOperand.GetType() == typeof(int))
						rightNumber = (int)rightOperand;
					else if (rightOperand.GetType() == typeof(Int64))
						rightNumber = (Int64)rightOperand;
					else if (rightOperand.GetType() == typeof(Single))
						rightNumber = (Single)rightOperand;
					else if (rightOperand.GetType() == typeof(UInt64))
						rightNumber = (UInt64)rightOperand;

					return CompareNumbers(leftNumber, rightNumber) == 0;
				}
			}
			//If none of the cases hits, then the function can only compare based on memory address
			return leftOperand == rightOperand;
		}

		public static bool IsGreater(Object leftOperand, Object rightOperand) {
			if (leftOperand != null && rightOperand != null) {
				if (leftOperand.GetType() == typeof(DateTime) &&
				           rightOperand.GetType() == typeof(DateTime)) {
					return DateTime.Compare((DateTime)leftOperand, (DateTime)rightOperand) > 0;
				} else if (leftOperand.GetType() == typeof(Guid) &&
				           rightOperand.GetType() == typeof(Guid)) {
					return ((Guid)leftOperand).CompareTo((Guid)rightOperand) > 0;
				} else if (leftOperand.GetType() == typeof(TimeSpan) &&
				           rightOperand.GetType() == typeof(TimeSpan)) {
					return TimeSpan.Compare((TimeSpan)leftOperand, (TimeSpan)rightOperand) > 0;
				} else if (leftOperand.IsNumber() && rightOperand.IsNumber()) {
					double leftNumber = 0;
					double rightNumber = 0;

					if (leftOperand.GetType() == typeof(double))
						leftNumber = (double)leftOperand;
					else if (leftOperand.GetType() == typeof(int))
						leftNumber = (int)leftOperand;
					else if (leftOperand.GetType() == typeof(Int64))
						leftNumber = (Int64)leftOperand;
					else if (leftOperand.GetType() == typeof(Single))
						leftNumber = (Single)leftOperand;
					else if (leftOperand.GetType() == typeof(UInt64))
						leftNumber = (UInt64)leftOperand;

					if (rightOperand.GetType() == typeof(double))
						rightNumber = (double)rightOperand;
					else if (rightOperand.GetType() == typeof(int))
						rightNumber = (int)rightOperand;
					else if (rightOperand.GetType() == typeof(Int64))
						rightNumber = (Int64)rightOperand;
					else if (rightOperand.GetType() == typeof(Single))
						rightNumber = (Single)rightOperand;
					else if (rightOperand.GetType() == typeof(UInt64))
						rightNumber = (UInt64)rightOperand;

					return CompareNumbers(leftNumber, rightNumber) > 0;
				}
			}
			//If none of the cases hits, then the comparison is not valid
			return false;
		}

		public static bool IsLess(Object leftOperand, Object rightOperand) {
			if (leftOperand != null && rightOperand != null) {
				if (leftOperand.GetType() == typeof(DateTime) &&
				    rightOperand.GetType() == typeof(DateTime)) {
					return DateTime.Compare((DateTime)leftOperand, (DateTime)rightOperand) < 0;
				} else if (leftOperand.GetType() == typeof(Guid) &&
				           rightOperand.GetType() == typeof(Guid)) {
					return ((Guid)leftOperand).CompareTo((Guid)rightOperand) < 0;
				} else if (leftOperand.GetType() == typeof(TimeSpan) &&
				           rightOperand.GetType() == typeof(TimeSpan)) {
					return TimeSpan.Compare((TimeSpan)leftOperand, (TimeSpan)rightOperand) < 0;
				} else if (leftOperand.IsNumber() && rightOperand.IsNumber()) {
					double leftNumber = 0;
					double rightNumber = 0;

					if (leftOperand.GetType() == typeof(double))
						leftNumber = (double)leftOperand;
					else if (leftOperand.GetType() == typeof(int))
						leftNumber = (int)leftOperand;
					else if (leftOperand.GetType() == typeof(Int64))
						leftNumber = (Int64)leftOperand;
					else if (leftOperand.GetType() == typeof(Single))
						leftNumber = (Single)leftOperand;
					else if (leftOperand.GetType() == typeof(UInt64))
						leftNumber = (UInt64)leftOperand;

					if (rightOperand.GetType() == typeof(double))
						rightNumber = (double)rightOperand;
					else if (rightOperand.GetType() == typeof(int))
						rightNumber = (int)rightOperand;
					else if (rightOperand.GetType() == typeof(Int64))
						rightNumber = (Int64)rightOperand;
					else if (rightOperand.GetType() == typeof(Single))
						rightNumber = (Single)rightOperand;
					else if (rightOperand.GetType() == typeof(UInt64))
						rightNumber = (UInt64)rightOperand;

					return CompareNumbers (leftNumber, rightNumber) < 0;
				}
			}
			//If none of the cases hits, then the comparison is not valid
			return false;
		}

		public static bool IsGreaterOrEqual(Object leftOperand, Object rightOperand) {
			if (leftOperand != null && rightOperand != null) {
				if (leftOperand.GetType() == typeof(DateTime) &&
				    rightOperand.GetType() == typeof(DateTime)) {
					return DateTime.Compare((DateTime)leftOperand, (DateTime)rightOperand) >= 0;
				} else if (leftOperand.GetType() == typeof(Guid) &&
				           rightOperand.GetType() == typeof(Guid)) {
					return ((Guid)leftOperand).CompareTo((Guid)rightOperand) >= 0;
				} else if (leftOperand.GetType() == typeof(TimeSpan) &&
				           rightOperand.GetType() == typeof(TimeSpan)) {
					return TimeSpan.Compare((TimeSpan)leftOperand, (TimeSpan)rightOperand) >= 0;
				} else if (leftOperand.IsNumber() && rightOperand.IsNumber()) {
					double leftNumber = 0;
					double rightNumber = 0;

					if (leftOperand.GetType() == typeof(double))
						leftNumber = (double)leftOperand;
					else if (leftOperand.GetType() == typeof(int))
						leftNumber = (int)leftOperand;
					else if (leftOperand.GetType() == typeof(Int64))
						leftNumber = (Int64)leftOperand;
					else if (leftOperand.GetType() == typeof(Single))
						leftNumber = (Single)leftOperand;
					else if (leftOperand.GetType() == typeof(UInt64))
						leftNumber = (UInt64)leftOperand;

					if (rightOperand.GetType() == typeof(double))
						rightNumber = (double)rightOperand;
					else if (rightOperand.GetType() == typeof(int))
						rightNumber = (int)rightOperand;
					else if (rightOperand.GetType() == typeof(Int64))
						rightNumber = (Int64)rightOperand;
					else if (rightOperand.GetType() == typeof(Single))
						rightNumber = (Single)rightOperand;
					else if (rightOperand.GetType() == typeof(UInt64))
						rightNumber = (UInt64)rightOperand;

					return CompareNumbers(leftNumber, rightNumber) >= 0;
				}
			}
			//If none of the cases hits, then the comparison is not valid
			return false;
		}

		public static bool IsLessOrEqual(Object leftOperand, Object rightOperand) {
			if (leftOperand != null && rightOperand != null) {
				if (leftOperand.GetType() == typeof(DateTime) &&
				    rightOperand.GetType() == typeof(DateTime)) {
					return DateTime.Compare((DateTime)leftOperand, (DateTime)rightOperand) <= 0;
				} else if (leftOperand.GetType() == typeof(Guid) &&
				           rightOperand.GetType() == typeof(Guid)) {
					return ((Guid)leftOperand).CompareTo((Guid)rightOperand) <= 0;
				} else if (leftOperand.GetType() == typeof(TimeSpan) &&
				           rightOperand.GetType() == typeof(TimeSpan)) {
					return TimeSpan.Compare((TimeSpan)leftOperand, (TimeSpan)rightOperand) <= 0;
				} else if (leftOperand.IsNumber() && rightOperand.IsNumber()) {
					double leftNumber = 0;
					double rightNumber = 0;

					if (leftOperand.GetType() == typeof(double))
						leftNumber = (double)leftOperand;
					else if (leftOperand.GetType() == typeof(int))
						leftNumber = (int)leftOperand;
					else if (leftOperand.GetType() == typeof(Int64))
						leftNumber = (Int64)leftOperand;
					else if (leftOperand.GetType() == typeof(Single))
						leftNumber = (Single)leftOperand;
					else if (leftOperand.GetType() == typeof(UInt64))
						leftNumber = (UInt64)leftOperand;

					if (rightOperand.GetType() == typeof(double))
						rightNumber = (double)rightOperand;
					else if (rightOperand.GetType() == typeof(int))
						rightNumber = (int)rightOperand;
					else if (rightOperand.GetType() == typeof(Int64))
						rightNumber = (Int64)rightOperand;
					else if (rightOperand.GetType() == typeof(Single))
						rightNumber = (Single)rightOperand;
					else if (rightOperand.GetType() == typeof(UInt64))
						rightNumber = (UInt64)rightOperand;

					return CompareNumbers(leftNumber, rightNumber) <= 0;
				}
			}
			//If none of the cases hits, then the comparison is not valid
			return false;
		}

		private static bool IsNumber(this object obj)
		{   
			if (obj.GetType() == typeof(double) ||
			    obj.GetType() == typeof(int) ||
				obj.GetType() == typeof(Int64) ||
				obj.GetType() == typeof(Single) || 
				obj.GetType() == typeof(UInt64))
				return true;
			return false;
		}

		/* This function calculates the difference between values, and compares the difference to a constant threshold.
		 	This is because conversions of other number types to doubles can create slight differences between normally
		 	equal numbers (2 as an Int64 and 2 as a Single might not be equal b/c the conversion makes one into 2.0000006). */
		public static int CompareNumbers(double leftOperand, double rightOperand) {
			double difference = leftOperand - rightOperand;
			if (difference <= MAX_DIFFERENCE &&
				difference >= -MAX_DIFFERENCE)
				return 0;
			else if (leftOperand < rightOperand)
				return -1;
			else
				return 1;
		}
	}
}

