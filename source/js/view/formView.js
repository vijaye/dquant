(function () {
  window.FormView = function(data) {
    this.data = data;
    this.pageSize = 10;
  }

  FormView.prototype = {
    render: function(rowIndex) {
      var div = DOM.create('div');

      rowIndex = rowIndex || 1;
      var formDiv = DOM.create('div', {
        className: 'data-form-host',
        style: 'overflow: auto;'
      });
      div.appendChild(formDiv);

      this.tableElement = this._renderForm(rowIndex);
      formDiv.appendChild(this.tableElement);

      var totalCount = this._getTotalCount();
      var that = this;
      var pageIndex = new PageIndex(rowIndex, totalCount, function (selectedIndex) {
        var te = that._renderForm(selectedIndex);
        formDiv.replaceChild(te, that.tableElement);
        that.tableElement = te;
      });

      div.appendChild(pageIndex.render());
      return div;
    },

    _getColumns: function() {
      if (typeof this.data.getColumns === 'function') {
        return this.data.getColumns();
      }

      if (this._columns) {
        return this._columns;
      }

      var firstRow = this._getRow(1);
      if (typeof firstRow === 'object') {
        var columns = [];
        for (var key in firstRow) {
          columns.push(key);
        }

        this._columns = columns;
        return this._columns;
      }

      var columns = ['Value'];
      columns._metadata = { singleton: true };
      this._columns = columns;
      return this._columns;
    },

    _getTotalCount: function() {
      if (typeof this.data.getLength === 'function') {
        return this.data.getLength();
      }

      // Check for is singleton
      return this.data.length || 1;
    },
    
    // index is index(1)
    _getRow: function(index) {
      if (this.data && typeof this.data.getRows === 'function') {
        var rows = this.data.getRows(index - 1, 1);
        if (rows.length > 0) {
          return rows[0];
        } else {
          return null;
        }
      } else if (typeof this.data === 'string') {
        return this.data;
      } else if (index > 0 && index <= this.data.length) {
        return this.data[index - 1];
      } else {
        return this.data;
      }
    },

    _renderRow: function(key, row, rowMeta, colMeta) {
      var tr = DOM.create('tr');
      if (colMeta && colMeta.singleton) {
        tr.appendChild(DOM.create('td', {}, 'Value'));
        tr.appendChild(DOM.create('td', {}, this._renderScalar(null, row)));
        return tr;
      }

      var val = row[key];
      if (!val) {
        val = '(null)';
      }

      var td = DOM.create('td', { className: 'data-form-label' }, key);
      tr.appendChild(td);

      var td = DOM.create('td', { className: 'data-cell' });
      tr.appendChild(td);

      var cell = this._renderScalar(key, val, rowMeta, colMeta);
      if (typeof cell === 'string') {
        td.innerText = cell;
      } else {
        td.appendChild(cell);
      }

      return tr;
    },

    _renderScalar: function(column, scalar, rowMeta, columnsMeta) {
      var val;
      if (scalar) {
        val = scalar.toString();
      } else {
        val = '(null)';
      }

      if (!column || column === 'id') {
        return val;
      }

      if (rowMeta && rowMeta.id && rowMeta.table && rowMeta.database) {
        if (columnsMeta) {
          var colMeta = columnsMeta[column];
          if (colMeta && colMeta.validValues) {
            var choiceCell = new ComboCell(val, colMeta.validValues, colMeta.defaultValue);
            return choiceCell.render();
          }
        }

        var editableCell = new EditableCell(val, true);
        return editableCell.render();
      } else {
        return val;
      }
    },

    _renderForm: function (index) {
      var tbl = DOM.create('table', {
        className: 'data-form'
      });

      var columns = this._getColumns();
      var row = this._getRow(index);
      for (var ii = 0, len = columns.length; ii < len; ii++) {
        var col = columns[ii];
        var tr = this._renderRow(col, row, row._metadata, columns._metadata);
        tbl.appendChild(tr);
      }
      return tbl;
    }
  }
})();