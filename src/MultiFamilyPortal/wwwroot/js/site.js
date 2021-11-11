window.MFPortal = {
  SubmitForm: function (element) {
    element.submit();
  },
  LocalTime: function () {
    return new Date().getTimezoneOffset();
  },
}
