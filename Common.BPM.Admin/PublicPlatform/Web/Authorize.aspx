<%@ Page Language="C#"%>

<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>用户身份验证</title>
    <script src="./js/jquery-2_2_1_min.js"></script>
    <script src="./js/common.js?v=<%=DateTime.Now.Ticks %>"></script>
    <script src="./js/Authorize.js?v=<%=DateTime.Now.Ticks %>"></script>
</head>
<body>
    <script type="text/javascript">
        $(document).ready(function () {
            //进行身份验证
            Authorize.authorize();
            //Authorize.test();
        });
    </script>
</body>
</html>
