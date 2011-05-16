(function () {
  window.Ajax = function (endpoint) {
    this.endpoint = endpoint;
  };

  Ajax.prototype.invoke = function (methodName) {
    var args = [];
    for (var i = 1, len = arguments.length; i < len; i++) {
      args.push(arguments[i]);
    }
    var obj = {
      method: methodName,
      arguments: args
    };
    var postData = JSON.stringify(obj);
    return this.invokeContent(postData);
  }

  Ajax.prototype.invokeContent = function (content) {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", this.endpoint, false);
    xhr.send(content);
    
    if (xhr.status == 200) {
      return JSON.parse(xhr.responseText);
    } else {
      alert("Error " + xhr.status);
      return null;
    }
  }

  Ajax.prototype.invokeAsync = function (methodName, success, error, args) {
    var obj = {
      method: methodName,
      arguments: args
    };
    var postData = JSON.stringify(obj);
    return this.invokeContentAsync(postData, success, error);
  }

  Ajax.prototype.invokeContentAsync = function (content, success, error) {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", this.endpoint, true);

    xhr.onreadystatechange = function () {
      if (xhr.readyState == 4) {
        if (xhr.status == 200 && success)
          success(JSON.parse(xhr.responseText));
        else if (error)
          error(xhr.status);
      }
    };
    xhr.send(content);
  }
})();