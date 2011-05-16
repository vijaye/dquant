(function () {
  window.PageIndex = function(selectedPage, totalPages, callback) {
    this.selectedPage = selectedPage;
    this.totalPages = totalPages;
    this.callback = callback;
  }

  PageIndex.prototype = {
    _fillPanelWithPageNumbers: function() {
      DOM.clear(this.panel);

      if (this.totalPages < 1) {
        for (var ii = 1; ii <= this.totalPages; ii++) {
          this.panel.appendChild(this._renderIndex(ii));
        }
      } else {
        this.panel.appendChild(this._renderIndex(1, 'First'));
        
        var prev = Math.max(1, this.selectedPage - 1);
        this.panel.appendChild(this._renderIndex(prev, '<'));

        this.panel.appendChild(DOM.create('span', {
          className: 'page-selected'
        }, this.selectedPage));
        this.panel.appendChild(DOM.create('span', {}, ' / ' + this.totalPages + ' '));
        var next = Math.min(this.totalPages, this.selectedPage + 1);
        this.panel.appendChild(this._renderIndex(next, '>'));
        this.panel.appendChild(this._renderIndex(this.totalPages, 'Last'));        
      }
    },

    render: function() {
      var that = this;
      this.panel = DOM.create('div', {
        className: 'page-index-panel',
        onclick: function (e) {
          var newPage;
          if (e.srcElement) {
            newPage = e.srcElement.getAttribute('data-pageNum');
            if (newPage) {
              that.selectedPage = parseInt(newPage);
              that.callback(that.selectedPage);
              that._fillPanelWithPageNumbers();
            }
          }
        }
      });

      this._fillPanelWithPageNumbers();
      return this.panel;
    },

    _renderIndex: function(index, text) {
      var className = 'page-index';
      return DOM.create('div', {
        className: className,
        "data-pageNum": index
      }, (text || index));
    }
  }
})();