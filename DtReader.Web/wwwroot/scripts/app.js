
(function () {
    import("/scripts/spotlight/spotlight.js").then(module => {
        const Spotlight = module;
        (window ?? {}).imageViewer = {
            startViewer: function(gallery) {
                console.log("start viewer");
                Spotlight.show(gallery, { autoslide: false, title:false, description:false});
            }
        };

        const imageViewClass = "spotlightImageViewer";
        Spotlight.init();
        function loadImageViewer(el) {
            const imageData = el.getElementsByClassName(imageViewClass);
            if(imageData.length === 1) {
                console.log("imageData", imageData)
                const gallery = JSON.parse(imageData[0].value);
                console.log("gallery", gallery[0])
                console.log("viewer launch!");
                window.imageViewer.startViewer(gallery);
            } else {
                console.log("no image data")
            }
        }
        document.body.addEventListener("htmx:afterSettle", function(detail) {
            console.log("afterSettle");
            loadImageViewer(detail.target);
        });
        Blazor.addEventListener("enhancedload", function(detail) {
            console.log("enhancedload");
            loadImageViewer(document.body);
        });
        console.log(document.body.getElementsByClassName(imageViewClass));
        loadImageViewer(document.body); 
    });
})();