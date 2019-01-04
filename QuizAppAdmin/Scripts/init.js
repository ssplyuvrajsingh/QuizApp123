"use strict";
var sidebar;
$(function () {

    //Initialize icheck

    $(".icheck").each(function () {

        var boxcolor = 'grey';

        if ($(this).hasClass('icheck-green')) {
            boxcolor = 'green';
        }
        if ($(this).hasClass('icheck-red')) {
            boxcolor = 'red';
        }
        if ($(this).hasClass('icheck-blue')) {
            boxcolor = 'blue';
        }
        if ($(this).hasClass('icheck-aero')) {
            boxcolor = 'aero';
        }

        if ($(this).hasClass('icheck-orange')) {
            boxcolor = 'orange';
        }
        if ($(this).hasClass('icheck-yellow')) {
            boxcolor = 'yellow';
        }
        if ($(this).hasClass('icheck-pink')) {
            boxcolor = 'pink';
        }
        if ($(this).hasClass('icheck-purple')) {
            boxcolor = 'purple';
        }
        if ($(this).hasClass('icheck-grey')) {
            boxcolor = 'grey';
        }

        $(this).iCheck({
            checkboxClass: 'icheckbox_square-' + boxcolor,
            radioClass: 'iradio_square-' + boxcolor,

        });
    });
    // Adjust Form

    $('.md-form-group').each(function () {
        $(this).find('input').on('focus', function () {
            $(this).siblings('label').addClass('small');
        })
        $(this).find('input').blur(function () {
            if ($(this).val() == "") {
                $(this).siblings('label').removeClass('small');
            }

        })
    })


    // Initialize Ripple EFfect

    $('.btn, .main-nav > li > a, .navbar-nav > li > a').on('click', function (event) {

        var $div = $('<div/>');
        var offset = $(this).offset();
        var xPos = event.pageX - offset.left;
        var yPos = event.pageY - offset.top;



        $div.addClass('ripple-effect');
        var ripple = $(".ripple-effect");

        ripple.css("height", $(this).height());
        ripple.css("width", $(this).height());
        $div
            .css({
                top: yPos - (ripple.height() / 2),
                left: xPos - (ripple.width() / 2),
                background: $(this).data("ripple-color")
            })
            .appendTo($(this));

        window.setTimeout(function () {
            $div.remove();
        }, 2000);

    });


    // Initialize Bootstrap Switch

    $(".bootstrap-switch").bootstrapSwitch();



    // Initialize Scrolling

    $('.chat-messages').slimScroll({
        height: '100%',
        color: '#bbb',
        position: 'right',
        distance: '0px',
        start: 'bottom',
    });
    $(".slimScrollBar").hide()
    $('.conversations').slimScroll({
        height: '100%',
        color: '#bbb',
        position: 'right',
        distance: '0px',
        start: 'top',
    });
    $(".slimScrollBar").hide()

    // Initialize Sidebar and Topbar Fixed States

    $('.fixed-sidebar-toggle').attr('checked', true);
    $('.fixed-sidebar-toggle').bootstrapSwitch('state', true, true);
    $('.fixed-topbar-toggle').attr('checked', true);
    $('.fixed-topbar-toggle').bootstrapSwitch('state', true, true);

    $('.fixed-topbar-toggle').on('switchChange.bootstrapSwitch', function (e) {
        var ele = $(this);

        if (ele.is(':checked')) {
            $('#topbar').addClass('navbar-fixed-top');
            $('body').addClass('fixed-navbar-top');
            $('.fixed-sidebar-toggle').attr('disabled', false);
            $('.fixed-sidebar-toggle').bootstrapSwitch('disabled', false);

        } else {
            $('.fixed-sidebar-toggle').attr('checked', false);
            $('.fixed-sidebar-toggle').bootstrapSwitch('state', false);
            $('.fixed-sidebar-toggle').attr('disabled', true);
            $('.fixed-sidebar-toggle').bootstrapSwitch('disabled', true);
            $('#topbar').removeClass('navbar-fixed-top');
            $('body').removeClass('fixed-navbar-top');
            $('.page-wrapper').removeClass('fixed-navbar-top');
        }
    });

    $('.fixed-sidebar-toggle').on('switchChange.bootstrapSwitch', function (e) {
        var ele = $(this);

        if (ele.is(':checked')) {
            $('#sidebar').addClass('sidebar-fixed');
            sidebar.enableScrolling();
        } else {
            $('#sidebar').removeClass('sidebar-fixed');
            sidebar.disableScrolling();
        }
    })

    //Account Select box
    $("#accounts").change(function () {
        var val = $(this).val();
        window.location.href = val;
    });


    $("#main-nav > li > a").click(function () {
        console.log("here1");
        console.log($(this).closest("ul").hasClass("main-nav"));
        if ($(this).closest("ul").hasClass("main-nav")) {
            if ($(this).parent("li").children(".sub-menu").css("display") == "none") {
                $("#main-nav li .sub-menu").slideUp('slow');
                $(this).parent("li").children(".sub-menu").slideDown('fast');
            } else {
                $("#main-nav li .sub-menu").slideUp('slow');
                $("#main-nav > li > .sub-menu .mCSB_container").removeClass("double-height");
                $("#main-nav > li > .sub-menu").removeClass("double-height");
            }
        }
    });

    $("#main-nav > li ul li").click(function () {
        console.log("here");
        console.log($(this).children(".sub-menu").css("display"));
        if ($(this).children(".sub-menu").css("display") == "none") {
            $("#main-nav > li > .sub-menu .sub-menu").hide();
            $(this).parents(".sub-menu").addClass("double-height");
            $(this).parents(".mCSB_container").addClass("double-height");
            $(this).children(".sub-menu").show();
        } else {
            $("#main-nav > li > .sub-menu").removeClass("double-height");
            $("#main-nav > li > .sub-menu .mCSB_container").removeClass("double-height");
            $("#main-nav > li > .sub-menu .sub-menu").hide();
        }
    });

    $('#main-nav').click(function (event) {
        event.stopPropagation();
    });

    $("#main-nav > li .sub-menu").mCustomScrollbar({
        axis: "x",
        theme: "light-thick",
        advanced: { autoExpandHorizontalScroll: true }
    });

    //$(window).click(function () {
    //    $("#main-nav li .sub-menu").slideUp('slow');
    //    $("#main-nav > li > .sub-menu").removeClass("double-height");
    //    $("#main-nav > li > .sub-menu .mCSB_container").removeClass("double-height");
    //});

    $("#checkAll").click(function () {
        var check = $(this).is(":checked");
        if (check) {
            $(this).parents(".border-div").find(".other-checkbox").find(".chk-self").attr("checked", true);
        } else {
            $(this).parents(".border-div").find(".other-checkbox").find(".chk-self").attr("checked", false);
        }
    });

    
    $("#main-nav > li > .sub-menu").each(function () {
        var cur = $(this);
        
        var childrenlsits = $(this).find("ul").find("li");
        for (var i = 0; i < childrenlsits.length; i++) {

            var menuitem = $(childrenlsits[i]);
            if (isActiveItem(menuitem)) {
                cur.show();
            }
        }
    });
})

