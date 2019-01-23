<%@ Page Title="" Language="C#" MasterPageFile="~/PublicPlatform/Course/Default.Master" AutoEventWireup="true" CodeBehind="BranchChoose.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Course.BranchChoose" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/BranchChoose.js?d=<%=DateTime.Now.Ticks %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div id="page1" class="page hidden">
            <div class="page__hd">
                <div class="page__title">选择专业方向</div>
                <div class="page__desc">每人选择一个第一专业方向，最多选择两个备选专业方向。</div>
            </div>
            <div class="page_bd">
                <div class="weui-cells__title">第一专业方向</div>
                <div class="weui-cells">
                    <div class="weui-cell weui-cell_select">
                        <div class="weui-cell__bd select__bd">
                        </div>
                    </div>
                </div>
                <div class="weui-cells__title">自我评价</div>
                <div class="weui-cells weui-cells_form">
                    <div class="weui-cell">
                        <div class="weui-cell__bd">
                            <textarea class="weui-textarea" placeholder="你的特长、兴趣及选择该专业方向后的学习规划。" rows="4" id="Introduction"></textarea>
                            <div class="weui-textarea-counter"><span id="overage">0</span>/500</div>
                        </div>
                    </div>
                </div>

                <div class="weui-cells__title">备选专业方向</div>
                <div class="weui-cells">
                    <div class="weui-cell weui-cell_select">
                        <div class="weui-cell__bd select__bd">
                        </div>
                    </div>
                    <div class="weui-cell weui-cell_select">
                        <div class="weui-cell__bd select__bd">
                        </div>
                    </div>
                </div>

                <div class="weui-btn-area">
                    <a class="weui-btn weui-btn_primary" href="javascript:" id="updateButton">确定</a>
                </div>
            </div>
        </div>
        <div id="page2" class="page hidden">
            
        </div>
    </div>

    <script id="tpl1" type="text/html">
        <select class="weui-select">
            <option value="0" selected="selected">请选择</option>
            {{each Data}}
            <option value="{{$value.KeyId}}">{{$value.Title}}</option>
            {{/each}}
        </select>
    </script>

    <script id="tpl2" type="text/html">
        <div class="page__hd">
            <div class="page__title">专业方向已确定</div>
            <div class="page__desc">你已被 {{Title}} 专业方向录取。</div>
        </div>
        <div class="weui-btn-area">
            <a class="weui-btn weui-btn_primary" href="javascript:window.history.back();">确定</a>
        </div>
    </script>
</asp:Content>
