using FluentValidation;
using MvcAgeApp.Infrastructure.GlobalConstants;
using MvcAgeApp.Models;
using System;

namespace MvcAgeApp.Infrastructure.Validators
{
    /// <summary>
    /// use fluent validtors for validation logic which is hard to express in DataAnnotations
    /// More readbale this way in my opinion
    /// </summary>
    public class UserLoginViewModelValidator : AbstractValidator<UserLoginViewModel>
    {
        public UserLoginViewModelValidator()
        {
            //check for null date of birth
            RuleFor(x => x.Dob).NotNull().WithMessage("Please enter your date of birth");

            //check for < 18 years of age
            RuleFor(x => x.Dob).LessThan(DateTime.Now.Date.AddYears(-18))
                .WithMessage(AppConstants.Under18LoginAttemptError);

            

        }

    }
}
