<%@ Page Title="" Language="C#" MasterPageFile="~/PublicPlatform/Exam/Default.Master" AutoEventWireup="true" CodeBehind="InvigilateList.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Exam.InvigilateList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/InvigilateList.js?d=<%=DateTime.Now.Ticks %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container" id="container">
        <div class="page tabbar">
            <div class="page__bd" style="height: 100%;">
			    <div class="weui-tab">
				    <div class="weui-tab__panel">
                        <div style="text-align: center">近期没有监考安排。</div>
				    </div>
				    <div class="weui-tabbar">
					    <a href="./InvigilateCalendar.aspx" class="weui-tabbar__item">
						    <img src="./images/icon_tabbar_calendar.png" alt="" class="weui-tabbar__icon">
						    <p class="weui-tabbar__label">监考日历</p>
					    </a>
					    <a href="./InvigilateList.aspx" class="weui-tabbar__item weui-bar__item_on">
						    <img src="./images/icon_tabbar_list.png" alt="" class="weui-tabbar__icon">
						    <p class="weui-tabbar__label">监考记录</p>
					    </a>
					    <a href="./ExamList.aspx" class="weui-tabbar__item">
						    <img src="./images/icon_tabbar_info.png" alt="" class="weui-tabbar__icon">
						    <p class="weui-tabbar__label">考试信息</p>
					    </a>
				    </div>
			    </div>
		    </div>
	    </div>
    </div>

    <script type="text/html" id="tpl">
        {{each Data data index}}
        <div class="weui-form-preview">
            <div class="weui-form-preview__bd">
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">考试名称</label>
                    <span class="weui-form-preview__value">{{data.ExamTitle}}</span>
                </div>
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">场次名称</label>
                    <span class="weui-form-preview__value">{{data.ExamSectionTitle}}</span>
                </div>
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">监考日期</label>
                    <span class="weui-form-preview__value">{{data.Date}}</span>
                </div>
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">监考时间</label>
                    <span class="weui-form-preview__value">{{data.Time}}</span>
                </div>
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">监考地点</label>
                    <span class="weui-form-preview__value">{{data.Address}}</span>
                </div>
                <div class="weui-form-preview__item">
                    <label class="weui-form-preview__label">确认状态</label>
                    <span class="weui-form-preview__value">{{if data.Confirmed==0}}<span style="color: #ff0000;">未确认</span>{{else}}已确认{{/if}}</span>
                </div>
            </div>
        </div>
        <br />
        {{/each}}
    </script>

    <script type="text/html" id="tpl2">
        <div style="text-align: center">没有查询到监考记录 。</div>
    </script>

    <script type="text/html" id="tpl3">
        <div style="text-align: center">
            用户还没有登录。<br /><span id="countdown">3</span>秒后验证身份，或点击下面的按钮直接验证。
        </div>

        <div class="weui-btn-area">
            <a class="weui-btn weui-btn_primary" href="{{Data}}">验证身份</a>
        </div>
    </script>
</asp:Content>
