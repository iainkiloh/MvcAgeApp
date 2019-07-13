using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MvcAgeApp.Controllers;
using MvcAgeApp.Infrastructure.GlobalConstants;
using MvcAgeApp.Infrastructure.Validators;
using MvcAgeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests.MvcAgeApp
{
    public class LoginControllerTests
    {
        #region setup
        private UserLoginViewModelValidator validator;

        private void InitializeAutoMapper()
        {
            //set up AutoMapper to map from Contracts to Entities and Vice Versa
            Mapper.Reset(); //ok for unit tests - not for production
            Mapper.Initialize(config =>
            {
                config.CreateMap<LoginAttempt, LoginAttemptViewModel>();
                config.CreateMap<UserLoginViewModel, LoginAttempt>()
                .ForMember(dest => dest.Name, opt => opt.NullSubstitute("Not Supplied"))
                .ForMember(dest => dest.Email, opt => opt.NullSubstitute("Not Supplied"))
                .ForMember(dest => dest.LoginAttemptTime, opt => opt.MapFrom(o => DateTime.Now));
            }
            );
        }
        #endregion

        //tests login fails if user already has 3 falied attempts in last hour
        [Fact]
        public void Lockout3WithinLastHourWorks()
        {
            //Arrange
            InitializeAutoMapper();
            var mock = new Mock<ILoginAttemptRepository>();
            mock.SetupGet(m => m.LoginAttempts)
                .Returns(new []
                {
                    new LoginAttempt { Email="iain@test.com", Name="iain", LoginSuccess = false, LoginAttemptTime = DateTime.Now, Id = 1 },
                    new LoginAttempt { Email="iain@test.com", Name="iain", LoginSuccess = false, LoginAttemptTime = DateTime.Now, Id = 2 },
                    new LoginAttempt { Email="iain@test.com", Name="iain", LoginSuccess = false, LoginAttemptTime = DateTime.Now, Id = 3 }
                }.AsQueryable());

            var controller = new UserLoginController(mock.Object);

            //Act 
            var result = controller.UserLogin(new UserLoginViewModel { Dob = DateTime.Now.AddYears(-20), Email = "iain@test.com", Name = "iain" });

            //Assert
            Assert.True(result.GetType().Name == nameof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.True(viewResult.ViewData.ModelState.ErrorCount == 1);

        }

        //tests login is successful with 2 failed logins within last hour
        [Fact]
        public void Lockout2WithinLastHourWorks()
        {
            //Arrange
            InitializeAutoMapper();
            var mock = new Mock<ILoginAttemptRepository>();
            mock.SetupGet(m => m.LoginAttempts)
                .Returns(new[]
                {
                    new LoginAttempt { Email="iain@test.com", Name="iain", LoginSuccess = false, LoginAttemptTime = DateTime.Now, Id = 1 },
                    new LoginAttempt { Email="iain@test.com", Name="iain", LoginSuccess = false, LoginAttemptTime = DateTime.Now, Id = 2 }
                    
                }.AsQueryable());

            var controller = new UserLoginController(mock.Object);

            //Act 
            var result = controller.UserLogin(new UserLoginViewModel { Dob = DateTime.Now.AddYears(-20), Email = "iain@test.com", Name = "iain" });

            //Assert
            Assert.True(result.GetType().Name == nameof(RedirectToActionResult));
            var actionResult = (RedirectToActionResult)result;
            Assert.True(actionResult.ActionName == "UserLoginList");

        }

        [Fact]
        public void Under18CantAccess_FluentValidator()
        {
            //Arrange
            //set up fluent validation
            validator = new UserLoginViewModelValidator();

            //Act 
            var response = validator.ShouldHaveValidationErrorFor(login =>
                login.Dob, DateTime.Now.AddYears(-17));

            //Assert
            Assert.True(response.Any() == true);
            Assert.True(response.First().ErrorMessage == AppConstants.Under18LoginAttemptError);
        }

        
        [Fact]
        public void CheckLoginHistoryListFiltered()
        {
            //Arrange
            InitializeAutoMapper();
            var mock = new Mock<ILoginAttemptRepository>();
            mock.SetupGet(m => m.LoginAttempts)
               .Returns(new[]
               {
                    new LoginAttempt { Email="iain@test.com", Name="iain", LoginSuccess = false, LoginAttemptTime = DateTime.Now, Id = 1 },
                    new LoginAttempt { Email="iain@test.com", Name="iain", LoginSuccess = false, LoginAttemptTime = DateTime.Now, Id = 2 },
                    new LoginAttempt { Email="saul@test.com", Name="saul", LoginSuccess = true, LoginAttemptTime = DateTime.Now, Id = 3 }

               }.AsQueryable());

            var controller = new UserLoginController(mock.Object);

            //Act 
            var result = controller.UserLoginList("iain", null);

            //Assert
            Assert.True(result.GetType().Name == nameof(ViewResult));
            var viewResult = (ViewResult)result;
            var data = (IEnumerable<LoginAttemptViewModel>)viewResult.Model;
            Assert.True(data.Count() == 2);
        }

        [Fact]
        public void CheckLoginHistoryListNonFiltered()
        {
            //Arrange
            InitializeAutoMapper();
            var mock = new Mock<ILoginAttemptRepository>();
            mock.SetupGet(m => m.LoginAttempts)
                .Returns(new[]
                {
                    new LoginAttempt { Email="iain@test.com", Name="iain", LoginSuccess = false, LoginAttemptTime = DateTime.Now, Id = 1 },
                    new LoginAttempt { Email="iain@test.com", Name="iain", LoginSuccess = false, LoginAttemptTime = DateTime.Now, Id = 2 },
                    new LoginAttempt { Email="saul@test.com", Name="saul", LoginSuccess = true, LoginAttemptTime = DateTime.Now, Id = 3 }

                }.AsQueryable());

            var controller = new UserLoginController(mock.Object);

            //Act 
            var result = controller.UserLoginList(null, null);

            //Assert
            Assert.True(result.GetType().Name == nameof(ViewResult));
            var viewResult = (ViewResult)result;
            var data = (IEnumerable<LoginAttemptViewModel>)viewResult.Model;
            Assert.True(data.Count() == 3);
        }



    }
}
