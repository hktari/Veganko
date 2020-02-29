using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veganko;
using Veganko.ViewModels;
using Autofac;
using Veganko.Services;
using Veganko.Services.Http;
using System.Collections.Generic;

namespace UnitTests.Shared.ViewModels.Account
{
    [TestClass]
    public class RegisterViewModelTests
    {
        [TestMethod]
        public void TestSubmitCommand_ErrorHandling()
        {
            MockAccountService accountService = (MockAccountService)App.IoC.Resolve<IAccountService>();
            accountService.SetError(new ServiceException
            {
                Errors = new Dictionary<string, string[]>
                {
                    { "Username", new[]{ "username taken" }},
                    { "Email", new[]{ "email taken" }},
                    { "Password", new[]{ "invalid password" }},
                }
            });

            RegisterViewModel vm = new RegisterViewModel();
            vm.Username.Value = "test";
            vm.Email.Value = "test@gmail.com";
            vm.PasswordInput.Password.Value = "Test123.";
            vm.PasswordInput.ConfirmPassword.Value = "Test123.";

            vm.SubmitCommand.Execute(null);

            Assert.IsFalse(vm.Username.IsValid);
            Assert.AreEqual("username taken", vm.Username.Errors[0]);

            Assert.IsFalse(vm.Email.IsValid);
            Assert.AreEqual("email taken", vm.Email.Errors[0]);

            Assert.IsFalse(vm.PasswordInput.Password.IsValid);
            Assert.AreEqual("invalid password", vm.PasswordInput.Password.Errors[0]);
        }
    }
}
