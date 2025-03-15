using System.Collections.Generic;

namespace BookSearchApp.Models
{
    public class SearchModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string MainCategory { get; set; } = string.Empty;
        public List<string> SelectedSubgenres { get; set; } = new List<string>();
        public List<string> SelectedAuthors { get; set; } = new List<string>();
        public bool SearchInSummary { get; set; } = true;
        public bool SearchInTitle { get; set; } = true;
        public bool SearchInKeywords { get; set; } = true;
    }
} 