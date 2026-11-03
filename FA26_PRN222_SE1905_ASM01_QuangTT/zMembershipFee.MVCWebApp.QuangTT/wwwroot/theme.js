/* Apply before paint; delegate clicks so Blazor navigation keeps working. */
(function () {
    "use strict";
    var root = document.documentElement;
    var saved;
    try { saved = localStorage.getItem("membership-theme"); } catch (_) {}
    root.dataset.theme = saved === "dark" || saved === "light"
        ? saved : (window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light");
    function sync() {
        document.querySelectorAll("[data-theme-toggle]").forEach(function (button) {
            var dark = root.dataset.theme === "dark";
            button.textContent = dark ? "Light mode" : "Dark mode";
            button.setAttribute("aria-pressed", String(dark));
        });
    }
    document.addEventListener("click", function (event) {
        if (!event.target.closest("[data-theme-toggle]")) return;
        root.dataset.theme = root.dataset.theme === "dark" ? "light" : "dark";
        try { localStorage.setItem("membership-theme", root.dataset.theme); } catch (_) {}
        sync();
    });
    document.addEventListener("DOMContentLoaded", sync);
    document.addEventListener("enhancedload", sync);
})();
