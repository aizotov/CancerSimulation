!function ($) {
	$(function(){
  		// carousel demo
  		$('#myCarousel').carousel()

		$("#documentationDownloadBtn").click(function( event ) {
	      	window.location.href = 'docs/CancerSimulationDescription.pdf';
	        event.preventDefault();
	   	});

		$("#usageInstructionsDownloadBtn").click(function( event ) {
	      	window.location.href = 'docs/Instructions.pdf';
	        event.preventDefault();
	   	});

	   	$("#exampleCancersDownloadBtn").click(function( event ) {
	      	window.location.href = 'docs/ExampleCancers.pdf';
	        event.preventDefault();
	   	});
	})
}(window.jQuery)