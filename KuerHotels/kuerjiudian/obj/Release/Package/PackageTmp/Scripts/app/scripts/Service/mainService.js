myApp.service('foo', function () {

    this.getPanel = function () {
        $('.fa-minus').click(function () {
            var cardContent = $(this).parent().parent().parent('.main-panel').children('.main-panel-content');
            if (cardContent.hasClass('active')) {
                $(this).addClass('fa-minus');
                $(this).removeClass('fa-plus');
                $(cardContent).removeClass('active');
            } else {
                $(this).removeClass('fa-minus');
                $(this).addClass('fa-plus');
                $(cardContent).addClass('active');
            }
            cardContent.toggle(500);
        });

    };


   
});