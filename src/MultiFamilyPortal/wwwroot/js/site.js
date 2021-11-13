window.MFPortal = {
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
  }
}
