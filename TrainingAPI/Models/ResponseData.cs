using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingAPI.Models
{
    public class ResponseData<T>
    {
        public string Message { get; set; }
        public T Data { get; set; } = (T)Activator.CreateInstance(typeof(T));
    }
}
