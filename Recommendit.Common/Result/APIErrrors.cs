namespace Recommendit.Result;

public class APIErrrors
{
    public static readonly Error ApiResponseError = new Error("API Error", "We could not fetch the API response for this call.");
    public static Error LoginError = new Error("Login Error", "We could not fetch the API response for this login.");
}