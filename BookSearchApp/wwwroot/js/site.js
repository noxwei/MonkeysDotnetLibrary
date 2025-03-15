// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    // Detect if user is on Apple device
    const isAppleDevice = /iPhone|iPad|iPod|Mac/.test(navigator.userAgent);
    if (isAppleDevice) {
        document.documentElement.classList.add('apple-device');
    }
    
    // Dark mode functionality
    const darkModeToggle = document.getElementById('dark-mode-toggle');
    const darkIcon = document.getElementById('dark-icon');
    const lightIcon = document.getElementById('light-icon');
    
    // Check for saved theme preference or use system preference
    const prefersDarkScheme = window.matchMedia('(prefers-color-scheme: dark)');
    
    // First check session storage (for page-to-page consistency)
    let storedTheme = sessionStorage.getItem('theme');
    
    // If not in session, check localStorage (for returning visits)
    if (!storedTheme) {
        storedTheme = localStorage.getItem('theme');
    }
    
    // Function to set the theme
    function setTheme(isDark) {
        const themeName = isDark ? 'dark' : 'light';
        
        document.documentElement.setAttribute('data-bs-theme', themeName);
        
        if (isDark) {
            document.body.classList.add('dark-mode');
            darkIcon.classList.add('d-none');
            lightIcon.classList.remove('d-none');
        } else {
            document.body.classList.remove('dark-mode');
            lightIcon.classList.add('d-none');
            darkIcon.classList.remove('d-none');
        }
        
        // Store in both session (for current browsing session across pages)
        // and local storage (for returning visits)
        sessionStorage.setItem('theme', themeName);
        localStorage.setItem('theme', themeName);
    }
    
    // Set initial theme based on preference
    if (storedTheme === 'dark') {
        setTheme(true);
    } else if (storedTheme === 'light') {
        setTheme(false);
    } else {
        setTheme(prefersDarkScheme.matches);
    }
    
    // Toggle theme when button clicked
    if (darkModeToggle) {
        darkModeToggle.addEventListener('click', () => {
            const isDarkTheme = document.documentElement.getAttribute('data-bs-theme') === 'dark';
            setTheme(!isDarkTheme);
        });
    }
    
    // Update theme when system preference changes
    prefersDarkScheme.addEventListener('change', (e) => {
        const userSetTheme = sessionStorage.getItem('theme') || localStorage.getItem('theme');
        // Only update if user hasn't manually set a preference
        if (!userSetTheme) {
            setTheme(e.matches);
        }
    });

    // Highlight search terms in results
    const searchTerm = document.querySelector('input[name="SearchModel.SearchTerm"]')?.value?.toLowerCase();
    
    if (searchTerm && searchTerm.length > 2) {
        const cardTexts = document.querySelectorAll('.card-text, .card-title');
        
        cardTexts.forEach(element => {
            const text = element.textContent;
            if (text.toLowerCase().includes(searchTerm)) {
                const regex = new RegExp(`(${searchTerm})`, 'gi');
                element.innerHTML = text.replace(regex, '<mark>$1</mark>');
            }
        });
    }

    // Make cards clickable
    const bookCards = document.querySelectorAll('.book-card');
    bookCards.forEach(card => {
        const detailsButton = card.querySelector('.show-details');
        
        card.addEventListener('click', function(e) {
            // Don't trigger if clicking on the button itself
            if (!e.target.closest('.show-details')) {
                detailsButton.click();
            }
        });
    });

    // Responsive adjustments
    function adjustLayout() {
        const filterPanel = document.querySelector('.filter-panel');
        if (window.innerWidth < 768 && filterPanel) {
            filterPanel.classList.add('collapse');
        } else if (filterPanel) {
            filterPanel.classList.remove('collapse');
        }
    }

    // Initial call and event listener
    adjustLayout();
    window.addEventListener('resize', adjustLayout);
});
