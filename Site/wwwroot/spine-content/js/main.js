$(document).ready(function () {
    $(window).on("scroll", function () {
        var scroll = $(window).scrollTop();

        if (scroll > 0) {
            $("#navbar").addClass("nb-scroll");
        } else {
            $("#navbar").removeClass("nb-scroll");
        }
    }).trigger("scroll");

    //unsubText

    window.sr = ScrollReveal();
    sr.reveal('#cladsafeQuote', {
        duration: 1600,
        opacity: 0,
        scale: 1,
        distance: '70px',
        origin: "left"
    });

    $('[data-parallax]').jQueryParallax();

    $("#addEmail").click(function () {
        $(this).prop("disabled", true);
        $("#mail").submit();
    });

    var validator = $("#mail").validate({
        rules: {
            name: "required",
            email: {
                required: true,
                email: true
            },
        },
        messages: JSON.stringify({
            name: "Vul uw naam in.",
            email: {
                required: "Vul uw e-mailadres in.",
                email: "Dit is geen juist e-mailadres."
            },
        }),
        errorElement: 'div',
        errorClass: 'cf-alert',
        errorPlacement: function (error, element) {
            error.appendTo(element.parent("div"));
        },
        submitHandler: function (form) {
            $.ajax({
                url: '/spine-api/newspaper',
                type: 'POST',
                dataType: 'json',
                contentType: "application/json;charset=utf-8",
                data: JSON.stringify({
                    name: $(form).find("#name").val(),
                    email: $(form).find("#email").val()
                }),
                success: function (data) {
                    if (!data.success) {
                        if (typeof data.errors.name !== "undefined") {
                            validator.showErrors({ "name": data.errors.name });
                        }
                        if (typeof data.errors.email !== "undefined") {
                            validator.showErrors({ "email": data.errors.email });
                        }
                        if (typeof data.errors.error !== "undefined") {
                            $(form).find("#cfResult").html(data.errors.error).addClass("cf-error");
                        }

                        $(form).find("#addEmail").prop("disabled", false);
                    } else {
                        if (typeof data.errors === "undefined") {
                            console.log(data.result);
                            $(form).find("#cfResult").html(data.result).removeClass("cf-error");
                            $(form).find("input[type=text], textarea").val("");
                        }
                    }
                },
                error: function () {
                }
            });
        }
    });

    var unsubcribeValidator = $('form#unsub').validate({
        rules: {
            email: {
                required: true,
                email: true
            }
        },
        messages: {
            email: {
                required: 'Vul uw e-mailadres in.',
                email: 'Het ingevoerde e-mailadres is onjuist.'
            }
        },
        errorElement: 'div',
        errorClass: 'cf-alert',
        errorPlacement: function (error, element) {
            element.parents('.modal-dialog').find('.cf-result').empty();
            error.appendTo(element.parents('.modal-dialog').find('.cf-result'));
        },
        unhighlight: function (element, errorClass, validClass) {
            var parent = $('#' + element.id).parents('.modal-dialog');
            if (parent.find('label.error:visible').length === 0) {
                parent.find('label.error').remove();
            }
        },
        success: function (error, element) {
            var parent = $(element.id).parents('.modal-dialog');
            error.remove();
            if (parent.find('label.error:visible').length === 0) {
                parent.find('label.error').remove();
            }
        },
        submitHandler: function (form) {
            $.ajax({
                url: '/spine-api/unsubscribe-newspaper',
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json;charset=utf-8',
                data: JSON.stringify({
                    email: $(form).find('#email').val()
                }),
                success: function (data) {
                    if (!data.success) {
                        if (typeof data.errors.email !== 'undefined') {
                            validator.showErrors({ 'email': data.errors.email });
                            $(form).find('input').val('').css('border-color', '#ff0000');
                            $(form).find('.svg').css('background-color', '#fee8ea');
                            $(form).find('.svg svg').css('fill', '#ff0000');
                        }
                        if (typeof data.errors.error !== 'undefined') {
                            $(form).find('#cfResult').html(data.errors.error).addClass('cf-error');
                            $(form).find('input').val('').css('border-color', '#00c887');
                            $(form).find('.svg').css('background-color', '#e6f9f5');
                            $(form).find('.svg svg').css('fill', '#00c887');
                        }

                        $(form).find('#unsubEmail').prop('disabled', false);
                    } else {
                        if (typeof data.errors === 'undefined') {
                            $(form).find('#cfResult').html(data.result).removeClass('cf-error');
                            $(form).find('input').val('').css('border-color', '#00c887');
                            $(form).find('.svg').css('background-color', '#e6f9f5');
                            $(form).find('.svg svg').css('fill', '#00c887');
                        }
                    }
                },
                error: function () {
                }
            });
        }
    });

    $('#unsubModal').on('shown.bs.modal', function (e) {
        $('#unsubModal').find('input').val('').removeAttr('style');
        $('#unsubModal').find('.svg').removeAttr('style');
        $('#unsubModal').find('.svg svg').removeAttr('style');
        $('#unsubModal').find('.cf-result').empty();

        unsubcribeValidator.resetForm();
    })

    $("#unsubEmail").click(function () {
        $(this).prop("disabled", true);
        $("#unsub").submit();
    });

    $('#unsubText a').on('click', function () {
        $('#unsubModal').modal('show');
    });

    CheckMainHash();

    $(window).on('hashchange', function (e) {
        CheckMainHash()
    });

    $(window).on('resize', function () {
        $('.modal-dialog').each(function (index) {
            if ($(this).height() >= $(window).height()) {
                $(this).removeClass('vertical-center-modal');
            } else {
                $(this).addClass('vertical-center-modal');
            }
        });
    }).trigger('resize');
});

function CheckMainHash() {
    var param = document.URL.split('#')[1];

    if (param !== null && param !== undefined) {
        if (param === 'uitschrijven') {
            $('#unsubModal').modal('show');
        }
    }
}