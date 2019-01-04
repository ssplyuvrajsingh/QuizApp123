"use strict";
/**
* CollpasibleSlider
* This is release of CollpasibleSlider is to be used only within the Emphasize Admin Template
* Collapsible Slider is a jQuery Plugin to generate collapsible Sliders
*/
$.fn.collpasibleSlider = function(options) {

    var element = this;
    element.addClass('collpasibleSlider');
    var settings = $.extend({
        //Default Settins
        miniClass: 'sidebar-mini',
        miniBodyClass: 'sidebar-toggled-mini',
        visibleClass:'sidebar-visible',
        visibleBodyClass: 'sidebar-toggled-visible',
        bodyClasses: true,
        toggleButtons:null,
        hideButtons:null,
        collapse: false,
        expand: false,
        hide: false,
        show:false,

    },options);
    if(settings.toggleButtons !== null && settings.toggleButtons !== undefined) {
        registerToggleButton(settings.toggleButtons);
    }
    if(settings.hideButtons !== null && settings.hideButtons !== undefined) {
        registerHideButton(settings.hideButtons);
    }
    if(settings.collapse) {
        collapse(element);
    }
    if(settings.expand) {
        expand(element);
    }
    if(settings.hide) {
        hide(element);
    }
    if(settings.show) {
        show(element);
    }
    function collapse(element){
        element.addClass(settings.miniClass);
        if(settings.bodyClasses)
        $('body').addClass(settings.miniBodyClass);
        disableScrolling(element);
    }
    function disableScrolling(element) {
        element.find('.sidebar-scrollable-content').slimScroll({
            destroy:true,
        });
        element.find(".sidebar-scrollable-content").css('overflow','visible');
    }
    function enableScrolling(element) {
        element.find('.sidebar-scrollable-content').slimScroll({
            height: '100vh',
            color: '#bbb',
            position: 'right',
            distance: '0px'
        });
        element.find(".slimScrollBar").hide()
    }
    function expand(element) {
        element.removeClass(settings.miniClass);
        if(settings.bodyClasses)
        $('body').removeClass(settings.miniBodyClass);
        enableScrolling(element);
    }

    function hide(element) {
        element.removeClass(settings.visibleClass);
        if(settings.bodyClasses)
        $('body').removeClass(settings.visibleBodyClass);

    }
    function show(element) {
        element.addClass(settings.visibleClass);
        if(settings.bodyClasses)
        $('body').addClass(settings.visibleBodyClass);
        enableScrolling(element);

    }
    function toggle(element) {

        if(element.hasClass(settings.miniClass)){
            expand(element);
        } else {
            collapse(element);
        }
    }
    function toggleCollapse(element) {

        if(element.hasClass(settings.visibleClass)){
            hide(element);

        } else {
            show(element);
        }
    }

    function registerToggleButton(selector){
        $(selector).on('click', function(){
            toggle(element);
            $(selector).toggleClass('active');
        })
    }

    function registerHideButton(selector){
        $(selector).on('click',function(){
            toggleCollapse(element);
            $(selector).toggleClass('active');
        })
    }


}

$(function(){

    // Initialize Scrolling

    $('.sidebar-scrollable-content').slimScroll({
        height: '100vh',
        color: '#bbb',
        position: 'right',
        distance: '0px'
    });
    $(".slimScrollBar").hide()

    //Initialize Sidebar Switching

    $('.sidebar.sidebar-left').collpasibleSlider({
        toggleButtons:'.navbar-sidebar-toggle',
        hideButtons:'.navbar-sidebar-collapse',

    });

    if($(window).width() < 990) {
        $('.sidebar.sidebar-left').collpasibleSlider({
            collapse:true,
        });
    }

    $(window).on('resize', function(){

        if($(window).width() < 990) {
            $('.sidebar.sidebar-left').collpasibleSlider({
                collapse:true,
            });
        }
        if($(window).width() > 1000) {
            $('.sidebar.sidebar-left').collpasibleSlider({
                expand:true,
            });
        }
    });

    // Initialize CollapsibleMenu
    $('#main-nav').CollapsibleMenu({autoDetectActive: true});

})
