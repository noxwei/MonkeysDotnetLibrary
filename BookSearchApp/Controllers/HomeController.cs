using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookSearchApp.Models;
using BookSearchApp.Services;
using System.Linq;

namespace BookSearchApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BookService _bookService;

    public HomeController(ILogger<HomeController> logger, BookService bookService)
    {
        _logger = logger;
        _bookService = bookService;
    }

    public IActionResult Index()
    {
        var allBooks = _bookService.GetAllBooks();
        
        // Group books by main category
        var fictionBooks = allBooks.Where(b => b.MainCategory.ToLower() == "fiction").ToList();
        var nonFictionBooks = allBooks.Where(b => b.MainCategory.ToLower() != "fiction").ToList();
        
        // Get subgenres for fiction and non-fiction
        var fictionSubgenres = fictionBooks
            .SelectMany(b => b.Subgenres)
            .Distinct()
            .OrderBy(s => s)
            .ToList();
            
        var nonFictionSubgenres = nonFictionBooks
            .SelectMany(b => b.Subgenres)
            .Distinct()
            .OrderBy(s => s)
            .ToList();
            
        var viewModel = new HomeViewModel
        {
            FictionBooks = fictionBooks,
            NonFictionBooks = nonFictionBooks,
            FictionSubgenres = fictionSubgenres,
            NonFictionSubgenres = nonFictionSubgenres
        };
        
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
