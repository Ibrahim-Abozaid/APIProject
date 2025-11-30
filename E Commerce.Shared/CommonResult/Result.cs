using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.CommonResult
{
    public class Result
    {
        private readonly List<Error> _errors = [];
        public bool IsSuccess => _errors.Count == 0;
        public bool IsFailure => !IsSuccess;

        public List<Error> Errors => _errors;


        //ok - success
        protected Result()
        {

        }

        //fail with error
        protected Result(Error error)
        {
            _errors.Add(error);
        }

        //fail with errors
        protected Result(List<Error> errors)
        {
            _errors.AddRange(errors);
        }


        //ok - success
        public static Result Ok() => new Result();

        //fail with error
        public static Result Fail(Error error) => new Result(error);

        //fail with errors
        public static Result Fail(List<Error> errors) => new Result(errors);


    }


    public class Result<TValue> : Result
    {
        private readonly TValue _value;
        public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot acces the value");

        //ok - success
        private Result(TValue value) : base()
        {
            _value = value;
        }
        //fail with error
        private Result(Error error) : base(error)
        {
            _value = default!;
        }

        //fail with errors
        private Result(List<Error> errors) : base(errors)
        {
            _value = default!;
        }


        public static Result<TValue> Ok(TValue value) => new Result<TValue>(value);
        public static new Result<TValue> Fail(Error error) => new Result<TValue>(error);
        public static new Result<TValue> Fail(List<Error> errors) => new Result<TValue>(errors);

        public static implicit operator Result<TValue>(TValue value) => Ok(value);
        public static implicit operator Result<TValue>(Error error) => Fail(error);
        public static implicit operator Result<TValue>(List<Error> errors) => Fail(errors);



    }

}
