using System.Collections.Generic;

namespace BookSearchApp.Models
{
    public class BookIndex
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string MainCategory { get; set; }
        public List<string> Subgenres { get; set; }
        public List<string> Keywords { get; set; }
    }
} 