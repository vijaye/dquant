(function () {
  window.OData = function(url) {
    if (!(this instanceof OData)) {
      return new OData(url);
    }
    this.url = url;
    this.ajax = new Ajax('ajax/odata.aspx');
  }

  OData.prototype = {
    getLength: function() {
      return 500;
    },

    getRows: function(start, length) {
      var ret = this.ajax.invoke("getRows", this.url, start, length);
      if (ret.error) {
        return [];
      } else {
        return ret.result;
      }
    }
  }

  Evaluator.defaultInstance.setSymbol('OData', OData, true);
})();