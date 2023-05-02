$(document).ready(function () {
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
                breakpoint: 4000,
                settings: {
                    slidesToShow: 6
                }
            },
            {
                breakpoint: 3400,
                settings: {
                    slidesToShow: 5
                }
            },
            {
                breakpoint: 2650,
                settings: {
                    slidesToShow: 4
                }
            },
            {
                breakpoint: 2000,
                settings: {
                    slidesToShow: 3
                }
            },
            {
                breakpoint: 800,
                settings: {
                    slidesToShow: 1
                }
            }
        ]
    });

    $("#slickFilter li").on("click", function () {
        var filter = $(this).data("filter");
        $("#slickFilter li").removeClass("active");
        $(this).addClass("active");
        $('#slickSlider').slick('slickUnfilter');

        if (filter !== "*") {
            $('#slickSlider').slick('slickFilter', filter);
        }
    });

    CheckHash();

    $(window).on('hashchange', function (e) {
        CheckHash()
    });
});

function CheckHash() {
    var param = document.URL.split('#')[1];
    console.log($('#slickSlider'));
    if (param !== null && param !== undefined) {
        $('#slickSlider').slick('slickUnfilter');
        $("#slickFilter li").removeClass("active");
        $("#slickFilter").find("li[data-title='" + param + "']").addClass("active");
        $('#slickSlider').slick('slickFilter', $("#slickFilter").find("li[data-title='" + param + "']").data("filter"));
    }
}

function CreateFriendlyUrl(i, a, p) {
    var $Divider = "-";
    var $Url = $(i).val()
                   .toLowerCase()
                   .replace(/^\s+|\s+$/g, $Divider)
                   .replace(/[_|\s]+/g, $Divider)
                   .replace(/[^a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð\u0400-\u04FF0-9-]+/g, "")
                   .replace(/[-]+/g, $Divider);

    $(i).parents(p).find(a).val($Url);
}