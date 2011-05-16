(function () {
  var _currentHandler = null;

  var _arityMap = {
    binary: function (node) {
      var f = _evaluate(node.first);
      var s = _evaluate(node.second);
      return _binaryOperatorMap[node.value](f, s);
    },
    literal: function (node) {
      return node.value;
    },
    ternary: function (node) {
      var f = _evaluate(node.first);
      var s = _evaluate(node.second);
      var t = _evaluate(node.third);
      return _ternaryOperatorMap[node.value](f, s, t);
    },
    unary: function (node) {
      return _unaryOperatorMap[node.value](node.first);
    },
    variable: function (node) {
      var dataObject = _symbols[node.value];
      if (dataObject && typeof (dataObject.subscribe) === 'function') {
        if (_currentHandler) {
          dataObject.subscribe(_currentHandler);
        }
        return dataObject.getValue();
      }

      return dataObject;
    }
  };

  var _binaryOperatorMap = {
    '+': function (f, s) { return f + s; },
    '-': function (f, s) { return f - s; },
    '*': function (f, s) { return f * s; },
    '/': function (f, s) { return f / s; },
    '===': function (f, s) { return f === s; },
    '==': function (f, s) { return f == s; },
    '!==': function (f, s) { return f !== s; },
    '!=': function (f, s) { return f != s; },
    '<': function (f, s) { return f < s; },
    '>': function (f, s) { return f > s; },
    '<=': function (f, s) { return f <= s; },
    '>=': function (f, s) { return f >= s; },
    '&&': function (f, s) { return f && s; },
    '||': function (f, s) { return f || s; },
    '[': function (f, s) { return f[s]; },
    '(': function (f, s) {
      if (typeof f === 'function')
        return f.apply(window, s);
      else
        return undefined;
    },
    '.': function (f, s) {
      if (typeof f !== 'undefined')
        return f[s];
      else
        return undefined;
    }
  };

  var _evaluate = function (node) {
    if (node instanceof Array) {
      var result = [];
      for (var i = 0, len = node.length; i < len; i++) {
        result.push(_evaluate(node[i]));
      }
      return result;
    } else if (node.arity === undefined) {
      return node;
    } else {
      return _arityMap[node.arity](node);
    }
  };

  var _symbols = {};

  var _ternaryOperatorMap = {
    '(': function (f, s, t) {
      var func = _binaryOperatorMap['.'](f, s);
      if (typeof func === 'function')
        return func.apply(f, t);
      else
        return undefined;
    }
  };

  var _unaryOperatorMap = {
    '-': function (f) {
      return -_evaluate(f);
    },
    '{': function (f) {
      var obj = {};
      for (var ii = 0, len = f.length; ii < len; ii++) {
        obj[f[ii].key] = _evaluate(f[ii]);
      }
      return obj;
    },
    '[': function (f) {
      var obj = [];
      for (var ii = 0, len = f.length; ii < len; ii++) {
        obj.push(_evaluate(f[ii]));
      }

      return obj;
    }
  };

  window.Evaluator = function () {
    this.parser = make_parse();
    this.setSymbol('Math', window.Math, true);
  };

  Evaluator.prototype.getSymbol = function (name) {
    var symbol = _symbols[name];
    if (!symbol) {
      symbol = new DataObject(null);
      _symbols[name] = symbol;
    }

    return symbol;
  };

  Evaluator.prototype.setSymbol = function (name, value, static) {
    var symbol = _symbols[name];
    if (static || (value !== undefined && typeof value.subscribe === 'function')) {
      if (symbol === undefined) {
        symbol = value;
        _symbols[name] = symbol;
      } else {
        if (typeof value.coalesce === 'function') {
          value.coalesce(symbol);
        }
        _symbols[name] = value;
      }
      return value;
    } else {
      if (symbol === undefined) {
        var symbol = new DataObject(value);
        _symbols[name] = symbol;
      } else {
        symbol.setValue(value);
      }

      return symbol;
    }
  };

  Evaluator.prototype.evaluate = function (source) {
    var tokens = source.tokens();
    var expression = this.parser.parseExpression(source);

    // TODO: How do we cleanup this subscription?
    var result = new DataObject(null);
    var changeHandler = function () {
      if (!result.isDisposed) {
        result.setValue(_evaluate(expression));
      }
    };

    _currentHandler = changeHandler;
    result.setValue(_evaluate(expression));
    _currentHandler = null;

    return result;
  };

  Evaluator.defaultInstance = new Evaluator();
})();