#pragma checksum "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0dce3f8a8a98ff461a7023b5bac590d32a3bfd2b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Temp_test), @"mvc.1.0.view", @"/Views/Temp/test.cshtml")]
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
#nullable restore
#line 4 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
using Newtonsoft.Json;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0dce3f8a8a98ff461a7023b5bac590d32a3bfd2b", @"/Views/Temp/test.cshtml")]
    public class Views_Temp_test : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
  
    Layout = null;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<p align=\"center\">\r\n    ������ � ");
#nullable restore
#line 7 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
        Write(ViewData["request_number"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n</p>\r\n<table width=\"0\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n    <tbody>\r\n        <tr>\r\n            <td width=\"172\" valign=\"top\">\r\n                <p>\r\n                    <strong>���� ��������:</strong>\r\n                    <strong>");
#nullable restore
#line 15 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                       Write(ViewData["creation_date"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</strong>
                </p>
            </td>
            <td width=""170"" valign=""top"" colspan=""2"">
                <p>
                    <strong> </strong>
                </p>
            </td>
            <td width=""282"" valign=""top"">
                <p>
                    <strong>
                        ����������� ���������� � ���������� �������
                    </strong>
                </p>
            </td>
        </tr>
        <tr>
            <td width=""624"" valign=""top"" colspan=""4"">
                <p>
                    <br>
                    <strong>������������ ��: </strong>
                    ���������� ������� �������� � �������� �� ��������� �
                    ������� ��������, ����������� ����� ����������� �
                    �������������, �� ������ ���������� �� ����� 0409405)�
                </p>
            </td>
        </tr>
        <tr>
            <td width=""180"" valign=""top"" colspan=""2"">
                <p>
                    <stro");
            WriteLiteral("ng>�������������� ���� ����������:</strong>\r\n                </p>\r\n            </td>\r\n            <td width=\"444\" valign=\"top\" colspan=\"2\">\r\n                <p>\r\n                    ");
#nullable restore
#line 50 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
               Write(ViewData["expected_date"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                </p>
            </td>
        </tr>
        <tr>
            <td width=""624"" valign=""top"" colspan=""4"">
                <p>
                    <strong>����� ������� ������������:</strong>
                </p>
                <p>
                    ");
#nullable restore
#line 60 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
               Write(ViewData["user_text"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                </p>
            </td>
        </tr>
        <tr>
            <td width=""624"" valign=""top"" colspan=""4"">
                <p>
                    <strong>��������� ������� ������������:</strong>
                </p>
                <p>
");
#nullable restore
#line 70 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                      
                        string reqJson = ViewData["requirement"].ToString();
                        dynamic reqObject = JsonConvert.DeserializeObject<dynamic>(reqJson);
                        

#line default
#line hidden
#nullable disable
#nullable restore
#line 73 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                   Write(Html.Raw(@reqObject.main_text.ToString().Replace("\n", "<br>")));

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <ul>\r\n");
#nullable restore
#line 75 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                             foreach (dynamic child in reqObject.child_nodes)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <li>\r\n                                    <b>");
#nullable restore
#line 78 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                                  Write(child.header);

#line default
#line hidden
#nullable disable
            WriteLiteral("</b><br>\r\n                                    ");
#nullable restore
#line 79 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                               Write(Html.Raw(@child.main_text.ToString().Replace("\n", "<br>")));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 80 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                                     if (child.child_nodes != null && child.child_nodes.Count > 0)
                                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                        <ul>\r\n");
#nullable restore
#line 83 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                                             foreach (dynamic ch in child.child_nodes)
                                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                <li>\r\n                                                    <b>");
#nullable restore
#line 86 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                                                  Write(ch.header);

#line default
#line hidden
#nullable disable
            WriteLiteral("</b><br>\r\n                                                    ");
#nullable restore
#line 87 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                                               Write(Html.Raw(@ch.main_text.ToString().Replace("\n", "<br>")));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                                </li>\r\n");
#nullable restore
#line 89 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                        </ul>\r\n");
#nullable restore
#line 91 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                </li>\r\n");
#nullable restore
#line 93 "D:\DersaProject.git\Dersa_C\Views\Temp\test.cshtml"
                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </ul>\r\n");
            WriteLiteral("                </p>\r\n\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591