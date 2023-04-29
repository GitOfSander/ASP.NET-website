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
    
    sr.reveal('#contactText', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    sr.reveal('#contactText2', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    sr.reveal('#contactQuote', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px',
        origin: "left"
    });

    sr.reveal('#contactForm', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    sr.reveal('#contactAddresses', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px'
    });

    $("#send").click(function () {
        $(this).prop("disabled", true);
        $("#form").submit();
    });

    //$.validator.addMethod('customphone', function (value, element) {
    //    return this.optional(element) || /^\d{3}-\d{3}-\d{4}$/.test(value);
    //}, "Dit is geen juist telefoonnummer");

    var validator = $("#form").validate({
		rules: {
		    name: "required",
		    phonenumber: "number",
			email: {
			    required: true,
			    email: true
			},
			message: "required"
		},
		messages: JSON.stringify({
		    name: "Vul uw naam in.",
		    phonenumber: "Dit is geen juist telefoonnummer. vul hier alleen cijfers in.",
			email: {
			    required: "Vul uw e-mailadres in.",
			    email: "Dit is geen juist e-mailadres."
			},
			message: "Vul uw bericht in."
		}),
		errorElement: 'div',
		errorClass: 'cf-alert',
		errorPlacement: function(error, element) {
		    error.appendTo( element.parent("div") );
		},
		submitHandler: function (form) {
            $.ajax({
                url: '/spine-api/contactform',
                type: 'POST',
                dataType: 'json',
                contentType: "application/json;charset=utf-8",
                data: JSON.stringify({
                    name: $(form).find("#name").val(),
                    email: $(form).find("#email").val(),
                    phonenumber: $(form).find("#phonenumber").val(),
                    message: $(form).find("#message").val()
                }),
                success: function (data) {
                    if (!data.success) {
                        if (typeof data.errors.name !== "undefined") {
                            validator.showErrors({ "name": data.errors.name });
                        }
                        if (typeof data.errors.email !== "undefined") {
                            validator.showErrors({ "email": data.errors.email });
                        }
                        if (typeof data.errors.phonenumber !== "undefined") {
                            validator.showErrors({ "phonenumber": data.errors.phonenumber });
                        }
                        if (typeof data.errors.message !== "undefined") {
                            validator.showErrors({ "message": data.errors.message });
                        }
                        if (typeof data.errors.error !== "undefined") {
                            $(form).find("#cfResult").html(data.errors.error).addClass("cf-error");
                        }

                        $(form).find("#send").prop("disabled", false);
                    } else {
                        if (typeof data.errors === "undefined") {
                            console.log(data.result);
                            $(form).find("#cfResult").html(data.result).removeClass("cf-error");
                            $(form).find("input[type=text], input[type=email], input[type=tel], textarea").val("");
                        }
                    }
                },
                error: function () {
		        }
		    });
		}
	});

    BuildMaps();
});

$(window).resize(function () {
    BuildMaps();
});

function BuildMaps() {
    //set your google maps parameters
    var latitude = 51.6304736,
        longitude = 4.8826588,
        map_zoom = 16;

    //google map custom marker icon - .png fallback for IE11
    var is_internetExplorer11 = navigator.userAgent.toLowerCase().indexOf('trident') > -1;


    //define the basic color of your map, plus a value for saturation and brightness
    var main_color = '#f7f7f7';

    //we define here the style of the map
    var style = [
    {
        "featureType": "administrative",
        "elementType": "labels.text.fill",
        "stylers": [
            {
                "color": "#444444"
            }
        ]
    },
    {
        "featureType": "landscape",
        "elementType": "all",
        "stylers": [
            {
                "color": "#f2f2f2"
            }
        ]
    },
    {
        "featureType": "poi",
        "elementType": "all",
        "stylers": [
            {
                "visibility": "off"
            }
        ]
    },
    {
        "featureType": "road",
        "elementType": "all",
        "stylers": [
            {
                "saturation": -100
            },
            {
                "lightness": 45
            }
        ]
    },
    {
        "featureType": "road.highway",
        "elementType": "all",
        "stylers": [
            {
                "visibility": "simplified"
            }
        ]
    },
    {
        "featureType": "road.arterial",
        "elementType": "labels.icon",
        "stylers": [
            {
                "visibility": "off"
            }
        ]
    },
    {
        "featureType": "transit",
        "elementType": "all",
        "stylers": [
            {
                "visibility": "off"
            }
        ]
    },
    {
        "featureType": "water",
        "elementType": "all",
        "stylers": [
            {
                "color": "#46bcec"
            },
            {
                "visibility": "on"
            }
        ]
    }
    ];

    //set google map options
    var map_options = {
        center: new google.maps.LatLng(latitude, longitude),
        zoom: map_zoom,
        panControl: false,
        zoomControl: false,
        mapTypeControl: false,
        streetViewControl: false,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        scrollwheel: false,
        styles: style
    };
    //inizialize the map
    var map = new google.maps.Map(document.getElementById('googlemaps'), map_options);

    //add a custom marker to the map				
    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(latitude, longitude),
        map: map,
        visible: true,
        optimized: false,
        icon: {
            anchor: new google.maps.Point(50, 25),
            url: '/spine-content/svg/location_icon.svg',
            scaledSize: new google.maps.Size(50, 50)
        }
    });
}