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
    
    sr.reveal('#teamQuote', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px',
        origin: "left"
    });

    sr.reveal('#teamText', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    sr.reveal('.tmItem:nth-of-type(odd)', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    sr.reveal('.tmItem:nth-of-type(even)', {
        duration: 1600,
        delay: 250,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    $("#bn").parallax({
        image: $("#bn").data("background"),
        scroll: "manual"
    });
});