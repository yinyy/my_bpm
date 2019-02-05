<%@ Page Title="" Language="C#" MasterPageFile="~/PublicPlatform/Exam/Default.Master" AutoEventWireup="true" CodeBehind="ExamList.aspx.cs" Inherits="BPM.Admin.PublicPlatform.Exam.ExamList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/ExamList.js?d=<%=DateTime.Now.Ticks %>"></script>
    <title>考试信息</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container" id="container">
        <div class="page tabbar">
            <div class="page__bd" style="height: 100%;">
			    <div class="weui-tab">
				    <div class="weui-tab__panel">
                        <div style="text-align: center">近期没有考试安排。</div>
				    </div>
				    <div class="weui-tabbar">
					    <a href="./InvigilateCalendar.aspx" class="weui-tabbar__item">
						    <img src="./images/icon_tabbar_calendar.png" alt="" class="weui-tabbar__icon">
						    <p class="weui-tabbar__label">监考日历</p>
					    </a>
					    <a href="./InvigilateList.aspx" class="weui-tabbar__item">
						    <img src="./images/icon_tabbar_list.png" alt="" class="weui-tabbar__icon">
						    <p class="weui-tabbar__label">监考记录</p>
					    </a>
					    <a href="./ExamList.aspx" class="weui-tabbar__item weui-bar__item_on">
						    <img src="./images/icon_tabbar_info.png" alt="" class="weui-tabbar__icon">
						    <p class="weui-tabbar__label">考试信息</p>
					    </a>
				    </div>
			    </div>
		    </div>
	    </div>
    </div>

    <script type="text/html" id="tpl">
        <div class="weui-panel">
            <div class="weui-panel__bd">
                <div class="weui-media-box weui-media-box_text">
                    <h4 class="weui-media-box__title">{{Title}}</h4>
                    <p class="weui-media-box__desc">{{Memo}}</p>
                    <ul class="weui-media-box__info">
                        <li class="weui-media-box__info__meta">{{Time}}</li>
                        {{if Status == 0}}
                        <li class="weui-media-box__info__meta" style="color: orange;">未开始</li>
                        {{else if Status == 1}}
                        <li class="weui-media-box__info__meta" style="color: darkgreen;">进行中</li>
                        {{else}}
                        <li class="weui-media-box__info__meta" style="color: #ff00ff;">已结束</li>
                        {{/if}}
                        {{if Freezed==1}}
                        <li class="weui-media-box__info__meta" style="color: brown;">已归档</li>
                        {{/if}}
                        {{if Booking == 1}}
                        <li class="weui-media-box__info__meta" style="color: #0000ff;">预约中</li>
                        {{/if}}
                    </ul>
                </div>
            </div>
            <div class="weui-panel__ft">
                <a href="./ExamDetail.aspx?id={{ID}}" class="weui-cell weui-cell_access weui-cell_link">
                    <div class="weui-cell__bd">查看详情</div>
                    <span class="weui-cell__ft"></span>
                </a>    
            </div>
        </div>
    </script>
</asp:Content>
