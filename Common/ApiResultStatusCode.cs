using System.ComponentModel.DataAnnotations;

namespace Common
{
    public enum ApiResultStatusCode
    {
        [Display(Name = "Success Action!")]
        Success = 1,
        [Display(Name = "Internal Server Error!")]
        ServerError = 2,
        [Display(Name = "Invalid Request!")]
        BadRequest = 3,
        [Display(Name = "Data Not Found!")]
        NotFound = 4,
        [Display(Name = "Empty ResulT!")]
        Empty = 5,
        [Display(Name = "Logical Error!")]
        LogicError = 6,
        [Display(Name = "UnAuthorized Error!")]
        UnAuthorized = 7,
        [Display(Name = "UnAuthenticated Error!")]
        UnAuthenticated = 8
    }
}
