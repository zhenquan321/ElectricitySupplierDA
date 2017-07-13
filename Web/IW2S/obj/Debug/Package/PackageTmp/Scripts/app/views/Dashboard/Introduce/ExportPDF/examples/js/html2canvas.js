
var pdf = new jsPDF('p', 'pt', 'a4');

var x = window.parent.document.getElementById('dashboard');
//var x = document.getElementById("myHeader");

pdf.addHTML(x,function () {
	var string = pdf.output('datauristring');
	$('.preview-pane').attr('src', string);
});
