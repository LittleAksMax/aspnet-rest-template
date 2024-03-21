namespace Movies.Contracts.Responses.Validation;

public class ValidationResponse
{
    public required string PropertyName { get; init; }
    public required string ErrorMessage { get; init; }
}