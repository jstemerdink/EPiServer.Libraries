// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Output.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.UnitTests.Subjects
{
    using System.Text;
    using System.Xml;

    using EPiServer.Libraries.Output;
    using EPiServer.Libraries.Output.Channels;
    using EPiServer.Libraries.Output.Formats;
    using EPiServer.Libraries.Output.Helpers;
    using EPiServer.Libraries.UnitTests.Specs;
    using EPiServer.Libraries.WebApi.Output.Controllers;

    using iTextSharp.text.pdf;

    using Machine.Specifications;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     Get text output.
    /// </summary>
    [Subject("Output")]
    public class Get_text_output : OutputSpecs
    {
        #region Fields

        /// <summary>
        ///     The of.
        /// </summary>
        private Because of = () => OutputHelper.HandleTxt(OutputItem, CmsContext.HttpContextBase);

        /// <summary>
        ///     The should_contain_txt_header
        /// </summary>
        private It should_contain_txt_header =
            () => CmsContext.HttpContextBase.Response.Headers["Content-Type"].ShouldEqual(OutputConstants.TextPlain);

        /// <summary>
        ///     The should_contain_txt_set_for_display
        /// </summary>
        private It should_contain_txt_set_for_display =
            () => CmsContext.HttpContextBase.Response.Output.ToString().ShouldContain(ContentToDisplay);

        /// <summary>
        ///     The should_not_contain_txt_not_set_for_display
        /// </summary>
        private It should_not_contain_txt_not_set_for_display =
            () => CmsContext.HttpContextBase.Response.Output.ToString().ShouldNotContain(ContentNotToDisplay);

        #endregion
    }

    /// <summary>
    ///     Get json output.
    /// </summary>
    [Subject("Output")]
    public class Get_json_output : OutputSpecs
    {
        #region Fields

        /// <summary>
        ///     The of.
        /// </summary>
        private Because of = () => OutputHelper.HandleJson(OutputItem, CmsContext.HttpContextBase);

        /// <summary>
        /// The should_be_valid_json.
        /// </summary>
        private It should_be_valid_json = () =>
            {
                object jsonResult = JsonConvert.DeserializeObject(CmsContext.HttpContextBase.Response.Output.ToString());
                JContainer a = jsonResult as JContainer;
                a.ShouldNotBeNull();
                JProperty t = a.First as JProperty;
                t.ShouldNotBeNull();
                t.Name.ShouldEqual(OutputItem.Name);

                JContainer c = t.Value as JContainer;
                c.ShouldNotBeNull();
                JProperty p = c.First as JProperty;
                p.ShouldNotBeNull();
                p.Value.ToString().ShouldEqual(ContentToDisplay);
                p.Name.ShouldEqual("outputtest_texttouseinoutput");
            };

        /// <summary>
        ///     The should_contain_txt_header
        /// </summary>
        private It should_contain_json_header =
            () => CmsContext.HttpContextBase.Response.Headers["Content-Type"].ShouldEqual(OutputConstants.ApplicationJSON);

        /// <summary>
        ///     The should_contain_txt_set_for_display
        /// </summary>
        private It should_contain_txt_set_for_display =
            () => CmsContext.HttpContextBase.Response.Output.ToString().ShouldContain(ContentToDisplay);

        /// <summary>
        ///     The should_not_contain_txt_not_set_for_display
        /// </summary>
        private It should_not_contain_txt_not_set_for_display =
            () => CmsContext.HttpContextBase.Response.Output.ToString().ShouldNotContain(ContentNotToDisplay);

        #endregion
    }

    /// <summary>
    ///     Get json output.
    /// </summary>
    [Subject("Output")]
    public class Get_webapi_json_output : OutputSpecs
    {
        #region Static Fields

        /// <summary>
        /// The api response.
        /// </summary>
        private static string apiResponse;

        #endregion

        #region Fields

        /// <summary>
        ///     The of.
        /// </summary>
        private Because of = () =>
        {
            ChannelSettings.Instance.EnableJSON = true;
            CmsContext.HttpContextBase.Request.AcceptTypes.ShouldNotBeNull();
            CmsContext.HttpContextBase.Request.AcceptTypes.SetValue(OutputConstants.ApplicationJSON, 0);
            ContentController controller = new ContentController(
                CmsContext.ContentRepository,
                CmsContext.HttpContextBase);

            ////controller.Request.Headers.Add("Accept", "application/json");
            apiResponse = controller.Get(OutputItem.PageLink.ID, CmsContext.MasterLanguage.Name);
        };

        /// <summary>
        ///     The should_be_valid_json.
        /// </summary>
        private It should_be_valid_json = () =>
        {
            object jsonResult = JsonConvert.DeserializeObject(apiResponse);
            JContainer a = jsonResult as JContainer;
            a.ShouldNotBeNull();
            JProperty t = a.First as JProperty;
            t.ShouldNotBeNull();
            t.Name.ShouldEqual(OutputItem.Name);

            JContainer c = t.Value as JContainer;
            c.ShouldNotBeNull();
            JProperty p = c.First as JProperty;
            p.ShouldNotBeNull();
            p.Value.ToString().ShouldEqual(ContentToDisplay);
            p.Name.ShouldEqual("outputtest_texttouseinoutput");
        };

        #endregion
    }

    /// <summary>
    ///     Get json output.
    /// </summary>
    [Subject("Output")]
    public class Get_no_json_output : OutputSpecs
    {
        #region Fields

        /// <summary>
        ///     The of.
        /// </summary>
        private Because of = () => OutputHelper.HandleJson(NoOutputItem, CmsContext.HttpContextBase);

        /// <summary>
        /// The should_be_no_valid_json.
        /// </summary>
        private It should_not_be_valid_json = () =>
        {
            object jsonResult = JsonConvert.DeserializeObject(CmsContext.HttpContextBase.Response.Output.ToString());
            JContainer a = jsonResult as JContainer;
            a.ShouldBeNull();
        };

        /// <summary>
        ///     The should_not_contain_txt_header
        /// </summary>
        private It should_not_contain_json_header =
            () => CmsContext.HttpContextBase.Response.Headers["Content-Type"].ShouldBeNull();

        /// <summary>
        ///     The should_contain_txt_set_for_display
        /// </summary>
        private It should_contain_txt_set_for_display =
            () => CmsContext.HttpContextBase.Response.Output.ToString().ShouldNotContain(ContentToDisplay);

        /// <summary>
        ///     The should_not_contain_txt_not_set_for_display
        /// </summary>
        private It should_not_contain_txt_not_set_for_display =
            () => CmsContext.HttpContextBase.Response.Output.ToString().ShouldNotContain(ContentNotToDisplay);

        #endregion
    }

    /// <summary>
    ///     Get xml output.
    /// </summary>
    [Subject("Output")]
    public class Get_xml_output : OutputSpecs
    {
        #region Fields

        /// <summary>
        ///     The of.
        /// </summary>
        private Because of = () => OutputHelper.HandleXml(OutputItem, CmsContext.HttpContextBase);

        /// <summary>
        ///     Check if the XML is valid and if the first data is the content to display.
        /// </summary>
        private It should_be_valid_xml = () =>
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(CmsContext.HttpContextBase.Response.Output.ToString());
                xmlDocument.FirstChild.FirstChild.InnerText.ShouldEqual(ContentToDisplay);
                xmlDocument.FirstChild.FirstChild.Name.ShouldEqual("outputtest_texttouseinoutput");
            };

        /// <summary>
        ///     The should_contain_txt_set_for_display
        /// </summary>
        private It should_contain_txt_set_for_display =
            () => CmsContext.HttpContextBase.Response.Output.ToString().ShouldContain(ContentToDisplay);

        /// <summary>
        ///     The should_contain_txt_header
        /// </summary>
        private It should_contain_xml_header =
            () => CmsContext.HttpContextBase.Response.Headers["Content-Type"].ShouldEqual(OutputConstants.TextXML);

        /// <summary>
        ///     The should_not_contain_txt_not_set_for_display
        /// </summary>
        private It should_not_contain_txt_not_set_for_display =
            () => CmsContext.HttpContextBase.Response.Output.ToString().ShouldNotContain(ContentNotToDisplay);

        #endregion
    }

    /// <summary>
    ///     Get xml output.
    /// </summary>
    [Subject("Output")]
    public class Get_webapi_xml_output : OutputSpecs
    {
        #region Static Fields

        /// <summary>
        /// The api response.
        /// </summary>
        private static string apiResponse;

        #endregion

        #region Fields

        /// <summary>
        ///     The of.
        /// </summary>
        private Because of = () =>
        {
            ChannelSettings.Instance.EnableXML = true;
            CmsContext.HttpContextBase.Request.AcceptTypes.ShouldNotBeNull();
            CmsContext.HttpContextBase.Request.AcceptTypes.SetValue(OutputConstants.TextXML, 0);
            ContentController controller = new ContentController(
                CmsContext.ContentRepository,
                CmsContext.HttpContextBase);

            apiResponse = controller.Get(OutputItem.PageLink.ID, CmsContext.MasterLanguage.Name);
        };

        /// <summary>
        ///     Check if the XML is valid and if the first data is the content to display.
        /// </summary>
        private It should_be_valid_xml = () =>
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(apiResponse);
            xmlDocument.FirstChild.FirstChild.InnerText.ShouldEqual(ContentToDisplay);
            xmlDocument.FirstChild.FirstChild.Name.ShouldEqual("outputtest_texttouseinoutput");
        };

        #endregion
    }

    /// <summary>
    /// The get_pdf_output.
    /// </summary>
    [Subject("Output")]
    public class Get_pdf_output : OutputSpecs
    {
        #region Static Fields

        /// <summary>
        ///     The result.
        /// </summary>
        private static byte[] pdf;

        #endregion

        #region Fields

        /// <summary>
        ///     The of.
        /// </summary>
        private Because of = () =>
            {
                PdfOutputFormat pdfOutputFormat = new PdfOutputFormat();
                pdfOutputFormat.HandleFormat(OutputItem, CmsContext.HttpContextBase);
                pdf = PdfOutputFormat.GeneratePdf(OutputItem);
            };

        /// <summary>
        ///     Check if the PDF is valid and if the first data is the content to display.
        /// </summary>
        private It should_be_a_valid_pdf = () =>
            {
                PdfReader r = new PdfReader(pdf);
                byte[] pageContent = r.GetPageContent(1);
                string text = Encoding.UTF8.GetString(pageContent);
                text.ShouldContain(ContentToDisplay);
                text.ShouldNotContain(ContentNotToDisplay);
            };

        /// <summary>
        ///     The should_contain_txt_header
        /// </summary>
        private It should_contain_pdf_header =
            () => CmsContext.HttpContextBase.Response.Headers["Content-Type"].ShouldEqual(OutputConstants.ApplicationPDF);

        #endregion
    }
}