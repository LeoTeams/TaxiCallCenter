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
            public static partial class IsConvertible
            {
                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Guid ToGuid(String value, [InvokerParameterName]String argumentName)
                {
                    if (!Guid.TryParse(value, out var result))
                    {
                        throw new ArgumentException($"{argumentName} is not a valid Guid value", argumentName);
                    }

                    return result;
                }

                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public static Guid ToGuid(String value, [InvokerParameterName]String argumentName, String message)
                {
                    if (!Guid.TryParse(value, out var result))
                    {
                        throw new ArgumentException(message, argumentName);
                    }

                    return result;
                }
            }
        }
    }
}