<%@ Page Language="C#"%>
<%@ Import Namespace="Senparc.Weixin.MP.AdvancedAPIs.OAuth" %>
<%@ Import Namespace="Senparc.Weixin.MP.CommonAPIs" %>
<%@ Import Namespace="Senparc.Weixin.MP.AdvancedAPIs" %>
<%@ Import Namespace="BPM.Core.Model" %>
<%@ Import Namespace="BPM.Core.Bll" %>

<%
    foreach (Department depart in DepartmentBll.Instance.GetAll())
    {
        if (!string.IsNullOrWhiteSpace(depart.Appid) && !AccessTokenContainer.CheckRegistered(depart.Appid))
        {
            AccessTokenContainer.Register(depart.Appid, depart.Secret);
        }
    }


    Department dept = DepartmentBll.Instance.Get(79);
%>

<%=AccessTokenContainer.CheckRegistered(dept.Appid)%>