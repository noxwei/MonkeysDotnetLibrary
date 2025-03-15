using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookSearchApp.Models;
using BookSearchApp.Services;
using System.Collections.Generic;
using System.Linq;

namespace BookSearchApp.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _bookService;
        private readonly ILogger<BookController> _logger;
        private const int PageSize = 25; // Changed from 9 to 25 books per page

        public BookController(BookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        public IActionResult Index(int page = 1, string subgenre = null)
        {
            LogProvider.ClearLogs();
            LogProvider.AddLog("Loading Book Search page");
            
            var allBooks = _bookService.GetAllBooks();
            
            // Filter by subgenre if provided
            if (!string.IsNullOrEmpty(subgenre))
            {
                LogProvider.AddLog($"Filtering by subgenre: {subgenre}");
                allBooks = allBooks.Where(b => b.Subgenres.Contains(subgenre)).ToList();
            }
            
            var paginatedBooks = GetPaginatedBooks(allBooks, page);
            
            // Get filtered authors, categories and subgenres based on the current search results
            var availableAuthors = allBooks.Select(b => b.Author).Distinct().OrderBy(a => a).ToList();
            var availableCategories = allBooks.Select(b => b.MainCategory).Distinct().OrderBy(c => c).ToList();
            var availableSubgenres = allBooks.SelectMany(b => b.Subgenres).Distinct().OrderBy(s => s).ToList();
            
            var viewModel = new BookSearchViewModel
            {
                AllCategories = availableCategories, // Only show categories in the results
                AllSubgenres = availableSubgenres, // Only show subgenres in the results
                AllAuthors = availableAuthors, // Only show authors in the results
                SearchResults = paginatedBooks,
                LogMessages = LogProvider.LogMessages,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(allBooks.Count / (double)PageSize),
                TotalBooks = allBooks.Count,
                SelectedSubgenre = subgenre
            };

            return View(viewModel);
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Search(SearchModel searchModel, int page = 1, string subgenre = null)
        {
            // Initialize searchModel if it's null
            searchModel ??= new SearchModel();
            
            LogProvider.AddLog($"Search request received with term: '{searchModel.SearchTerm}'");
            
            // If subgenre is provided, add it to the selected subgenres
            if (!string.IsNullOrEmpty(subgenre) && 
                (searchModel.SelectedSubgenres == null || !searchModel.SelectedSubgenres.Contains(subgenre)))
            {
                if (searchModel.SelectedSubgenres == null)
                {
                    searchModel.SelectedSubgenres = new List<string>();
                }
                searchModel.SelectedSubgenres.Add(subgenre);
            }
            
            // Get search results filtered by search term, category, subgenres, and authors
            var searchResults = _bookService.SearchBooks(searchModel);
            var paginatedResults = GetPaginatedBooks(searchResults, page);
            
            // Only get categories, subgenres, and authors that are available in the current search results
            var availableCategories = searchResults.Select(b => b.MainCategory).Distinct().OrderBy(c => c).ToList();
            var availableSubgenres = searchResults.SelectMany(b => b.Subgenres).Distinct().OrderBy(s => s).ToList();
            var availableAuthors = searchResults.Select(b => b.Author).Distinct().OrderBy(a => a).ToList();
            
            var viewModel = new BookSearchViewModel
            {
                SearchModel = searchModel,
                AllCategories = availableCategories, // Only categories in search results
                AllSubgenres = availableSubgenres, // Only subgenres in search results
                AllAuthors = availableAuthors, // Only authors in search results
                SearchResults = paginatedResults,
                SearchTerm = searchModel.SearchTerm,
                LogMessages = LogProvider.LogMessages,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(searchResults.Count / (double)PageSize),
                TotalBooks = searchResults.Count,
                SelectedSubgenre = subgenre
            };

            return View("Index", viewModel);
        }

        private List<Book> GetPaginatedBooks(List<Book> books, int page)
        {
            return books
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public IActionResult Details(string id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [HttpGet]
        public IActionResult GetSearchSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }
            
            // Normalize the term
            term = term.ToLower().Trim();
            
            var allBooks = _bookService.GetAllBooks();
            
            // Find books that match the term in title, author, or keywords
            var matchingBooks = allBooks
                .Where(b => 
                    b.Title.ToLower().Contains(term) || 
                    b.Author.ToLower().Contains(term) || 
                    b.Keywords.Any(k => k.ToLower().Contains(term)) ||
                    b.Subgenres.Any(s => s.ToLower().Contains(term)))
                .Take(10) // Limit to 10 suggestions
                .Select(b => new
                {
                    id = b.Id,
                    title = b.Title,
                    author = b.Author,
                    category = b.MainCategory,
                    subgenres = string.Join(", ", b.Subgenres.Take(3))
                })
                .ToList();
            
            return Json(matchingBooks);
        }
    }
} 