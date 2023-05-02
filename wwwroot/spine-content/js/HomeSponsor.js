$(document).ready(function () {
    sr.reveal('#bnTitle', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px',
        origin: "left"
    });

    sr.reveal('#bnBreadcrumbs', {
        duration: 1600,
        delay: 300,
        opacity: 0,
        scale: 1,
        distance: '70px',
        origin: "left"
    });
    
    sr.reveal('.saText', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    sr.reveal('#pjiCollage', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px',
        origin: "bottom"
    });
});

// All images need to be loaded for this plugin to work so
// we end up waiting for the whole window to load in this example
$(window).load(function () {
    $(document).ready(function () {
        collage();
        $('.clg').collageCaption();
    });
});

// Here we apply the actual CollagePlus plugin
function collage() {
    $('.clg').removeWhitespace().collagePlus(
        {
            'fadeSpeed': 2000,
            'targetHeight': 200
            //'effect': 'effect-2',
            //'direction': 'vertical'
        }
    );
};

// This is just for the case that the browser window is resized
var resizeTimer = null;
$(window).bind('resize', function () {
    // hide all the images until we resize them
    $('.clg .clgWrapper').css("opacity", 0);
    // set a timer to re-apply the plugin
    if (resizeTimer) clearTimeout(resizeTimer);
    resizeTimer = setTimeout(collage, 200);
});