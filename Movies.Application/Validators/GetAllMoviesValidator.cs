using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class GetAllMoviesValidator : AbstractValidator<GetAllMoviesOptions>
{
    public static readonly string[] AcceptableSortFields =
    {
        "title",
        "yearofrelease"
    };
    
    public GetAllMoviesValidator()
    {
        RuleFor(x=>x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.SortField).
            Must(x => x is null || AcceptableSortFields.Contains(x , StringComparer.OrdinalIgnoreCase))
            .WithMessage("you can only sort by title of year of release");

        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        
        RuleFor(x => x.PageSize).InclusiveBetween(1, 25);
    }
}