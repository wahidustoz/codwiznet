(function () {

    window.onscroll = function () { scrolledHandler() };
    function scrolledHandler() {
        var winScroll = document.body.scrollTop || document.documentElement.scrollTop;
        var height = document.documentElement.scrollHeight - document.documentElement.clientHeight;
        var scrolled = (winScroll / height) * 100;
        $("#scroll-progess-bar").width(scrolled + "%");
    }
})();