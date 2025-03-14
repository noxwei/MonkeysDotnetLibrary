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
        private const int PageSize = 9; // Number of books per page

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
            
            var viewModel = new BookSearchViewModel
            {
                AllCategories = _bookService.GetAllCategories(),
                AllSubgenres = _bookService.GetAllSubgenres(),
                AllAuthors = _bookService.GetAllAuthors(),
                SearchResults = paginatedBooks,
                LogMessages = LogProvider.LogMessages,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(allBooks.Count / (double)PageSize),
                TotalBooks = allBooks.Count,
                SelectedSubgenre = subgenre
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Search(SearchModel searchModel, int page = 1, string subgenre = null)
        {
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
            
            var searchResults = _bookService.SearchBooks(searchModel);
            var paginatedResults = GetPaginatedBooks(searchResults, page);
            
            var viewModel = new BookSearchViewModel
            {
                SearchModel = searchModel,
                AllCategories = _bookService.GetAllCategories(),
                AllSubgenres = _bookService.GetAllSubgenres(),
                AllAuthors = _bookService.GetAllAuthors(),
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
    }
} 