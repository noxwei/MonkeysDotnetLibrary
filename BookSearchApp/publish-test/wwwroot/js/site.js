// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
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
