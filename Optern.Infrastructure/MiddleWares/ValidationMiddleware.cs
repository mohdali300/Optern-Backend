using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Optern.Infrastructure.MiddleWares
{
	public class ValidationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IEnumerable<IValidator> _validators;

		public ValidationMiddleware(RequestDelegate next, IEnumerable<IValidator> validators)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_validators = validators;
		}

		// start invoke middleware
		public async Task InvokeAsync(HttpContext context)
		{
			// check if it is a graphql request
			if (context.Request.Path.StartsWithSegments("/graphql"))
			{
				// read request body
				var requestBody=await new StreamReader(context.Request.Body).ReadToEndAsync();
				//convert to Json
				var requestJson=JsonConvert.DeserializeObject<dynamic>(requestBody);

				if(requestJson?.variables!=null)
				{
					foreach(var validator in _validators)
					{
						// get inputs type to match its validator
						var modelType = validator.GetType().GetGenericArguments()[0];
						var input = requestJson?.variables?[modelType.Name];

						if (input != null)
						{
							var result = await ValidateInput(input.ToString(), modelType, validator);
							if (!result.IsValid)
							{
								await HandleValidationFailure(context, result);
								return;
							}
						}
					}
				}

			}

			// if validation done and inputs is valid, request will continu to resolver (service)
			await _next(context);
		}


		// take inputs and his type to validate it with its validator
		// return result of the running validation
		private async Task<ValidationResult> ValidateInput(string input,Type modeltype,IValidator validator)
		{
			var deserializedInput = JsonConvert.DeserializeObject(input, modeltype);
			var result = await (Task<ValidationResult>)validator.GetType().GetMethod("ValidateAsync")
				.Invoke(validator, new[] { deserializedInput });

			return result;
		}


		// executing if validation failed
		//return property that validation failed in, and error message
		private async Task HandleValidationFailure(HttpContext context,ValidationResult validationResult)
		{
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			var errors = new
			{
				errors = validationResult.Errors.Select(e => new
				{
					Field = e.PropertyName,
					Message = e.ErrorMessage,
				})
			};

			await context.Response.WriteAsync(JsonConvert.SerializeObject(errors));
		}
	}
}