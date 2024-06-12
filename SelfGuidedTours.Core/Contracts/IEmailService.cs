using SelfGuidedTours.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IEmailService
    {
        Task SendEmail(SendEmailDto sendEmailRequest,string emailBodyFormat);
    }
}
