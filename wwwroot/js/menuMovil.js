// menuMobile.js - Control del menú hamburguesa responsivo

document.addEventListener("DOMContentLoaded", function () {
    const btnMenu = document.getElementById("btnMenu");
    const sidebar = document.querySelector(".sidebar");
    const overlay = document.createElement("div");

    overlay.classList.add("overlay-menu");
    document.body.appendChild(overlay);

    btnMenu.addEventListener("click", function () {
        sidebar.classList.toggle("activa");
        overlay.classList.toggle("activa");
    });

    overlay.addEventListener("click", function () {
        sidebar.classList.remove("activa");
        overlay.classList.remove("activa");
    });
});
