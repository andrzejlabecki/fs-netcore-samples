using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Fs.Core.Contracts;
using Fs.Core.Interfaces.Services;

namespace Fs.Core.Exceptions
{
    public class RulesException : Exception
    {
        private readonly List<ErrorInfo> _errors;

        public RulesException(string propertyName, string errorMessage, string prefix = "")
        {
            _errors = Errors;
            _errors.Add(new ErrorInfo($"{prefix}{propertyName}", errorMessage));
        }

        public RulesException(string propertyName, string errorMessage, object onObject, string prefix = "")
        {
            _errors = Errors;
            _errors.Add(new ErrorInfo($"{prefix}{propertyName}", errorMessage, onObject));
        }

        public RulesException()
        {
            _errors = Errors;
        }

        public RulesException(List<ErrorInfo> errors)
        {
            _errors = errors;
        }

        public List<ErrorInfo> Errors
        {
            get
            {
                return _errors ?? new List<ErrorInfo>();
            }
        }
    }

    public class ErrorInfo
    {
        private readonly string _errorMessage;
        private readonly string _propertyName;
        private readonly object _onObject;

        public ErrorInfo(string propertyName, string errorMessage)
        {
            _propertyName = propertyName;
            _errorMessage = errorMessage;
            _onObject = null;
        }

        public ErrorInfo(string propertyName, string errorMessage, object onObject)
        {
            _propertyName = propertyName;
            _errorMessage = errorMessage;
            _onObject = onObject;
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }

    }

    public static class RulesExceptionExtensions
    {
        public static void AddModelStateErrors(this RulesException ex, ModelStateDictionary modelState)
        {
            foreach (var error in ex.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        public static void AddModelStateErrors(this IEnumerable<RulesException> errors, ModelStateDictionary modelState)
        {
            foreach (RulesException ex in errors)
            {
                ex.AddModelStateErrors(modelState);
            }
        }

        public static IEnumerable Errors(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                return modelState.ToDictionary(kvp => kvp.Key,
                    kvp => kvp.Value
                        .Errors
                        .Select(e => e.ErrorMessage).ToArray())
                        .Where(m => m.Value.Count() > 0);
            }
            return null;
        }

        public static IEnumerable Errors(this ModelStateDictionary modelState, bool fixName = false)
        {
            if (!modelState.IsValid)
            {
                return modelState.ToDictionary(kvp => fixName ? kvp.Key.Replace("model.", string.Empty) : kvp.Key,
                    kvp => kvp.Value
                        .Errors
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value entered." : e.ErrorMessage).ToArray())
                        .Where(m => m.Value.Count() > 0);
            }
            return null;
        }
    }

}
