function destroyWidgets(container) {
	if (!ensureKendoAndJquery()) { return; }
	kendo.destroy(container);
}

function ensureKendoAndJquery() {
	if (!window.$ || !window.kendo) {
		alert("something went wrong with loading the Kendo library, review the script references");
		return false;
	}
	return true;
}

function createWidget(functionName, container, id, dotNetComponent, initialColor) {
	if (!ensureKendoAndJquery()) { return; }

	window[functionName]($(container).find("#" + id), dotNetComponent, initialColor);
}

function createPdfViewer($elem) {
	$.when(
		$.getScript("https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.2.2/pdf.js"),
		$.getScript("https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.2.2/pdf.worker.js")
	).done(function () {
		window.pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.2.2/pdf.worker.js';
	}).then(function () {
		$elem.kendoPDFViewer({
			pdfjsProcessing: {
				file: $elem[0].dataset.filename
			},
			width: $elem[0].dataset.width,
			height: $elem[0].dataset.height
		}).data("kendoPDFViewer");
	});
}