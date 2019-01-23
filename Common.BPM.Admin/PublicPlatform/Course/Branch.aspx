<%@ Page Title="" Language="C#" MasterPageFile="~/PublicPlatform/Course/Default.Master" AutoEventWireup="true" CodeBehind="Branch.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Course.Branch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/Branch.js?d=<%=DateTime.Now.Ticks %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="container">
        <div class="page">
            <div class="page__hd">
                <div class="weui-panel weui-panel_access">
                    <div class="weui-panel__hd">专业方向设置</div>
                    <div class="weui-panel__bd" id="list">
                        
                    </div>
                </div>
            </div>
        </div>
        <div class="weui-btn-area">
            <a class="weui-btn weui-btn_primary" href="./BranchChoose.aspx" id="chooseButton">选择专业方向</a>
        </div>
    </div>

    <script id="tpl" type="text/html">
        <a href="./BranchDetail.aspx?id={{KeyId}}" class="weui-media-box weui-media-box_appmsg">
            <div class="weui-media-box__hd">
                <img class="weui-media-box__thumb" src="./images/branch_small.png" alt="">
            </div>
            <div class="weui-media-box__bd">
                <h4 class="weui-media-box__title">{{Title}}</h4>
                <p class="weui-media-box__desc">{{Description}}</p>
            </div>
        </a>
    </script>

</asp:Content>
