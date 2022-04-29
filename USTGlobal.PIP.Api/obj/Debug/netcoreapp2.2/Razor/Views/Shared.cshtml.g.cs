#pragma checksum "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ef837de94de8739c0fe8a7a89dbfd6c3767ab58a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared), @"mvc.1.0.view", @"/Views/Shared.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Shared.cshtml", typeof(AspNetCore.Views_Shared))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ef837de94de8739c0fe8a7a89dbfd6c3767ab58a", @"/Views/Shared.cshtml")]
    public class Views_Shared : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<USTGlobal.PIP.ApplicationCore.DTOs.EmailDTO>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString(""), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("style", new global::Microsoft.AspNetCore.Html.HtmlString("background-color: #f6f6f6; font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(58, 27, true);
            WriteLiteral("\r\n<!doctype html>\r\n<html>\r\n");
            EndContext();
            BeginContext(85, 3138, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "ef837de94de8739c0fe8a7a89dbfd6c3767ab58a3772", async() => {
                BeginContext(91, 229, true);
                WriteLiteral("\r\n    <meta name=\"viewport\" content=\"width=device-width\">\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\r\n    <title>PIP Digital</title>\r\n    <style>\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n        ");
                EndContext();
                BeginContext(321, 1610, true);
                WriteLiteral(@"@media only screen and (max-width: 620px) {
            table[class=body] h1 {
                font-size: 28px !important;
                margin-bottom: 10px !important;
            }

            table[class=body] p,
            table[class=body] ul,
            table[class=body] ol,
            table[class=body] td,
            table[class=body] span,
            table[class=body] a {
                font-size: 16px !important;
            }

            table[class=body] .wrapper,
            table[class=body] .article {
                padding: 10px !important;
            }

            table[class=body] .content {
                padding: 0 !important;
            }

            table[class=body] .container {
                padding: 0 !important;
                width: 100% !important;
            }

            table[class=body] .main {
                border-left-width: 0 !important;
                border-radius: 0 !important;
                border-right-width: 0 !i");
                WriteLiteral(@"mportant;
            }

            table[class=body] .btn table {
                width: 100% !important;
            }

            table[class=body] .btn a {
                width: 100% !important;
            }

            table[class=body] .img-responsive {
                height: auto !important;
                max-width: 100% !important;
                width: auto !important;
            }
        }
        /* -------------------------------------
            PRESERVE THESE STYLES IN THE HEAD
        ------------------------------------- */
        ");
                EndContext();
                BeginContext(1932, 1284, true);
                WriteLiteral(@"@media all {
            .ExternalClass {
                width: 100%;
            }

                .ExternalClass,
                .ExternalClass p,
                .ExternalClass span,
                .ExternalClass font,
                .ExternalClass td,
                .ExternalClass div {
                    line-height: 100%;
                }

            .apple-link a {
                color: inherit !important;
                font-family: inherit !important;
                font-size: inherit !important;
                font-weight: inherit !important;
                line-height: inherit !important;
                text-decoration: none !important;
            }

            #MessageViewBody a {
                color: inherit;
                text-decoration: none;
                font-size: inherit;
                font-family: inherit;
                font-weight: inherit;
                line-height: inherit;
            }

            .btn-primary table td:hov");
                WriteLiteral(@"er {
                background-color: #34495e !important;
            }

            .btn-primary a:hover {
                background-color: #34495e !important;
                border-color: #34495e !important;
            }
        }
    </style>
");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(3223, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(3225, 6259, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "ef837de94de8739c0fe8a7a89dbfd6c3767ab58a8441", async() => {
                BeginContext(3455, 1544, true);
                WriteLiteral(@"
    <table border=""0"" class=""body"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; background-color: #f6f6f6;"">
        <tr>
            <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">&nbsp;</td>
            <td class=""container"" style=""font-family: sans-serif; font-size: 14px; vertical-align: top; display: block; Margin: 0 auto; max-width: 580px; padding: 10px; width: 580px;"">
                <div class=""content"" style=""box-sizing: border-box; display: block; Margin: 0 auto; max-width: 580px; padding: 10px;"">

                    <!-- START CENTERED WHITE CONTAINER -->
                    <table class=""main"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; background: #ffffff; border-radius: 3px;"">

                        <!-- START MAIN CONTENT AREA -->
                        <tr>
                            <td class=""wrapper"" style=""font-family: sans-serif; font-size: 14px; v");
                WriteLiteral(@"ertical-align: top; box-sizing: border-box; padding: 20px;"">
                                <table border=""0"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;"">
                                    <tr>
                                        <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">
                                            <p style=""font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;"">Hi ");
                EndContext();
                BeginContext(5000, 36, false);
#line 144 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                                                                                                                    Write(Model.TemplateData.ReceiverFirstName);

#line default
#line hidden
                EndContext();
                BeginContext(5036, 1, true);
                WriteLiteral(" ");
                EndContext();
                BeginContext(5038, 35, false);
#line 144 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                                                                                                                                                          Write(Model.TemplateData.ReceiverLastName);

#line default
#line hidden
                EndContext();
                BeginContext(5073, 212, true);
                WriteLiteral(",</p>\r\n                                            <p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\">\r\n                                                PIP \"");
                EndContext();
                BeginContext(5286, 30, false);
#line 146 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                Write(Model.TemplateData.SFProjectId);

#line default
#line hidden
                EndContext();
                BeginContext(5316, 22, true);
                WriteLiteral("\", has been shared by ");
                EndContext();
                BeginContext(5339, 34, false);
#line 146 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                                                                     Write(Model.TemplateData.SenderFirstName);

#line default
#line hidden
                EndContext();
                BeginContext(5373, 1, true);
                WriteLiteral(" ");
                EndContext();
                BeginContext(5375, 33, false);
#line 146 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                                                                                                         Write(Model.TemplateData.SenderLastName);

#line default
#line hidden
                EndContext();
                BeginContext(5408, 749, true);
                WriteLiteral(@"
                                                with following access:
                                            </p>

                                            <table border=""1"" class=""btn btn-light"" style=""border-collapse: separate; width: 100%; box-sizing: border-box;"">
                                                <tbody>
                                                    <tr>
                                                        <th style=""text-align:left"">PIP</th>
                                                        <th style=""text-align:left"">Version</th>
                                                        <th style=""text-align:left"">Access</th>
                                                    </tr>

");
                EndContext();
#line 158 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                      int i = 0;

#line default
#line hidden
                BeginContext(6224, 52, true);
                WriteLiteral("                                                    ");
                EndContext();
#line 159 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                     foreach (var version in @Model.Versions)
                                                    {

#line default
#line hidden
                BeginContext(6374, 126, true);
                WriteLiteral("                                                        <tr>\r\n                                                            <td>");
                EndContext();
                BeginContext(6501, 30, false);
#line 162 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                           Write(Model.TemplateData.SFProjectId);

#line default
#line hidden
                EndContext();
                BeginContext(6531, 71, true);
                WriteLiteral("</td>\r\n                                                            <td>");
                EndContext();
                BeginContext(6603, 7, false);
#line 163 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                           Write(version);

#line default
#line hidden
                EndContext();
                BeginContext(6610, 71, true);
                WriteLiteral("</td>\r\n                                                            <td>");
                EndContext();
                BeginContext(6682, 14, false);
#line 164 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                           Write(Model.Roles[i]);

#line default
#line hidden
                EndContext();
                BeginContext(6696, 70, true);
                WriteLiteral("</td>\r\n                                                        </tr>\r\n");
                EndContext();
#line 166 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                        i++;
                                                    }

#line default
#line hidden
                BeginContext(6883, 451, true);
                WriteLiteral(@"                                                </tbody>
                                            </table>

                                            <p style=""font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;"">Please find the comments below:</p>
                                            <p style=""font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;"">");
                EndContext();
                BeginContext(7335, 26, false);
#line 172 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
                                                                                                                                                 Write(Model.TemplateData.Comment);

#line default
#line hidden
                EndContext();
                BeginContext(7361, 227, true);
                WriteLiteral(".</p>\r\n                                            <p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\">\r\n                                                Please click this <a");
                EndContext();
                BeginWriteAttribute("href", " href=\"", 7588, "\"", 7606, 1);
#line 174 "D:\workspace\pip\src\USTGlobal.PIP.Api\Views\Shared.cshtml"
WriteAttributeValue("", 7595, Model.Link, 7595, 11, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(7607, 1870, true);
                WriteLiteral(@" target=""_blank"">link</a> to access the PIP
                                            </p>
                                            <p style=""font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;"">Thanks,</p>
                                            <p style=""font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;"">PIP Support Team</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <!-- END MAIN CONTENT AREA -->
                    </table>

                    <!-- START FOOTER -->
                    <div class=""footer"" style=""clear: both; Margin-top: 10px; text-align: center; width: 100%;"">
                        <table border=""0"" style=""border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;"">
             ");
                WriteLiteral(@"               <tr>
                                <td class=""content-block powered-by"" style=""font-family: sans-serif; vertical-align: top; padding-bottom: 10px; padding-top: 10px; font-size: 12px; color: #999999; text-align: center;"">
                                    Powered by <a href=""https://www.xpanxion.com/"" style=""color: #999999; font-size: 12px; text-align: center; text-decoration: none;"">PIPDigital Email Service</a>.
                                </td>
                            </tr>
                        </table>
                    </div>
                    <!-- END FOOTER -->
                    <!-- END CENTERED WHITE CONTAINER -->
                </div>
            </td>
            <td style=""font-family: sans-serif; font-size: 14px; vertical-align: top;"">&nbsp;</td>
        </tr>
    </table>
");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(9484, 11, true);
            WriteLiteral("\r\n</html>\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<USTGlobal.PIP.ApplicationCore.DTOs.EmailDTO> Html { get; private set; }
    }
}
#pragma warning restore 1591
