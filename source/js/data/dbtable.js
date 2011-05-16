(function () {
  window.DbTable = function(database, table) {
    if (!(this instanceof DbTable)) {
      return new DbTable(database, table);
    }
    this.database = database;
    this.table = table;
    this.ajax = new Ajax('ajax/database.aspx');
  }

  DbTable.prototype = {
    getColumns: function() {
      if (!this.columns) {
        var ret = this.ajax.invoke("getColumns", this.database, this.table);
        if (ret.error) {
          this.columns = [];
        } else {
          this.columns = ret.result;
        }
      }

      return this.columns;
    },

    getLength: function() {
      var ret = this.ajax.invoke("getLength", this.database, this.table);
      if (ret.error) {
        return 0;
      } else {
        return ret.result;
      }
    },

    getRows: function(start, length) {
      var ret = this.ajax.invoke("getRows", this.database, this.table, start, length);
      if (ret.error) {
        return [];
      } else {
        return ret.result;
      }
    }
  }

  Evaluator.defaultInstance.setSymbol('DbTable', DbTable, true);
})();