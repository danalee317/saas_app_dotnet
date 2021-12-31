﻿window.MFPortal = {
  SubmitForm: function (element) {
    element.submit();
  },
  LocalTime: function () {
    return new Date().getTimezoneOffset();
  },
  GoogleCaptcha: function (dotNetObject, selector, sitekeyValue) {
    return grecaptcha.render(selector, {
      'sitekey': sitekeyValue,
      'callback': (response) => { dotNetObject.invokeMethodAsync('CallbackOnSuccess', response); },
      'expired-callback': () => { dotNetObject.invokeMethodAsync('CallbackOnExpired'); }
    });
  },
  DisposeKendo: function destroyWidgets(container) {
    if (!this.EnsureKendo()) { return; }
    kendo.destroy(container);
  },
  EnsureKendo: function ensureKendoAndJquery() {
    if (!window.$ || !window.kendo) {
      alert("Something went wrong with loading the Kendo library, review the script references.");
      return false;
    }
    return true;
  },
  KendoIntialiase: function createWidget(container, id, dotNetComponent, initialColor) {
    if (!this.EnsureKendo()) { return; }
  
    this.ViewPDF($(container).find("#" + id), dotNetComponent, initialColor);
  },
  ViewPDF: function createPdfViewer($elem) {
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
}
