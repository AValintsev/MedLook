if (document.querySelector('.glider')) {
	new Glider(document.querySelector('.glider'), {
		slidesToShow: 1,
		dots: '#dots',
		draggable: true,
		scrollLock: true,
		arrows: {
			prev: '.glider-prev',
			next: '.glider-next'
		},
		responsive: [
			{
				breakpoint: 1200,
				settings: {
					slidesToShow: 3,
					slidesToScroll: 1
				}
			},
			{
				breakpoint: 900,
				settings: {
					slidesToShow: 2,
					slidesToScroll: 1
				}
			},
			{
				breakpoint: 475,
				settings: {
					slidesToShow: 1,
					slidesToScroll: 1
				}
			}
		]
	});
}