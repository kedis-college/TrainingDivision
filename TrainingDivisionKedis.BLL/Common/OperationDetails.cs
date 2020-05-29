using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDivisionKedis.BLL.Common
{
    public class OperationDetails<T>
    {
        private OperationDetails()
        {
        }

        private OperationDetails(T entity)
        {
            Entity = entity;
        }


        public bool Succedeed { get; private set; }
        public Error Error { get; private set; }
        public T Entity { get; }

        public static OperationDetails<T> Success(T entity)
        {
            var result = new OperationDetails<T>(entity)
            {
                Succedeed = true
            };
            return result;
        }

        public static OperationDetails<T> Failure(string message, string property)
        {
            var result = new OperationDetails<T>
            {
                Succedeed = false,
                Error = new Error(message, property)
            };
            return result;
        }
    }

    public class Error
    {
        public Error(string message, string property)
        {
            Message = message;
            Property = property;
        }

        public string Message { get; }
        public string Property { get; }
    }
}
