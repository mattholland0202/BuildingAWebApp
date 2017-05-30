/* Put all JS code in an immediately invoked function expression (AKA self-executing anonymous function) so that variables etc aren't added to global scope */
(function () {
    //var ele = $("#username");
    //ele.text("MyNewName");

    //var main = $("#main");
    //main.on("mouseenter", function () {
    //    main.style = "background-color: #888;";
    //});
    //main.on("mouseleave", function () {
    //    main.style.background = "";
    //});

    //var menuItems = $("ul.menu li a"); // The anchor inside a listItem inside a menu for an unordered list
    //menuItems.on("click", function () {
    //    var me = $(this);
    //    alert(me.text());
    //})

    var $sidebarAndWrapper = $("#sidebar,#wrapper"); // returns a "wrapped set" of DOM elements
    var $icon = $("#sidebarToggle i.fa"); // the "i" is a child of it, classed with fa

    $("#sidebarToggle").on("click", function () {
        $sidebarAndWrapper.toggleClass("hide-sidebar");
        if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
            $icon.removeClass("fa-angle-left");
            $icon.addClass("fa-angle-right");
        } else {
            $icon.removeClass("fa-angle-right");
            $icon.addClass("fa-angle-left");
        }
    });
    
})();