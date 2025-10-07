// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Language switching functionality
function changeLanguage(culture) {
    document.cookie = `.AspNetCore.Culture=c=${culture}|uic=${culture}; path=/; max-age=31536000`;
    location.reload();
}
