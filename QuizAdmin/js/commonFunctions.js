var adminController = "/home/";

function doAjax(url, data, successCallback) {
    $.ajax({
        type: "Get",
        data: data,
        url: url,
        success: function (data) {
            successCallback(data);
        },
        error: function (data) {
            alert("Some unexpected error ocuured");
        }

    });
}

function doAjaxPost(url, data, successCallback) {
    $.ajax({
        type: "POST",
        data: data,
        url: url,
        success: function (data) {
            successCallback(data);
        },
        error: function (data) {
            alert("error occured");
        }

    });
}

function addDatepicker() {
    //changeDateFormat();
    $(".datepicker").datetimepicker();
}

function changeDateFormat() {

    $(".datepicker-input").each(function () {
        var txt = $(this).val();

        try {
            if (txt != "") {
                var d = new Date(txt);
                var newDate = (d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear();
                $(this).val(newDate);
            }
        }
        catch (err) {
            $(this).val(txt);
        }
    });
}

function changeurl(url) {
    $(url).attr("src", "/Images/NoPhotoAvailable.jpg");
}
$(function () {
    handleLoading();
});

function handleLoading() {


    $(document).ajaxStart(function () {
        $('#loader-container').show();
    });
    $(document).ajaxStop(function () {
        $('#loader-container').hide();
    });
}