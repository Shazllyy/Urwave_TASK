﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>(); // Initialize the collection
    }
}