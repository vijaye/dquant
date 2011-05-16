(function () {
  window.TableView = function(data) {
    this.data = data;
    this.pageSize = 10;
  }

  TableView.prototype = {
    render: function(page) {
      var div = DOM.create('div');

      page = page || 1;
      var tableDiv = DOM.create('div', {
        className: 'data-table-host',
        style: 'overflow: auto;'
      });
      div.appendChild(tableDiv);

      this.tableElement = this._renderTable(page);
      tableDiv.appendChild(this.tableElement);

      var totalCount = this._getTotalCount();
      if (totalCount > this.pageSize) {
        var maxPage = Math.floor(totalCount / this.pageSize);
        if (totalCount % this.pageSize > 0) {
          maxPage++;
        }

        var that = this;
        var pageIndex = new PageIndex(page, maxPage, function (selectedPage) {
          var te = that._renderTable(selectedPage);
          tableDiv.replaceChild(te, that.tableElement);
          that.tableElement = te;
        });
        div.appendChild(pageIndex.render());
      }

      return div;
    },

    _getColumns: function() {
      if (typeof this.data.getColumns === 'function') {
        return this.data.getColumns();
      }

      if (this._columns) {
        return this._columns;
      }

      var firstRow = this.data;
      var topRows = this._getRows(1);
      if (topRows.length > 0) {
        firstRow = topRows[0];
      } 

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

      return this.data.length;
    },
    
    // page is index(1)
    _getRows: function(page) {
      if (this.data && typeof this.data.getRows === 'function') {
        return this.data.getRows((page - 1) * 10, 10);
      } else if (typeof this.data === 'string') {
        return [this.data];
      } else if (this.data.length > 0) {
        var minIndex = Math.min(Math.max(0, (page - 1) * this.pageSize), this.data.length);
        var maxIndex = Math.min(minIndex + this.pageSize, this.data.length);

        var rowsData = [];
        for (var ii = minIndex; ii < maxIndex; ii++) {
          rowsData.push(this.data[ii]);
        }

        return rowsData;
      } else {
        return [this.data];
      }
    },

    _renderRow: function(columns, row) {
      var tr = DOM.create('tr');
      if (columns._metadata && columns._metadata.singleton) {
        tr.appendChild(DOM.create('td', {}, this._renderScalar(null, row)));
        return tr;
      }

      for (var ii = 0, len = columns.length; ii < len; ii++) {
        var key = columns[ii];
        var val = row[key];
        if (!val) {
          val = '(null)';
        }

        var td = DOM.create('td', { className: 'data-cell' });
        tr.appendChild(td);
        
        var cell = this._renderScalar(key, val, row._metadata, this._getColumns()._metadata);
        if (typeof cell === 'string') {
          td.innerText = cell;
        } else {
          td.appendChild(cell);
        }
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

        var editableCell = new EditableCell(val);
        return editableCell.render();
      } else {
        return val;
      }
    },

    _renderTable: function(page) {
      var tbl = DOM.create('table', {
        className: 'data-table'
      });

      var thr = DOM.create('tr', {
        className: 'data-table-head-row'
      });
      tbl.appendChild(thr);

      var columns = this._getColumns();
      for (var ii = 0, len = columns.length; ii < len; ii++) {
        var th = DOM.create('th', {
          className: 'data-table-head-col'
        }, columns[ii]);

        thr.appendChild(th);
      }

      var rows = this._getRows(page);
      for (var ii = 0, len = rows.length; ii < len; ii++) {
        var row = rows[ii];
        var tr = this._renderRow(columns, row);
        tbl.appendChild(tr);
      }
      return tbl;
    }
  }
})();