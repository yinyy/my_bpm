<%@ Page Language="C#"  %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq" %>

<%
    using (StreamWriter writer = new StreamWriter(Server.MapPath("~/App_Data/Params-" + DateTime.Now.Ticks + ".txt")))
    {
        writer.WriteLine(string.Format("echostr={0}\r\nrid={1}", Request.Params["echostr"], Request.Params["rid"]));
    }

    //http://127.0.0.1:9582/Extra/StandardHandler.ashx?signature=473c528dbec76e92c41365a7c2e0f186c4834480&timestamp=1486818830&nonce=123&echostr=abc&tag=lwpicc&itag=Senlan&board=100001&money=1000&rid=1
%>