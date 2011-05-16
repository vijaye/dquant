(function () {
  window.Database = function(database) {
    if (!(this instanceof Database)) {
      return new Database(database);
    }
    this.database = database;
    this.ajax = new Ajax('ajax/database.aspx');
  }

  Database.prototype = {
    _getTables: function() {
      if (!this._tables) {
        var ret = this.ajax.invoke("getTables", this.database);
        if (ret.error) {
          this._tables = [];
        } else {
          this._tables = ret.result;
        }
      }
      return this._tables;
    },

    getLength: function() {
      var tables = this._getTables();
      return tables.length;
    },

    getRows: function(start, length) {
      return this._getTables();
    }
  }

  Evaluator.defaultInstance.setSymbol('Database', Database, true);
})();