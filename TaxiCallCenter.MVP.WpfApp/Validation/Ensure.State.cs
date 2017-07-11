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
        public static class State
        {
            [DebuggerStepThrough]
            [ContractAnnotation("value:null => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotNull<T>(T value)
                where T : class
            {
                if (value == null)
                {
                    throw new InvalidOperationException();
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("value:null => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotNull<T>(T value, String message)
                where T : class
            {
                if (value == null)
                {
                    throw new InvalidOperationException(message);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("value:notnull => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Null<T>(T value)
                where T : class
            {
                if (value != null)
                {
                    throw new InvalidOperationException();
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("value:notnull => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Null<T>(T value, String message)
                where T : class
            {
                if (value != null)
                {
                    throw new InvalidOperationException(message);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("condition:false => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MeetCondition(Boolean condition)
            {
                if (!condition)
                {
                    throw new InvalidOperationException();
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("condition:false => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void MeetCondition(Boolean condition, String message)
            {
                if (!condition)
                {
                    throw new InvalidOperationException(message);
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("condition:true => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void DoesNotMeetCondition(Boolean condition)
            {
                if (condition)
                {
                    throw new InvalidOperationException();
                }
            }

            [DebuggerStepThrough]
            [ContractAnnotation("condition:true => halt")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void DoesNotMeetCondition(Boolean condition, String message)
            {
                if (condition)
                {
                    throw new InvalidOperationException(message);
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotEmpty(String value)
            {
                if (value == String.Empty)
                {
                    throw new InvalidOperationException();
                }
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotEmpty(String value, String message)
            {
                if (value == String.Empty)
                {
                    throw new InvalidOperationException(message);
                }
            }

            [ContractAnnotation("value:null => halt")]
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotNullOrEmpty(String value)
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException();
                }
            }

            [ContractAnnotation("value:null => halt")]
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void NotNullOrEmpty(String value, String message)
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}