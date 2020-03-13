function isNumeric(k) {
    if (k.keyCode >= 96 && k.keyCode <= 105)
        return true;
    if (k.keyCode >= 48 && k.keyCode <= 57)
        return true;
    if (k.keyCode == 116 || k.keyCode == 37 || k.keyCode == 39 || k.keyCode == 9 || k.keyCode == 8 || k.keyCode == 46 || k.keyCode == 13)
        return true;
    return false;
}

$(document).ready(
    function () {
        $(':input').focus(function () { $(this).removeClass('input-validation-error') });
        $('.onlyNumbers').keydown(isNumeric);
    }
);