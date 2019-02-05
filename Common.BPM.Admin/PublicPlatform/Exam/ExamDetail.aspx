<%@ Page Title="" Language="C#" MasterPageFile="~/PublicPlatform/Exam/Default.Master" AutoEventWireup="true" CodeBehind="ExamDetail.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Exam.ExamDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/ExamDetail.js?d=<%=DateTime.Now.Ticks %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container" id="container">
        <div class="page preview">
            <div class="page__bd">
                
            </div>
            <div class="page__ft">
                <div class="weui-btn-area">
                    <a class="weui-btn weui-btn_primary" href="javascript:window.history.back()">返回</a>
                </div>
            </div>
        </div>
    </div>

    <script type="text/html" id="tpl">
        {{each Sections section index}}
        <div class="weui-form-preview">
            <div class="weui-form-preview__hd">
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">场次</label>
                    <em class="weui-form-preview__value">{{section.Title}}</em>
                </div>
            </div>
            <div class="weui-form-preview__bd">
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">考试日期</label>
                    <span class="weui-form-preview__value">{{section.Date}}</span>
                </div>
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">考试时间</label>
                    <span class="weui-form-preview__value">{{section.Time}}</span>
                </div>
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">所需人数</label>
                    <span class="weui-form-preview__value">{{section.TeacherRequired}} 人</span>
                </div>
                <%if (Session["openid"] != null){%>
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">预约情况</label>
                    <span class="weui-form-preview__value"{{if section.Choosed}} style="color: blue;font-weight: bolder;"{{/if}}>{{if section.Choosed}}已预约{{else}}未预约{{/if}}</span>
                </div>
                <%} %>
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">本场说明</label>
                    <span class="weui-form-preview__value">{{section.Memo}}</span>
                </div>
            </div>
            
            {{if Exam.Status==0 && Exam.Freezed==0 && Exam.Booking==1}}
                <%if (Session["openid"] != null){ %>
                    {{if section.Choosed}}
                    <div class="weui-form-preview__ft">
                        <a class="weui-form-preview__btn weui-form-preview__btn_primary" href="javascript:selectExamSection({{section.ID}}, false);">取消</a>
                    </div>
                    {{else}}
                    <div class="weui-form-preview__ft">
                        <a class="weui-form-preview__btn weui-form-preview__btn_primary" href="javascript:selectExamSection({{section.ID}}, true);">预约</a>
                    </div>
                    {{/if}}
                <%}else{%>
                <div class="weui-form-preview__ft">
                    <a class="weui-form-preview__btn weui-form-preview__btn_primary" href="{{OAuthUrl}}">授权访问</a>
                </div>
                <%} %>
            {{/if}}
        </div>
        <br>
        {{/each}}
    </script>

    <script type="text/javascript">
        var examId = <%=Request["id"]%>;
    </script>
</asp:Content>
