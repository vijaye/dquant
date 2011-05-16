<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wsexplorer.aspx.cs" Inherits="wsexplorer" %>

<!doctype html />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Web service explorer</title>
  <script type="text/javascript">
    function getMethods() {
      var xhr = new XMLHttpRequest();
      xhr.open('GET', 'http://www.mozilla.org/', true);
      xhr.onreadystatechange = function (e) {
        if (xhr.readyState != 4) {
          return;
        }

        if (xhr.status == 200) {
          dump(xhr.responseText);
        }
        else {
          dump("Error loading page\n");
        }
      };
      req.send(null);
    }
    
  </script>
</head>
<body>
  <form id="form1" runat="server">
  <div>
    <input type="text" value="http://www.webservicex.net/MortgageIndex.asmx" />
    <br />
    <button type="button" onclick='getMethods()'>Explore</button>
  </div>
  <div id="explorer">
  </div>
  </form>
</body>
</html>
