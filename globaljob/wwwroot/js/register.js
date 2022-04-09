$(document).ready(function () {
    if ($('#profiles :selected').text() === 'Recruteur') {
        $("#name").text("Compony Name");
        $('#LastN').hide();
        $('#DateB').hide();
    }

    $("select#profiles").change(function () {
        if ($(this).find("option:selected").text() === 'Recruteur') {
            $("#name").text("Compony Name");
            $('#LastN').hide();
            $('#DateB').hide();
        }
        else {
            $("#name").text("First Name");
            $('#LastN').show();
            $('#DateB').show();
        }

    });
});
