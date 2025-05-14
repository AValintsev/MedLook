$(function () {

	$(".collapsible-box > .btn").on('click', ($evt) => {
		$('.collapsible-box--body').toggleClass('open');
	});

});