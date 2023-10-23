// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var isNavBarShowed = false;
function ShowNavBar(){
    if (!isNavBarShowed) {
        document.getElementById("navbar").style.display = "initial";
        isNavBarShowed = true;
    }
    else {
        document.getElementById("navbar").style.display = "none";
        isNavBarShowed = false;
    }
}