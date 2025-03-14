using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookSearchApp.Models;
using BookSearchApp.Services;
using System.Collections.Generic;

namespace BookSearchApp.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(BookService bookService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            LogProvider.ClearLogs();
            LogProvider.AddLog("Loading Book Search page");
            
            var viewModel = new BookSearchViewModel
            {
                AllCategories = _bookService.GetAllCategories(),
                AllSubgenres = _bookService.GetAllSubgenres(),
                AllAuthors = _bookService.GetAllAuthors(),
                SearchResults = _bookService.GetAllBooks(),
                LogMessages = LogProvider.LogMessages
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Search(SearchModel searchModel)
        {
            LogProvider.AddLog($"Search request received with term: '{searchModel.SearchTerm}'");
            
            var viewModel = new BookSearchViewModel
            {
                SearchModel = searchModel,
                AllCategories = _bookService.GetAllCategories(),
                AllSubgenres = _bookService.GetAllSubgenres(),
                AllAuthors = _bookService.GetAllAuthors(),
                SearchResults = _bookService.SearchBooks(searchModel),
                SearchTerm = searchModel.SearchTerm,
                LogMessages = LogProvider.LogMessages
            };

            return View("Index", viewModel);
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