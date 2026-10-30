// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const header = document.querySelector('header'); // Change to your header's class/ID
const stickyElement = document.querySelector('.apk_header');

function updateStickyOffset() {
    stickyElement.style.top = `${header.offsetHeight + 10}px`;
}

updateStickyOffset();
window.addEventListener('resize', updateStickyOffset);

function dismissAppPromo() {
    
    const promoContainer = document.querySelector('.apk_header');
    if (promoContainer) {
        promoContainer.style.setProperty('display', 'none', 'important');
    }

    const date = new Date();
    date.setTime(date.getTime() + (365 * 24 * 60 * 60 * 1000));
    document.cookie = "hideAppPromo=true; expires=" + date.toUTCString() + "; path=/; SameSite=Strict";
}