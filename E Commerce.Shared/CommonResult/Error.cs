using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.CommonResult
{
    public class Error
    {
        private Error(string code, string description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            ErrorType = errorType;
        }

        public string Code { get; }
        public string Description { get; }
        public ErrorType ErrorType { get; }


        public static Error Failure(string code = "General.Failure" , string description = "General Failure Has Occured")
        {
            return new Error(code, description, ErrorType.Failure);
        }

        public static Error Validation(string code = "General.Validation" , string description = "Validation Error Has Occured")
        {
            return new Error(code, description, ErrorType.Validation);
        }


        public static Error NotFound(string code = "General.NotFound", string description = "The Requested Resource has not found")
        {
            return new Error(code, description, ErrorType.NotFound);
        }

        public static Error Unauthorized(string code = "General.Unauthorized", string description = "Your Are Not Authorized")
        {
            return new Error(code, description, ErrorType.Unauthorized);
        }

        public static Error Forbidden( string code = "General.Forbidden", string description = "You Don't have Permissions")
        {
            return new Error(code, description, ErrorType.Forbidden);
        }

        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description = "The Provided Credentials are Invalid")
        {
            return new Error(code, description, ErrorType.InvalidCredentials);
        }





    }
}
