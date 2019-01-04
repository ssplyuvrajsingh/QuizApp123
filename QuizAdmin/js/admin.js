var admin = {
    LoadPartialView: function (partail) {
        doAjax(adminController + partail, null, function (data) {
            $('#main-content').html(data);
        });
    },
};