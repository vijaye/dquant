(function () {
  window.EditableCell = function(value, wrap) {
    this.value = value;
    this.editMode = false;
    this.textBox = null;
    this.wrap = wrap ? true : false;

    var className = 'editable-cell-readonly';
    if (this.wrap) {
      className = ' wrap'; // TODO: use CSS add class.
    }

    this._content = DOM.create('div', {
      className: className,
      onclick: Utils.bind(this.handleClick, this)
    }, value);
  }

  EditableCell.prototype = {
    handleClick: function(e) {
      if (this.editMode) {
        return;
      }

      this.editMode = true;
      if (!this.textBox) {
        this.textBox = DOM.create('input', {
          type: 'text'
        });
      }

      this.textBox.value = this.value;
      this.textBox.style.width = this._content.clientWidth + "px";

      DOM.clear(this._content);
      this._content.appendChild(this.textBox);
      this.textBox.focus();
      this.textBox.select();

      this.textBox.onblur = Utils.bind(function(e) {
        this.value = this.textBox.value;
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