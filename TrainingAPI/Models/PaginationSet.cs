using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingAPI.Models
{
    public class PaginationSet<T>
    {
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public List<T> Items { get; set; } = (List<T>)Activator.CreateInstance(typeof(List<T>));
    }
}
