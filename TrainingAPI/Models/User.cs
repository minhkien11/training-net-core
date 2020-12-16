﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingAPI.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string UnsignedName { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
    }
}
