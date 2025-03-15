using System;
using System.Collections.Generic;

namespace BookSearchApp.Models
{
    public class Book
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string MainCategory { get; set; } = string.Empty;
        public List<string> Subgenres { get; set; } = new List<string>();
        public List<string> Keywords { get; set; } = new List<string>();
    }
} 