function isActiveItem(menuitem) {
    var i, anchor = menuitem.children("a"),
        submenu = menuitem.children(".sub-menu");
    if (submenu.length > 0) {
        
        var childrenlists = submenu.find("ul").children("li");
        for (i = 0; i < childrenlists.length; i++) {
            if (isActiveItem($(childrenlists[i]))) {
                console.log(submenu);
                return submenu.show(), submenu.parents(".sub-menu").find(".mCSB_container").addClass("double-height"), submenu.parents(".sub-menu").addClass("double-height"), !0;
            } 
        }
    } else if (anchor.length > 0 && 0 !== anchor.attr("href").indexOf("#")) {

        var link = anchor.prop("href");
        link = link.split("#")[0];

        var parts = link.split("/").slice(3),
            filepath = "/" + parts.join("/");
        if (filepath.toLowerCase() == window.location.pathname.toLowerCase())
            return !0;
        if (filepath + "/index.html" == window.location.pathname || filepath == window.location.pathname + "index.html")
            return !0;
    } return !1;
}

//"use strict"; $.fn.CollapsibleMenu =

//    function (options) {

//        function process(submenu) {
//            submenu.css("overflow-y", "hidden");

//            var calculatedheight = submenu[0].scrollHeight;
//            submenu.data("calculated-height", calculatedheight),
//                submenu.css("max-height", calculatedheight + "px"),
//                submenu.children("li").children("ul").each(function () {
//                    process($(this))
//                }),
//                submenu.hasClass(settings.expandClass) || collapse(submenu)
//        }

//        function collapse(submenu) {
//            var container = submenu.parent(); submenu.animate({ "max-height": 0 },
//                {
//                    duration: settings.animationTime, complete: function () {
//                        submenu.removeClass(settings.expandClass),
//                            container.removeClass(settings.collapsedContainerClass)
//                    }
//                })
//        }

//        function expand(submenu, firstexpand) {
//            var calculatedheight = submenu.data("calculated-height"), container = submenu.parent(); !0 !== firstexpand && submenu.parent().parent().find("li ul.open").each(function () { collapse($(this)) }), submenu.animate({ "max-height": calculatedheight + "px" }, { duration: settings.animationTime }), submenu.addClass(settings.expandClass), container.addClass(settings.collapsedContainerClass), container.addClass("background")
//        }


//        function detectCurrent(element) {
//            var i, childrenlsits = element.children("li");



//            for (i = 0; i < childrenlsits.length; i++) {

//                var menuitem = $(childrenlsits[i]);
//                if (isActive(menuitem))
//                    return menuitem.addClass("active"), !0
//            }

//        }


//        function isActive(menuitem) {
//            var i, anchor = menuitem.children("a"),
//                submenu = menuitem.children("ul");
//            if (submenu.length > 0) {
//                var childrenlists = submenu.children("li");
//                for (i = 0; i < childrenlists.length; i++) {
//                    if (isActive($(childrenlists[i])))
//                        return submenu.addClass("open"), !0
//                }
//            } else if (anchor.length > 0 && 0 !== anchor.attr("href").indexOf("#")) {

//                var link = anchor.prop("href");
//                link = link.split("#")[0];

//                var parts = link.split("/").slice(3),
//                    filepath = "/" + parts.join("/");
//                if (filepath == window.location.pathname)
//                    return menuitem.addClass("active"), !0;
//                if (filepath + "/index.html" == window.location.pathname || filepath == window.location.pathname + "index.html")
//                    return menuitem.addClass("active"), !0
//            } return !1
//        }


//        var settings = $.extend({
//            animationTime: 300,
//            expandClass: "open",
//            collapsedContainerClass: "active",
//            autoDetectActive: !1
//        }, options);

//        this.each(function () {
//            var element = $(this);
//            settings.autoDetectActive && detectCurrent(element),
//                element.children("li").children("ul").each(function () {
//                    process($(this))
//                }),

//                element.on("click", "li a", function (event) {
//                    $(this).blur();
//                    var submenu = $(this).siblings("ul");
//                    submenu.length > 0 && (event.preventDefault(), submenu.hasClass(settings.expandClass) ? collapse(submenu) : expand(submenu))
//                })
//        })
//    };