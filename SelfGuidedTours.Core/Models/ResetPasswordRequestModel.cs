using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Models
{
    public class ResetPasswordRequestModel
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
