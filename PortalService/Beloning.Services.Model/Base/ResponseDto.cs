using System;

namespace Beloning.Services.Model
{
    public class ResponseDto
    {
        public string Result { get; set; }
        public string Error { get; set; }
        public bool Success => string.IsNullOrWhiteSpace(Error);

        public static ResponseDto CreateSuccessResponseDto(string result = "")
        {
            return new ResponseDto
            {
                Result = result
            };
        }

        public static ResponseDto CreateErrorResponseDto(string message)
        {
            return new ResponseDto
            {
                Error = message
            };
        }
        
    }
}