$(document).ready(
    function () {
        $(":input").focus(function () {
            $(this).removeClass("input-validation-error")
        });
    }
);