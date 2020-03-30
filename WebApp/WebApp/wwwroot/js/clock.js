$(document).ready(() => {
	$("#menu-toggle").click(function (e) {
		e.preventDefault();
		$("#wrapper").toggleClass("toggled");
	});
	setInterval(function () {
		var d = new Date();

		var month = d.getMonth() + 1;
		var day = d.getDate();
		var time = `${(d.getHours() < 10 ? '0' : '')}${d.getHours()}:${(d.getMinutes() < 10 ? '0' : '')}${d.getMinutes()}:${(d.getSeconds() < 10 ? '0' : '')}${d.getSeconds()}`;
		var output = (('' + day).length < 2 ? '0' : '') + day + '/' + (('' + month).length < 2 ? '0' : '') + month + '/' + d.getFullYear();
		$(".nav-date").text(time + " " + output);
	}, 1000);

});