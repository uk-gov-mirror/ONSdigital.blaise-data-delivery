using System;

namespace BlaiseDataDelivery.Helpers
{
    internal static class ParameterValidationHelper
    {
        public static void ThrowExceptionIfNullOrEmpty(this string parameter, string parameterName)
        {
            if(parameter == null)
            {
                throw new ArgumentNullException($"The parameter '{parameterName}' must be supplied"); 
            }

            if (string.IsNullOrWhiteSpace(parameter))
            {

                throw new ArgumentException($"A value for the parameter '{parameterName}' must be supplied");
            }
        }

        public static void ThrowExceptionIfNull<T>(this T parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException($"The parameter '{parameterName}' must be supplied");
            }
        }
    }
}
