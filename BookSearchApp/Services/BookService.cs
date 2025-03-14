using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BookSearchApp.Models;
using Microsoft.Extensions.Logging;

namespace BookSearchApp.Services
{
    public class BookService
    {
        private readonly ILogger<BookService> _logger;
        private readonly string _detailsPath;
        private readonly string _indexPath;
        private List<Book> _books;
        private List<BookIndex> _bookIndices;

        public BookService(ILogger<BookService> logger)
        {
            _logger = logger;
            _detailsPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "books-detail.json");
            _indexPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "books-index.json");
            LoadBooks();
        }

        private void LoadBooks()
        {
            try
            {
                LogProvider.AddLog($"Loading books from {_detailsPath} and {_indexPath}");
                
                // Log the current directory to help diagnose path issues
                string currentDirectory = Directory.GetCurrentDirectory();
                LogProvider.AddLog($"Current directory: {currentDirectory}");
                
                // Check if Data directory exists
                string dataDirectory = Path.Combine(currentDirectory, "Data");
                if (!Directory.Exists(dataDirectory))
                {
                    LogProvider.AddLog($"ERROR: Data directory not found: {dataDirectory}");
                    _books = new List<Book>();
                    _bookIndices = new List<BookIndex>();
                    return;
                }
                else
                {
                    LogProvider.AddLog($"Data directory found: {dataDirectory}");
                    // List files in the Data directory
                    string[] files = Directory.GetFiles(dataDirectory);
                    LogProvider.AddLog($"Files in Data directory: {string.Join(", ", files.Select(Path.GetFileName))}");
                }
                
                if (!File.Exists(_detailsPath))
                {
                    LogProvider.AddLog($"ERROR: Book details file not found: {_detailsPath}");
                    _books = new List<Book>();
                    _bookIndices = new List<BookIndex>();
                    return;
                }
                else
                {
                    LogProvider.AddLog($"Book details file found: {_detailsPath}");
                }
                
                if (!File.Exists(_indexPath))
                {
                    LogProvider.AddLog($"ERROR: Book index file not found: {_indexPath}");
                    _books = new List<Book>();
                    _bookIndices = new List<BookIndex>();
                    return;
                }
                else
                {
                    LogProvider.AddLog($"Book index file found: {_indexPath}");
                }
                
                string detailsJson = File.ReadAllText(_detailsPath);
                string indexJson = File.ReadAllText(_indexPath);
                
                LogProvider.AddLog($"Details JSON length: {detailsJson.Length} characters");
                LogProvider.AddLog($"Index JSON length: {indexJson.Length} characters");

                try
                {
                    _books = JsonSerializer.Deserialize<List<Book>>(detailsJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    LogProvider.AddLog($"Successfully deserialized book details. Count: {(_books?.Count ?? 0)}");
                    
                    if (_books != null && _books.Count > 0)
                    {
                        // Log a sample book to verify data
                        var sampleBook = _books[0];
                        LogProvider.AddLog($"Sample book: Id={sampleBook.Id}, Title={sampleBook.Title}, Author={sampleBook.Author}");
                    }
                }
                catch (Exception ex)
                {
                    LogProvider.AddLog($"ERROR deserializing book details: {ex.Message}");
                    _books = new List<Book>();
                }

                try
                {
                    _bookIndices = JsonSerializer.Deserialize<List<BookIndex>>(indexJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    LogProvider.AddLog($"Successfully deserialized book indices. Count: {(_bookIndices?.Count ?? 0)}");
                }
                catch (Exception ex)
                {
                    LogProvider.AddLog($"ERROR deserializing book indices: {ex.Message}");
                    _bookIndices = new List<BookIndex>();
                }
                
                LogProvider.AddLog($"Successfully loaded {_books?.Count ?? 0} books and {_bookIndices?.Count ?? 0} book indices");
            }
            catch (Exception ex)
            {
                LogProvider.AddLog($"ERROR: {ex.Message}");
                LogProvider.AddLog($"Stack trace: {ex.StackTrace}");
                _books = new List<Book>();
                _bookIndices = new List<BookIndex>();
            }
        }

        public List<Book> GetAllBooks()
        {
            LogProvider.AddLog($"Getting all books, count: {_books?.Count ?? 0}");
            return _books?.ToList() ?? new List<Book>();
        }

        public List<BookIndex> GetAllBookIndices()
        {
            return _bookIndices;
        }

        public List<string> GetAllCategories()
        {
            return _books.Select(b => b.MainCategory).Distinct().OrderBy(c => c).ToList();
        }

        public List<string> GetAllSubgenres()
        {
            return _books.SelectMany(b => b.Subgenres).Distinct().OrderBy(s => s).ToList();
        }

        public List<string> GetAllAuthors()
        {
            return _books.Select(b => b.Author).Distinct().OrderBy(a => a).ToList();
        }

        public List<Book> SearchBooks(SearchModel searchModel)
        {
            LogProvider.AddLog($"Searching books with term: '{searchModel.SearchTerm}', Category: '{searchModel.MainCategory}', " +
                              $"Subgenres: {(searchModel.SelectedSubgenres?.Count ?? 0)}, Authors: {(searchModel.SelectedAuthors?.Count ?? 0)}");
            
            var query = _books.AsQueryable();

            // Filter by main category if specified
            if (!string.IsNullOrWhiteSpace(searchModel.MainCategory))
            {
                LogProvider.AddLog($"Filtering by category: {searchModel.MainCategory}");
                query = query.Where(b => b.MainCategory == searchModel.MainCategory);
            }

            // Filter by subgenres if any selected
            if (searchModel.SelectedSubgenres != null && searchModel.SelectedSubgenres.Any())
            {
                LogProvider.AddLog($"Filtering by subgenres: {string.Join(", ", searchModel.SelectedSubgenres)}");
                query = query.Where(b => b.Subgenres.Any(s => searchModel.SelectedSubgenres.Contains(s)));
            }

            // Filter by authors if any selected
            if (searchModel.SelectedAuthors != null && searchModel.SelectedAuthors.Any())
            {
                LogProvider.AddLog($"Filtering by authors: {string.Join(", ", searchModel.SelectedAuthors)}");
                query = query.Where(b => searchModel.SelectedAuthors.Contains(b.Author));
            }

            // Search by term if provided
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                string searchTerm = searchModel.SearchTerm.ToLower();
                LogProvider.AddLog($"Searching for term: {searchTerm} in " +
                                  $"Title: {searchModel.SearchInTitle}, " +
                                  $"Summary: {searchModel.SearchInSummary}, " +
                                  $"Keywords: {searchModel.SearchInKeywords}");
                
                query = query.Where(b => 
                    (searchModel.SearchInTitle && b.Title.ToLower().Contains(searchTerm)) ||
                    (searchModel.SearchInSummary && b.Summary.ToLower().Contains(searchTerm)) ||
                    (searchModel.SearchInKeywords && b.Keywords.Any(k => k.ToLower().Contains(searchTerm)))
                );
            }

            var results = query.ToList();
            LogProvider.AddLog($"Search returned {results.Count} results");
            return results;
        }

        public Book GetBookById(string id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }
    }
} 