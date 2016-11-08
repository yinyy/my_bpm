<%@ Page Language="C#"  %>

<!DOCTYPE html>  
<html>  
    <head>  
        <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />  
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />  
        <title>Hello, World</title>  
        <style type="text/css">  
            html,body,div,ul,ol{
                padding: 0px;
                margin: 0px;
                font-size: 0.8em;
            }

            html{height:100%}  
            body{height:100%;margin:0px;padding:0px}  
            #container{height:100%} 
           
             .container{
                padding: 20px 0px;
            } 

            .center{
                text-align: center;
            }
        </style>  
        <script type="text/javascript" src="http://api.map.baidu.com/api?v=2.0&ak=Uei1ehk1l2Usia7AkqO1HWktWEWZkGf3"></script>
        <script src="../scripts/jquery-1.8.3.min.js"></script>
        <script src="../scripts/easyui/jquery.easyui.min.js"></script>
        <link href="../scripts/easyui/themes/icon.css" rel="stylesheet" />
        <link href="../scripts/easyui/themes/default/easyui.css" rel="stylesheet" />
        <script type="text/javascript" src="js/AgricultureMap.js?v=<%=DateTime.Now.Ticks %>"></script>
    </head>  
 
    <body>  
        <div id="container"></div>
    </body>  
</html>>