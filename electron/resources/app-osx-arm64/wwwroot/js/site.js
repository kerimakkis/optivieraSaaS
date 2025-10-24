// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Language switching functionality
function changeLanguage(culture) {
    showLoading();
    document.cookie = `.AspNetCore.Culture=c=${culture}|uic=${culture}; path=/; max-age=31536000`;
    location.reload();
}

// Optiviera Loading Spinner Functions
function showLoading() {
    // Remove existing loading if any
    hideLoading();
    
    const loadingHTML = `
        <div id="optiviera-loading" class="optiviera-loading">
            <div class="optiviera-loading-content">
                <div class="optiviera-spinner"></div>
            </div>
        </div>
    `;
    
    document.body.insertAdjacentHTML('beforeend', loadingHTML);
}

function hideLoading() {
    const loading = document.getElementById('optiviera-loading');
    if (loading) {
        loading.classList.add('hidden');
        setTimeout(() => {
            loading.remove();
        }, 500);
    }
}

// Global loading for page transitions
document.addEventListener('DOMContentLoaded', function() {
    // Hide loading on page load
    hideLoading();
    
    // Show loading on form submissions
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function() {
            showLoading();
        });
    });
    
    // Show loading on navigation links
    const navLinks = document.querySelectorAll('a:not([href^="#"]):not([href^="javascript:"]):not([target="_blank"])');
    navLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            // Don't show loading for external links or hash links
            if (this.href && !this.href.startsWith('http') && !this.href.includes('#')) {
                showLoading();
            }
        });
    });
});

// Show loading on window beforeunload
window.addEventListener('beforeunload', function() {
    showLoading();
});
