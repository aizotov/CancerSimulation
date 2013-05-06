$(function () {
	$( "#tabs" ).tabs().addClass( "ui-tabs-vertical ui-helper-clearfix" );
    $( "#tabs li" ).removeClass( "ui-corner-top" ).addClass( "ui-corner-right");

    $( "button" ).button();

	$("#downloadDescription").click(function( event ) {
      	window.location.href = 'docs/CancerSimulationDescription.pdf';
        event.preventDefault();
   	});

	$("#downloadInstructions").click(function( event ) {
      	window.location.href = 'docs/Instructions.pdf';
        event.preventDefault();
   	});

   	$("#downloadExampleCancers").click(function( event ) {
      	window.location.href = 'docs/ExampleCancers.pdf';
        event.preventDefault();
   	});
});



