/* feather icons JS */
feather.replace()


/* data Table JS */
$(document).ready(function() {
	var t = $('#AllDataList').DataTable({
		"ordering": false,
		"searching": false,
		"paging": false,
		"info": false,
		
        "columnDefs": [ {
            "searchable": false,
            "orderable": false,
            "targets": 0
        } ],
        "order": [[ 1, 'asc' ]]
    } );
 
    t.on( 'order.dt search.dt', function () {
        t.column(0, {search:'applied', order:'applied'}).nodes().each( function (cell, i) {
            //cell.innerHTML = i+1;
        } );
    } ).draw();
    
	
	/* textarea word count */
	$("textarea").keyup(function () {
		var characterCount = $(this).val().length,
		current = $("#current"),
		maximum = $("#maximum"),
		theCount = $("#the-count");
		current.text(characterCount);
	});

	// toggle menu in small screen
	$(".menuToggleIcon").click(function () {
		$(".sidebar, .hideNav").toggleClass("active");
		$("body").toggleClass("overflow-hidden");
	});
	// hide nav
	$(".hideNav").click(function() {
		$(".sidebar, .hideNav").removeClass("active");
		$("body").removeClass("overflow-hidden");
	})
	
	// toggle filter on click 
	$("#filterPopup").click(function() {
		$(".filterMainBlock").toggleClass("active");
		$("body").toggleClass("overflow-hidden");
	});

	// remove active class from filter
	$(".filterMainBlock .filterContentBlock .btn-close, .shadowEffect").click(function() {
		$(".filterMainBlock").removeClass("active");
		$("body").removeClass("overflow-hidden");
	});

	// multiple value in range 
	const setLabel = (lbl, val) => {
		const label = $(`#slider-${lbl}-label`);
		label.text(val);
		const slider = $(`#slider-div .${lbl}-slider-handle`);
		const rect = slider[0].getBoundingClientRect();
		label.offset({
			top: rect.top - 30,
			left: rect.left
		});
	}
	const setLabels = (values) => {
		setLabel("min", values[0]);
		setLabel("max", values[1]);
	}
	//$('#range').slider().on('slide', function (ev) {
	//	setLabels(ev.value);
	//});
	//setLabels($('#range').attr("data-value").split(","));

	// multiple value in range end

});





