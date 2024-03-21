namespace Movies.Contracts.Responses.Validation;

public class ValidationFailureResponse
{
    public required IEnumerable<ValidationResponse> Errors { get; init; }
}