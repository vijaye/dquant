<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test1.aspx.cs" Inherits="test1" %>

<!doctype html />
<html>
<head id="Head1" runat="server">
  <script src="js/es3.js" type="text/javascript"></script>
  <script src="js/parser.js" type="text/javascript"></script>
  <script src="js/dataObject.js" type="text/javascript"></script>
  <script src="js/evaluator.js" type="text/javascript"></script>
  <script src="js/ajax.js" type="text/javascript"></script>
  <script src="js/utils.js" type="text/javascript"></script>
  <script src="js/part.js" type="text/javascript"></script>

  <script src="js/data/database.js" type="text/javascript"></script>
  <script src="js/data/dbtable.js" type="text/javascript"></script>
  <script src="js/data/odata.js" type="text/javascript"></script>
  
  <script src="js/view/pageIndex.js" type="text/javascript"></script>
  <script src="js/view/formulaBar.js" type="text/javascript"></script>
  <script src="js/view/editableCell.js" type="text/javascript"></script>
  <script src="js/view/comboCell.js" type="text/javascript"></script>
  <script src="js/view/tableView.js" type="text/javascript"></script>
  <script src="js/view/formView.js" type="text/javascript"></script>
  <link rel="Stylesheet" type="text/css" href="site.css" />
  <title></title>
  <script type="text/javascript">
    var partId = 1;
    function onAddPart() {
      var parts = document.getElementById('parts');
      var part = new Part('part_' + partId++);
      parts.appendChild(part.render());
      return false;
    }
  </script>
</head>
<body>
  <form id="form1" runat="server"></form>
  <div>
    <div id='pane' style="float:left; width: 800px;"></div>
    <div id='parts' style="margin-left: 80px;">
      <button onclick='onAddPart();' type='button'><label>Add Part</label></button>
    </div>
  </div>
</body>
</html>
