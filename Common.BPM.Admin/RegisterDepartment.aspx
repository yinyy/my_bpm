<%@ Page Language="C#"%>
<%@ Import Namespace="Senparc.Weixin.MP.AdvancedAPIs.OAuth" %>
<%@ Import Namespace="Senparc.Weixin.MP.CommonAPIs" %>
<%@ Import Namespace="Senparc.Weixin.MP.AdvancedAPIs" %>
<%@ Import Namespace="BPM.Core.Model" %>
<%@ Import Namespace="BPM.Core.Bll" %>

<html>
    <head><title>注册部门</title></head>
    <body>
<%
    foreach (Department depart in DepartmentBll.Instance.GetAll())
    {
        if (!string.IsNullOrWhiteSpace(depart.Appid))
        {
            AccessTokenContainer.Register(depart.Appid, depart.Secret);
%>
        <p><%=depart.DepartmentName %> 完成注册。</p>
<%
        }
    }
%>

        <script type="text/javascript">
            JSON.stringify
        </script>
    </body>
</html>