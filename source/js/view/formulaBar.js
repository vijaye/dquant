(function () {
  window.FormulaBar = function () {
    this.content = DOM.create('div', {
      className: 'data-formula-bar'
    });

    this.textBox = DOM.create('input', {
      type: 'text',
      onkeydown: Utils.bind(this._onkeydown, this)
    });

    this.content.appendChild(this.textBox);
  }

  FormulaBar.prototype = {
    focus: function() {
      if (this.content) {
        this.content.focus();
      }
    },

    _onkeydown: function (e) {
      var e = e || window.event;
      var keyCode = (e.keyCode ? e.keyCode : e.which); 
      
      switch (keyCode) {
        case 13:
          // Return
          if (this.onevaluate) {
            this.onevaluate(this.textBox.value);
            return true;
          }
      }

      return false;
    },
    
    render: function () {
      return this.content;
    }
  }
})();