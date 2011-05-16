(function () {
  window.DOM = {
    create: function (tag, attributes, content) {
      var element = document.createElement(tag);
      
      for (var key in attributes) {
        if (key.indexOf('on') === 0) {
          Event.listen(element, key, attributes[key]);  
        } else {
          if (key == 'className') {
            element.setAttribute('class', attributes[key]);
          } else {
            element.setAttribute(key, attributes[key]);
          }
        }
      }

      if (content) {
        element.innerText = content;
      }

      return element;
    },

    clear: function (obj) {
      while (obj.hasChildNodes()) {
        obj.removeChild(obj.firstChild);
      }
    }
  }

  window.Event = {
    listen: function(obj, event, handler) {
      if (typeof handler !== 'function') {
        throw "Not a function.";
      }

      var onevent = event;
      if (event.indexOf('on') === 0) {
        event = event.substr(2);
      } else {
        onevent = 'on' + event;
      }
      if (obj.addEventListener) {
        obj.addEventListener(event, handler, false);
      } else if (obj.attachEvent) {
        obj.attachEvent(onevent, handler); 
      } else {
        obj[onevent] = handler;
      }
    }
  }

  window.Utils = {
    bind: function(func, _this, arg1, arg2) {
      var fixedArgs = [];
      for (var i = 2, len = arguments.length; i < len; i++) {
        fixedArgs.push(arguments[i]);
      }

      return function() {
        var args = [];
        for (var ii = 0, len = fixedArgs.length; ii < len; ii++) {
          args.push(fixedArgs[ii]);
        }

        for (var ii = 0, len = arguments.length; ii < len; ii++) {
          args.push(arguments[ii]);
        }
        
        func.apply(_this, args);
      }
    }
  }
})();