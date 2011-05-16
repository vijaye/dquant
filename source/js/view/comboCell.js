(function () {
  window.ComboCell = function(value, validValues, defaultValue) {
    this.value = value;
    this.editMode = false;
    this.comboBox = null;
    this.validValues = validValues;
    this.defaultValue = defaultValue;

    this._content = DOM.create('div', {
      className: 'editable-cell-readonly',
      onclick: Utils.bind(this.handleClick, this)
    }, value);
  }

  ComboCell.prototype = {
    handleClick: function(e) {
      if (this.editMode) {
        return;
      }

      this.editMode = true;
      if (!this.comboBox) {
        this.comboBox = DOM.create('select');
        for (var ii = 0, len = this.validValues.length; ii < len; ii++) {
          var option = this.validValues[ii];
          this.comboBox.appendChild(DOM.create('option', {
            value: option
          }, option));
        }
      }

      this.comboBox.value = this.value || this.defaultValue;
      this.comboBox.style.width = this._content.clientWidth + "px";

      DOM.clear(this._content);
      this._content.appendChild(this.comboBox);
      this.comboBox.focus();

      this.comboBox.onblur = Utils.bind(function (e) {
        this.value = this.comboBox.value;
        DOM.clear(this._content);
        this._content.innerText = this.value;
        this.editMode = false;
      }, this);
    },

    render: function() {
      return this._content;
    }
  }
})();