using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using JetBrains.Annotations;

namespace TaxiCallCenter.MVP.WpfApp.Validation
{
    public static partial class Ensure
    {
        public static partial class Argument
        {
            [DebuggerStepThrough]
            [ContractAnnotation("argumentValue:null => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotNull<T>(T argumentValue, [InvokerParameterName]String argumentName)
                where T : class
            {
                if (argumentValue == null)
                {
                    throw new ArgumentNullException(argumentName);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("argumentValue:null => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotNull<T>(T argumentValue, [InvokerParameterName]String argumentName, String message)
                where T : class
            {
                if (argumentValue == null)
                {
                    throw new ArgumentNullException(argumentName, message);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("argumentValue:notnull => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Null<T>(T argumentValue, [InvokerParameterName]String argumentName)
                where T : class
            {
                if (argumentValue != null)
                {
                    throw new ArgumentNullException(argumentName);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("argumentValue:notnull => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Null<T>(T argumentValue, [InvokerParameterName]String argumentName, String message)
                where T : class
            {
                if (argumentValue != null)
                {
                    throw new ArgumentNullException(argumentName, message);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("condition:false => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MeetCondition(Boolean condition, [InvokerParameterName] String argumentName)
            {
                if (!condition)
                {
                    throw new ArgumentException($"{argumentName} does not meet required condition", argumentName);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("condition:false => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MeetCondition(Boolean condition, [InvokerParameterName] String argumentName, String message)
            {
                if (!condition)
                {
                    throw new ArgumentException(message, argumentName);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("condition:true => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void DoesNotMeetCondition(Boolean condition, [InvokerParameterName] String argumentName)
            {
                if (condition)
                {
                    throw new ArgumentException($"{argumentName} does not meet required condition", argumentName);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("condition:true => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void DoesNotMeetCondition(Boolean condition, [InvokerParameterName] String argumentName, String message)
            {
                if (condition)
                {
                    throw new ArgumentException(message, argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void EqualTo<T>(T value, T expectedValue, [InvokerParameterName] String argumentName)
                where T : IEquatable<T>
            {
                if (!value.Equals(expectedValue))
                {
                    throw new ArgumentException($"{argumentName} must be equal to {expectedValue}", argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void EqualTo<T>(T value, T expectedValue, [InvokerParameterName] String argumentName, String message)
                where T : IEquatable<T>
            {
                if (!value.Equals(expectedValue))
                {
                    throw new ArgumentException(message, argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotEqualTo<T>(T value, T notExpectedValue, [InvokerParameterName] String argumentName)
                where T : IEquatable<T>
            {
                if (value.Equals(notExpectedValue))
                {
                    throw new ArgumentException($"{argumentName} must not be equal to {notExpectedValue}", argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotEqualTo<T>(T value, T notExpectedValue, [InvokerParameterName] String argumentName, String message)
                where T : IEquatable<T>
            {
                if (value.Equals(notExpectedValue))
                {
                    throw new ArgumentException(message, argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void GreaterThan<T>(T value, T minimumValue, [InvokerParameterName] String argumentName)
                where T : IComparable<T>
            {
                if (value.CompareTo(minimumValue) <= 0)
                {
                    throw new ArgumentException($"{argumentName} must be greater than {minimumValue}", argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void GreaterThan<T>(T value, T minimumValue, [InvokerParameterName] String argumentName, String message)
                where T : IComparable<T>
            {
                if (value.CompareTo(minimumValue) <= 0)
                {
                    throw new ArgumentException(message, argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void GreaterThanOrEqualTo<T>(T value, T minimumValue, [InvokerParameterName] String argumentName)
                where T : IComparable<T>
            {
                if (value.CompareTo(minimumValue) < 0)
                {
                    throw new ArgumentException($"{argumentName} must be greater than or equal to {minimumValue}", argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void GreaterThanOrEqualTo<T>(T value, T minimumValue, [InvokerParameterName] String argumentName, String message)
                where T : IComparable<T>
            {
                if (value.CompareTo(minimumValue) < 0)
                {
                    throw new ArgumentException(message, argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void LessThan<T>(T value, T maximumValue, [InvokerParameterName] String argumentName)
                where T : IComparable<T>
            {
                if (value.CompareTo(maximumValue) >= 0)
                {
                    throw new ArgumentException($"{argumentName} must be less than {maximumValue}", argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void LessThan<T>(T value, T maximumValue, [InvokerParameterName] String argumentName, String message)
                where T : IComparable<T>
            {
                if (value.CompareTo(maximumValue) >= 0)
                {
                    throw new ArgumentException(message, argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void LessThanOrEqualTo<T>(T value, T maximumValue, [InvokerParameterName] String argumentName)
                where T : IComparable<T>
            {
                if (value.CompareTo(maximumValue) > 0)
                {
                    throw new ArgumentException($"{argumentName} must be less than or equal to {maximumValue}", argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void LessThanOrEqualTo<T>(T value, T maximumValue, [InvokerParameterName] String argumentName, String message)
                where T : IComparable<T>
            {
                if (value.CompareTo(maximumValue) > 0)
                {
                    throw new ArgumentException(message, argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotEmpty(String value, [InvokerParameterName] String argumentName)
            {
                if (value == String.Empty)
                {
                    throw new ArgumentException($"{argumentName} must not be empty", argumentName);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotEmpty(String value, [InvokerParameterName] String argumentName, String message)
            {
                if (value == String.Empty)
                {
                    throw new ArgumentException(message, argumentName);
                }
            }

            [ContractAnnotation("value:null => halt")]
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotNullOrEmpty(String value, [InvokerParameterName] String argumentName)
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"{argumentName} must not be neither null nor empty", argumentName);
                }
            }

            [ContractAnnotation("value:null => halt")]
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotNullOrEmpty(String value, [InvokerParameterName] String argumentName, String message)
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(message, argumentName);
                }
            }
        }
    }
}