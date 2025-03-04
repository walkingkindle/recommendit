using DataRetriever.Models;
using Microsoft.Extensions.Options;

namespace DataRetriever.Helpers
{

public class ApiSettingsValidator : IValidateOptions<TvDbSettings>
{
    public ValidateOptionsResult Validate(string? name, TvDbSettings options)
    {
        if (string.IsNullOrEmpty(options.ApiKey) || string.IsNullOrEmpty(options.ApiUrl))
        {
            return ValidateOptionsResult.Fail("ApiKey is required in ApiSettings.");
        }

        return ValidateOptionsResult.Success;
    }
}

    }
