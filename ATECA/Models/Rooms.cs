﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATECA.Models
{
    public class Rooms
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sleep { get; set; }
        public string Beds { get; set; }
        public string Description { get; set; }
        public int? Price { get; set; }
        public string Category { get; set; }
    }
}