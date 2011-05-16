(function () {
  window.Database = function (name) {
    this.name = name;
    this.ajax = new Ajax('ajax/database.aspx');
  }

  Database.prototype.getColumns = function (tableName) {
    var ret = this.ajax.invoke("getColumns", "dataviz_1", tableName);
    if (ret.error) {
      return null;
    } else {
      return ret.result;
    }
  }

  Database.prototype.getTables = function () {
    var ret = this.ajax.invoke("getTables", this.name);
    if (ret.error) {
      return null;
    } else {
      return ret.result;
    }
  }

  Database.prototype.renderTable = function (tableName) {
    var ret = this.ajax.invoke("renderTable", this.name, tableName, 0);
    alert(ret);
    if (ret.error) {
      return ret.error;
    } else {
      return ret.result;
    }
  }

})();