using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
namespace SelfGuidedTours.Core.CustomExceptions
{
    public class EmailAlreadyInUseException : Exception
    {
        public EmailAlreadyInUseException() : base(UserWithEmailAlreadyExistsErrorMessage)
        {

        }
    }
}
