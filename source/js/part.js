(function () {
  window.Part = function (name) {
    this.name = name;

    this.caption = DOM.create('div', {
      className: 'data-part-caption'
    });
    this.caption.appendChild(DOM.create('span', {
      className: 'data-part-name'
    }, name));
    
    this.formulaPane = DOM.create('div', {});    
    this.formulaBar = new FormulaBar();
    this.formulaBar.onevaluate = Utils.bind(this._onevaluate, this);
    this.formulaPane.appendChild(this.formulaBar.render());
    this.formulaBar.focus();
    
    this.resultsPane = DOM.create('div', {
      className: 'data-results'
    });

    this.content = DOM.create('div', {
      className: 'data-part'
    });

    this.content.appendChild(this.caption);
    this.content.appendChild(this.formulaPane);
    this.content.appendChild(this.resultsPane);

    this.evaluator = Evaluator.defaultInstance;
    var symbol = this.evaluator.getSymbol(this.name);
    symbol.subscribe(Utils.bind(this._renderResults, this));
  }

  Part.prototype = {
    _onevaluate: function (formula) {
      var r = this.evaluator.evaluate(formula);
      this.evaluator.setSymbol(this.name, r, false);
    },

    _renderResults: function (result) {
      var val = result.getValue();
      // var view = new TableView(val);
      var view = new FormView(val);
      DOM.clear(this.resultsPane);
      this.resultsPane.appendChild(view.render());
    },

    render: function () {
      return this.content;
    }
  }
})();