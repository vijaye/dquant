<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_default" %>

<!doctype html />
<html>
<head runat="server">
  <script src="js/es3.js" type="text/javascript"></script>
  <script src="js/parser.js" type="text/javascript"></script>
  <script src="js/dataObject.js" type="text/javascript"></script>
  <script src="js/evaluator.js" type="text/javascript"></script>
  <script src="js/ajax.js" type="text/javascript"></script>
  <script src="js/utils.js" type="text/javascript"></script>
  <script src="js/view/tableview.js" type="text/javascript"></script>
  
  <script src="js/database.js" type="text/javascript"></script>
  <link rel="Stylesheet" type="text/css" href="site.css" />
  <title></title>
  <script type="text/javascript">
    var parser = make_parse();
    var evaluator = new Evaluator();

    function updateVariable() {
      var val3 = document.getElementById("valText");
      evaluator.setSymbol('val', val3.value);
    }

    function parseSource() {
      var source = document.getElementById("source");
      var results = document.getElementById("results");

      var stmts = parser.parseStatements(source.value);
      results.innerHTML = stmts.toString();
    }

    function evalSource() {
      var source = document.getElementById("source");
      var results = document.getElementById("results");

      var r = evaluator.evaluate(source.value);
      if (r && r.getValue()) {
        results.innerHTML = r.getValue().toString();
        r.subscribe(function () {
          results.innerHTML = r.getValue().toString();
        });
      }
    }

    var count = 0;
    function stressTest() {
      if (count < 1000) {
        count++;
        evaluator.setSymbol('val', count);
        setTimeout(stressTest, 0);
      }
    }

    function testAjax() {
      var a = new Ajax("ajax/database.aspx");
      // a.invoke("foobar", 23, 55, "sdfsdf", 66);
      a.invoke("add", 23, 54);
    }

    function testAjax2() {
      var source = document.getElementById("source");
      var results = document.getElementById("results");

      var a = new Ajax("ajax/database.aspx");
      var result = a.invokeContent(source.value);

      results.innerText = result;
    }

    function showTable() {
      var results = document.getElementById("results");
      var database = new Database('dataviz_1');
      var table = database.getTables()[0];

      var t = database.renderTable(table);
      results.innerHTML = t;
    }

    function onLoad() {
      updateVariable();
    }
  </script>
</head>
<body onload='onLoad()'>
  <form id="form1" runat="server">
  <div>
    this is some text. aslk alskjflas flaksjdf<br />
    val =
    <input id='valText' value='42' onchange='updateVariable()' />
    <textarea id="source" class="sourceText" rows="6" cols="120"></textarea>
    <br />
    <button type="button" onclick="evalSource()">Evaluate</button>
    <button type="button" onclick="parseSource()">Just Parse</button>
    <button type="button" onclick="stressTest()">Stress</button>
    <button type="button" onclick="testAjax()">Test Ajax</button>
    <button type="button" onclick="testAjax2()">Test Ajax2</button>
    <button type="button" onclick="showTable()">Show Table</button>
    <div id="results"></div>
  </div>
  </form>
</body>
</html>
