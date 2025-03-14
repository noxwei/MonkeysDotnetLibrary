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
                string detailsJson = File.ReadAllText(_detailsPath);
                string indexJson = File.ReadAllText(_indexPath);

                _books = JsonSerializer.Deserialize<List<Book>>(detailsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _bookIndices = JsonSerializer.Deserialize<List<BookIndex>>(indexJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading book data");
                _books = new List<Book>();
                _bookIndices = new List<BookIndex>();
            }
        }

        public List<Book> GetAllBooks()
        {
            return _books;
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
            var query = _books.AsQueryable();

            // Filter by main category if specified
            if (!string.IsNullOrWhiteSpace(searchModel.MainCategory))
            {
                query = query.Where(b => b.MainCategory == searchModel.MainCategory);
            }

            // Filter by subgenres if any selected
            if (searchModel.SelectedSubgenres != null && searchModel.SelectedSubgenres.Any())
            {
                query = query.Where(b => b.Subgenres.Any(s => searchModel.SelectedSubgenres.Contains(s)));
            }

            // Filter by authors if any selected
            if (searchModel.SelectedAuthors != null && searchModel.SelectedAuthors.Any())
            {
                query = query.Where(b => searchModel.SelectedAuthors.Contains(b.Author));
            }

            // Search by term if provided
            if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
            {
                string searchTerm = searchModel.SearchTerm.ToLower();
                
                query = query.Where(b => 
                    (searchModel.SearchInTitle && b.Title.ToLower().Contains(searchTerm)) ||
                    (searchModel.SearchInSummary && b.Summary.ToLower().Contains(searchTerm)) ||
                    (searchModel.SearchInKeywords && b.Keywords.Any(k => k.ToLower().Contains(searchTerm)))
                );
            }

            return query.ToList();
        }

        public Book GetBookById(string id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }
    }
} 