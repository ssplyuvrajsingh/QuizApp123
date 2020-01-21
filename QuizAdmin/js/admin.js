var admin = {
    LoadPartialView: function (partail) {
        console.log(adminController + partail);
        doAjax(adminController + partail, null, function (data) {
            $('#main-content').html(data);
        });
    },
};