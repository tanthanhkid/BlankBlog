(function($) {
	"use strict"

	// Mobile dropdown
	$('.has-dropdown>a').on('click', function() {
		$(this).parent().toggleClass('active');
	});

	// Aside Nav
	$(document).click(function(event) {
		if (!$(event.target).closest($('#nav-aside')).length) {
			if ( $('#nav-aside').hasClass('active') ) {
				$('#nav-aside').removeClass('active');
				$('#nav').removeClass('shadow-active');
			} else {
				if ($(event.target).closest('.aside-btn').length) {
					$('#nav-aside').addClass('active');
					$('#nav').addClass('shadow-active');
				}
			}
		}
	});

	$('.nav-aside-close').on('click', function () {
		$('#nav-aside').removeClass('active');
		$('#nav').removeClass('shadow-active');
	});


	$('.search-btn').on('click', function() {
		$('#nav-search').toggleClass('active');
	});

	$('.search-close').on('click', function () {
		$('#nav-search').removeClass('active');
    });

    ////Trang chu load more
    //$("#loadmore-button").click(function () {
       
    //    var lastIndex = $("#loadmore-lastindex").val();
    //    var to = parseInt(lastIndex) + 10;
    //    var authorizationToken = $("#antiforgerytoken-" + lastIndex).val();
    //    $.ajax({
    //        url: "/Home/AllPost",
    //        data: { from: lastIndex, to:to },
    //        type: "GET",
    //        beforeSend: function (xhr) { xhr.setRequestHeader('RequestVerificationToken', authorizationToken); },
    //        success: function () { alert('Success!' + authHeader); }
    //    });


    //});

 
})(jQuery);
