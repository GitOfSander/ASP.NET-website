var revapi,
    $PreviousSlide,
    sliderHideItems;

$(document).ready(function () {
    //if (getCookie("sliderHiddenItems") === "")
    //{
    //    setCookie("sliderHiddenItems", "[]", 365);
    //}

    //sliderHideItems = $.parseJSON(getCookie("sliderHiddenItems"));
    //$.each(sliderHideItems, function (index, value) {
    //    $(".sd-intro .tp-banner li[data-id='" + value.id + "']").remove();
    //});

    revapi = $('.sd-intro .tp-banner').revolution(
    {
         delay: 9000,
         startwidth: 1170,
         startheight: 500,
         fullScreen: "on",
         forceFullWidth: "on",
         minFullScreenHeight: "320",
         //videoJsPath: "rs-plugin/videojs/",
         fullScreenOffsetContainer: "header",
         lazyLoad: "on",
         lazyType: "all",
         navigationType: "none"
    });

    //revapi.on("revolution.slide.onchange", function (e, data) {
    //    if ($(".sd-intro .tp-banner li[data-index='" + $PreviousSlide + "']").data("one-time"))
    //    {
    //        var id = $(".sd-intro .tp-banner li[data-index='" + $PreviousSlide + "']").data("id");
    //        sliderHideItems = $.parseJSON(getCookie("sliderHiddenItems"));
    //        var result = $.grep(sliderHideItems, function (e) { return e.id === id; });

    //        if (result.length === 0)
    //        {
    //            sliderHideItems.push(
    //                { 'id': $(".sd-intro .tp-banner li[data-index='" + $PreviousSlide + "']").data("id") }
    //            );
    //            setCookie("sliderHiddenItems", JSON.stringify(sliderHideItems), 365);
    //        }

    //        //$(".sd-intro .tp-banner li[data-index='" + $PreviousSlide + "']").find("video").remove();
    //        //$(".sd-intro .tp-banner li[data-index='" + $PreviousSlide + "'] .sd-ol").removeAttr("style");
    //    }
        
    //    $PreviousSlide = data.slideIndex;
    //    //if ($(".sd-intro .tp-banner li[data-index='" + data.slideIndex + "']").attr("data-hide"))
    //    //{
    //    //}
    //});

    $(".sd-intro #next").on("click", function () {
        $('.sd-intro .tp-banner').revnext();
    });

    $(".sd-intro #previous").on("click", function () {
        $('.sd-intro .tp-banner').revprev();
    });

    sr.reveal('#introText', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    sr.reveal('#projectText', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    $("#slickSlider").slick({
        nextArrow: '<button type="button" data-role="none" class="slick-next slick-arrow" aria-label="Next" role="button">Next<svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 3.8 7" style="enable-background:new 0 0 3.8 7;" xml:space="preserve"><polyline class="st0" points="0.2,0.2 3.5,3.5 0.2,6.8 " /></svg></button>',
        prevArrow: '<button type="button" data-role="none" class="slick-prev slick-arrow" aria-label="Previous" role="button"><svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 3.8 7" style="enable-background:new 0 0 3.8 7;" xml:space="preserve"><polyline class="st0" points="3.7,6.8 0.4,3.5 3.7,0.2 " /></svg></button>',
        dots: false,
        infinite: true,
        loop: true,
        slidesToShow: 8,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 6000,
        lazyLoad: 'ondemand',
        responsive: [
            {
                breakpoint: 3370,
                settings: {
                    slidesToShow: 7
                }
            },
            {
                breakpoint: 2900,
                settings: {
                    slidesToShow: 6
                }
            },
            {
                breakpoint: 2400,
                settings: {
                    slidesToShow: 5
                }
            },
            {
                breakpoint: 2000,
                settings: {
                    slidesToShow: 4
                }
            },
            {
                breakpoint: 1100,
                settings: {
                    slidesToShow: 3
                }
            },
            {
                breakpoint: 950,
                settings: {
                    slidesToShow: 2
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1
                }
            }
        ]
    });
});

//function setCookie(cname, cvalue, exdays) {
//    var d = new Date();
//    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
//    var expires = "expires=" + d.toUTCString();
//    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
//}

//function getCookie(cname) {
//    var name = cname + "=";
//    var decodedCookie = decodeURIComponent(document.cookie);
//    var ca = decodedCookie.split(';');
//    for (var i = 0; i < ca.length; i++) {
//        var c = ca[i];
//        while (c.charAt(0) === ' ') {
//            c = c.substring(1);
//        }
//        if (c.indexOf(name) === 0) {
//            return c.substring(name.length, c.length);
//        }
//    }
//    return "";
//}