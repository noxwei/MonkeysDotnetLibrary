using System.Collections.Generic;

namespace BookSearchApp.Models
{
    public class BookSearchViewModel
    {
        public SearchModel SearchModel { get; set; } = new SearchModel();
        public List<Book> SearchResults { get; set; } = new List<Book>();
        public List<string> AllCategories { get; set; } = new List<string>();
        public List<string> AllSubgenres { get; set; } = new List<string>();
        public List<string> AllAuthors { get; set; } = new List<string>();
        public string SearchTerm { get; set; }
        public List<string> LogMessages { get; set; } = new List<string>();
    }
} 