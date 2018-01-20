﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models
{
    public class Comment
    {
        public string Username { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
