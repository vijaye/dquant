(function () {
  if (!window.Object) {
    window.Object = {};
  }

  if (!window.Object.create) {
    window.Object.create = function (obj) {
      var newObj = {};
      for (var key in obj) {
        newObj[key] = obj[key];
      }

      return newObj;
    };
  }
})();