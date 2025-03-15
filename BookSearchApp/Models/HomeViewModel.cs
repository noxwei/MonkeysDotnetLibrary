using System.Collections.Generic;

namespace BookSearchApp.Models
{
    public class HomeViewModel
    {
        public List<Book> FictionBooks { get; set; } = new List<Book>();
        public List<Book> NonFictionBooks { get; set; } = new List<Book>();
        public List<string> FictionSubgenres { get; set; } = new List<string>();
        public List<string> NonFictionSubgenres { get; set; } = new List<string>();
    }
} 