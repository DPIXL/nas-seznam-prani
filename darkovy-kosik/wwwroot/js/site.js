// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const header = document.querySelector('header'); // Change to your header's class/ID
const stickyElement = document.querySelector('.download_button');

function updateStickyOffset() {
    stickyElement.style.top = `${header.offsetHeight + 10}px`;
}

updateStickyOffset();
window.addEventListener('resize', updateStickyOffset);