using System;

namespace Beloning.Services.Model
{
    public class ResponseDto<T> 
    {
        public T Result { get; set; }
        public string Error { get; set; }
        public bool Success => string.IsNullOrWhiteSpace(Error);

        public static ResponseDto<T> CreateSuccessResponseDto(T result)
        {
            return new ResponseDto<T>
            {
                Result = result
            };
        }

        public static ResponseDto<T> CreateErrorResponseDto(string message)
        {
            return new ResponseDto<T>
            {
                Error = message
            };
        }
    }
}