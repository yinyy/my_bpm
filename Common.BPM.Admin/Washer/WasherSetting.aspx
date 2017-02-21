<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="WasherSetting.aspx.cs" Inherits="BPM.Admin.Washer.WasherSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../scripts/Colorpicker/spectrum.css" rel="stylesheet" />
    <link href="../scripts/fine-uploader/fine-uploader-new.min.css" rel="stylesheet" />
    <style type="text/css">
        div.c ul:first-child li label:first-child{
            text-align: right;
            width: 100px;

            display: inline-block;
        }

        div.c ul:first-child li img#Logo{
            width: 48px;
            height: 48px;

            border: 1px solid #ffffff;
        }

        .full-spectrum .sp-palette {
            max-width: 200px;
        }

        div.c ul:first-child li ul {
            display: inline-block;
            vertical-align: middle;
        }

        div.c ul:first-child li ul li label:first-child{
            width: 60px;
            text-align: right;
        }

        div.c ul:first-child li ul li input{
            width: 60px;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="sysconfig" style="margin:10px;">
        <h1>微信参数设置</h1>
        <div class="c">
            <ul>
                <li><label>appid：</label><input type="text" id="txt_Appid" name="Appid" style="width: 500px;"/></li>
                <li><label>secret：</label><input type="text" id="txt_Secret" name="Secret" style="width: 500px;"/></li>
                <li><label>aeskey：</label><input type="text" id="txt_Aeskey" name="Aeskey" style="width: 500px;"/></li>
                <li><label>Token：</label><input type="text" id="txt_Token" name="Token" style="width: 500px;"/></li>
                <li>
                    <label>洗车支付选项：</label><input type="text" id="txt_WxPayOption" name="WxPayOption" style="width: 500px;"/>
                </li>
                <li> 
                    <label>&nbsp;</label>注：在微信洗车支付界面的固定额度（单位为分）。例如：600,800,1000。则在用户界面上显示固定的6元、8元、10元。留空，则用户可以支付任意金额。
                </li>
           </ul>
        </div>
        <h1>短信接口参数设置</h1>
        <div class="c">
            <ul>
                <li><label>cid：</label><input type="text" id="txt_SmsCid" name="SmsCid" style="width: 500px;"/></li>
                <li><label>uid：</label><input type="text" id="txt_SmsUid" name="SmsUid" style="width: 500px;"/></li>
                <li><label>pas：</label><input type="text" id="txt_SmsPas" name="SmsPas" style="width: 500px;"/></li>
                <li><label>url：</label><input type="text" id="txt_SmsUrl" name="SmsUrl" style="width: 500px;"/></li>
           </ul>
        </div>
        <h1>品牌</h1>
        <div class="c">
            <ul>
                <li><label>名称：</label><input type="text" id="txt_Brand" name="Brand" style="width: 300px;"/></li>
                <li><label style="display: inline-block; vertical-align: middle;">商标：</label><img id="Logo" src="/images/PublicPlatform/default_logo.png" alt="Logo" style="display: inline-block; vertical-align: middle;"/><input type="hidden" id="txt_Logo" name="Logo"/><a href="#" id="Upload" class="alertify-button alertify-button-ok" style="margin: 0px 0px 0px 15px; display: inline-block; vertical-align: middle;">上传</a><label style="display: inline-block; vertical-align: middle;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;建议使用48x48像素的png图片</label></li>
           </ul>
        </div>
        <h1>注册信息</h1>
        <div class="c">
            <ul>
                <li>
                    <label>&nbsp;</label>送&nbsp;<input type="text" id="txt_RegisterCoupon" name="RegisterCoupon" style="width:50px;text-align:center;"/>&nbsp;洗车币（元）的优惠卡（填“0”不启用），有效期&nbsp;<input type="text" id="txt_RegisterCouponDay" name="RegisterCouponDay" style="width:50px;text-align:center;" />&nbsp;天。
                </li>
                <li><label>&nbsp;</label><label>或</label></li>
                <li>
                    <label>&nbsp;</label>送&nbsp;<input type="text" id="txt_RegisterPoint" name="RegisterPoint" style="width:50px;text-align:center;"/>&nbsp;积分（填“0”不启用）
                </li>
           </ul>
        </div>
        <h1>购买洗车卡</h1>
        <div class="c">
            <ul>
                <li>
                    <label for="txt_Buy_Card_Option">分类：</label><textarea id="txt_Buy_Card_Option" rows="5" cols="100" style="display: inline-block;vertical-align: middle;"></textarea>
                </li>
                <li><label>&nbsp;</label>格式：洗车卡名称,洗车卡面值（元）,洗车卡售价（元）,洗车卡有效期（天）,购买洗车卡赠送的积分。多种面值的洗车卡用“;”隔开。</li>
           </ul>
        </div>
        <h1>洗车</h1>
        <div class="c">
            <ul>                
                <li>
                    <label>微信支付：</label><label>送支付金额&nbsp;</label><input type="text" id="txt_Pay_Wash_Card_Wx" name="txt_Pay_Wash_Card_Wx" style="text-align:center;width: 50px;"/><label>&nbsp;%的积分。</label>
                </li>
                <li>
                    <label>会员支付：</label><label>送结算金额&nbsp;</label><input type="text" id="txt_Pay_Wash_Card_Vip" name="txt_Pay_Wash_Card_Vip" style="text-align:center;width: 50px;"/><label>&nbsp;%的积分。（包括：手机号+密码支付、卡号+密码支付和刷卡支付）</label>
                </li>
                <li>
                    <label>优惠卡支付：</label><label>不送积分。</label>
                </li>
            </ul>
        </div>
        <h1>推荐奖励<span style="color:#ff0000;">（仅限购买洗车卡和微信支付洗车）</span></h1>
        <div class="c">
            <ul>
                <li>
                    <label>积分比例：</label><input type="text" id="txt_GiftLevel" name="Levels" style="width:814px;"/>
                </li>
                <li>
                    <label>&nbsp;</label>用户消费，给其推荐者的奖励。
                </li>
                <li>
                    <label>&nbsp;</label>格式：一级奖励（百分比）,二级奖励（百分比）,三级奖励（百分比）,四级奖励（百分比）,五级奖励（百分比）。奖励等级一般不超过五级。
                </li>
                <li>
                    <label>&nbsp;</label>例如：50,10,1,0,0。表示消费金额*50%的积分奖励给一级推荐者，消费金额*10%的积分奖励给二级推荐者，消费金额*1%的积分奖励给三级推荐者，四级、五级无奖励。
                </li>
           </ul>
        </div>
        <h1>分享奖励</h1>
        <div class="c">
            <ul>
                <li>
                    <label>给朋友：</label>送&nbsp;<input type="text" id="txt_Relay_Friend" name="RelayFriend" style="width:50px;text-align:center;"/>&nbsp;积分。
                </li>
                <li>
                    <label>到朋友圈：</label>送&nbsp;<input type="text" id="txt_Relay_Moment" name="RelayMoment" style="width:50px;text-align:center;"/>&nbsp;积分。
                </li>
           </ul>
        </div>
    </div>


    <div style="margin:140px;width:160px; margin-top:40px; font-family:'Microsoft YaHei'">
        <a id="btnok" href="javascript:;" class="alertify-button alertify-button-ok">保存设置</a>
        <%--<a id="btnmenu" href="javascript:;" class="alertify-button alertify-button-ok">创建菜单</a>--%>
    </div>

    <div id="fine-uploader" style="display: none;">
    </div>


    <script type="text/template" id="qq-template">
        <div class="qq-uploader-selector qq-uploader" qq-drop-area-text="Drop files here">
            <div class="qq-total-progress-bar-container-selector qq-total-progress-bar-container">
                <div role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" class="qq-total-progress-bar-selector qq-progress-bar qq-total-progress-bar"></div>
            </div>
            <div class="qq-upload-drop-area-selector qq-upload-drop-area" qq-hide-dropzone>
                <span class="qq-upload-drop-area-text-selector"></span>
            </div>
            <div class="qq-upload-button-selector qq-upload-button">
                <div>Upload a file</div>
            </div>
            <span class="qq-drop-processing-selector qq-drop-processing">
                <span>Processing dropped files...</span>
                <span class="qq-drop-processing-spinner-selector qq-drop-processing-spinner"></span>
            </span>
            <ul class="qq-upload-list-selector qq-upload-list" aria-live="polite" aria-relevant="additions removals">
                <li>
                    <div class="qq-progress-bar-container-selector">
                        <div role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" class="qq-progress-bar-selector qq-progress-bar"></div>
                    </div>
                    <span class="qq-upload-spinner-selector qq-upload-spinner"></span>
                    <img class="qq-thumbnail-selector" qq-max-size="100" qq-server-scale>
                    <span class="qq-upload-file-selector qq-upload-file"></span>
                    <span class="qq-edit-filename-icon-selector qq-edit-filename-icon" aria-label="Edit filename"></span>
                    <input class="qq-edit-filename-selector qq-edit-filename" tabindex="0" type="text">
                    <span class="qq-upload-size-selector qq-upload-size"></span>
                    <button type="button" class="qq-btn qq-upload-cancel-selector qq-upload-cancel">Cancel</button>
                    <button type="button" class="qq-btn qq-upload-retry-selector qq-upload-retry">Retry</button>
                    <button type="button" class="qq-btn qq-upload-delete-selector qq-upload-delete">Delete</button>
                    <span role="status" class="qq-upload-status-text-selector qq-upload-status-text"></span>
                </li>
            </ul>

            <dialog class="qq-alert-dialog-selector">
                <div class="qq-dialog-message-selector"></div>
                <div class="qq-dialog-buttons">
                    <button type="button" class="qq-cancel-button-selector">Close</button>
                </div>
            </dialog>

            <dialog class="qq-confirm-dialog-selector">
                <div class="qq-dialog-message-selector"></div>
                <div class="qq-dialog-buttons">
                    <button type="button" class="qq-cancel-button-selector">No</button>
                    <button type="button" class="qq-ok-button-selector">Yes</button>
                </div>
            </dialog>

            <dialog class="qq-prompt-dialog-selector">
                <div class="qq-dialog-message-selector"></div>
                <input type="text">
                <div class="qq-dialog-buttons">
                    <button type="button" class="qq-cancel-button-selector">Cancel</button>
                    <button type="button" class="qq-ok-button-selector">Ok</button>
                </div>
            </dialog>
        </div>
    </script>


    
    <script src="ashx/WasherSettingHandler.ashx?action=js" type="text/javascript"></script>
    <!-- 引入js文件 -->
    <script src="../scripts/Colorpicker/spectrum.js?d=<%=DateTime.Now %>"></script>
    <script src="../scripts/fine-uploader/fine-uploader.min.js"></script>
    <script src="js/WasherSetting.js?d=<%=DateTime.Now %>"></script>
    <script src="../Editor/xhEditor/xheditor-1.2.1.min.js"></script>
    <script src="../Editor/xhEditor/xheditor_lang/zh-cn.js"></script>
</asp:Content>


