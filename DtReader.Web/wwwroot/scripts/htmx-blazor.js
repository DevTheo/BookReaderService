// An enhanced load allows users to navigate between different pages
console.log("htmx loaded");
Blazor.addEventListener("enhancedload", function () {
    console.log("updating htmx tags");
    // HTMX need to reprocess any htmx tags because of enhanced loading
     setTimeout(()=> htmx.process(document.body), 100);
});
