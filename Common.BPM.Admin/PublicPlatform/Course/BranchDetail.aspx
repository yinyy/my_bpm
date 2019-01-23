<%@ Page Title="" Language="C#" MasterPageFile="~/PublicPlatform/Course/Default.Master" AutoEventWireup="true" CodeBehind="BranchDetail.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Course.BranchDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/BranchDetail.js?d=<%=DateTime.Now.Ticks %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="page">
            <div class="page__bd" id="detail">
                
            </div>
            <div class="weui-btn-area">
                <a href="javascript:window.history.back();" class="weui-btn weui-btn_primary">返回</a>
            </div>
        </div>
    </div>
    

    <script id="tpl" type="text/html">
        <article class="weui-article">
            <h1>{{Title}}</h1>
            <section>
                <section>
                    <h3>专业方向负责教师：{{Teacher}}</h3>
                    <h3>专业方向介绍</h3>
                    <p style="text-indent: 2em">{{Description}}</p>
                </section>
            </section>
        </article>
    </script>

    <script type="text/javascript">
        loadDetail(<%=Request.Params["id"]%>);
    </script>
</asp:Content>
