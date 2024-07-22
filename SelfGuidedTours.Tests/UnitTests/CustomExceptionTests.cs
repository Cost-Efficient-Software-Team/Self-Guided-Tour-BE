using SelfGuidedTours.Core.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class CustomExceptionTests
    {
        [Test]
        public void EmailAlreadyInUseException_ShouldReturnErrorMessage()
        {
            // Arrange
            var exception = new EmailAlreadyInUseException();

            // Act
            var result = exception.Message;

            // Assert
           Assert.That(result, Is.EqualTo("User with this email already exists!"));
        }
    }
}
