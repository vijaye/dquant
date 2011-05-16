(function () {
  function arrayContains(array, value) {
    for (var ii = 0, len = array.length; ii < len; ii++) {
      if (array[ii] === value) {
        return true;
      }
    }

    return false;
  }

  window.DataObject = function (value) {
    this.handlers = [];
    this._value = value;
    this.isDisposed = false;
  };

  DataObject.prototype.coalesce = function (oldDO) {
    oldDO.setValue(this.getValue());
    for (var ii = 0, len = oldDO.handlers.length; ii < len; ii++) {
      var handler = oldDO.handlers[ii];
      if (!arrayContains(this.handlers, handler)) {
        this.handlers.push(handler);
      }
    }

    oldDO.dispose();
  }

  DataObject.prototype.dispose = function () {
    this.handlers = [];
    this.isDisposed = true;
  }

  DataObject.prototype.getValue = function () {
    return this._value;
  }

  DataObject.prototype.setValue = function (value) {
    var oldValue = this._value;
    this._value = value;

    var that = this;
    var arg = { oldValue: oldValue, newValue: value };
    for (var i = 0, len = this.handlers.length; i < len; i++) {
      var f = that.handlers[i];
      try {
        f(that, arg);
      } catch (ex) {
        // TODO: ?
      }
    }
    return this;
  }

  DataObject.prototype.subscribe = function (handler) {
    if (typeof (handler) !== 'function') {
      throw "Need a function as a handler";
    }
    this.handlers.push(handler);
    return this;
  }
})();