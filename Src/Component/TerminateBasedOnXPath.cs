using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.BizTalk.XPath;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;
using BizTalkComponents.Utils;

namespace Shared.PipelineComponents
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("E7F67FC1-FC6C-40B2-BD94-14AA7BD4F9D7")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public partial class TerminateBasedOnXPath : IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
       

        [DisplayName("XPath")]
        [Description("The XPath to evaluated")]
        [RequiredRuntime]
        public string XPath { get; set; }

      
        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
           
            IBaseMessagePart bodyPart = pInMsg.BodyPart;

            bool found = false;
            string value = string.Empty;
            string evalString = string.Empty;

            Stream inboundStream = bodyPart.GetOriginalDataStream();
            VirtualStream virtualStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
            ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(inboundStream, virtualStream);

            XmlTextReader xmlTextReader = new XmlTextReader(readOnlySeekableStream);
            XPathCollection xPathCollection = new XPathCollection();
            XPathReader xPathReader = new XPathReader(xmlTextReader, xPathCollection);

            int lastxpath = XPath.LastIndexOf("][");

            if(lastxpath > -1)
            {
                evalString = XPath.Substring(lastxpath + 2, XPath.LastIndexOf(']') - (lastxpath + 2));
                XPath = XPath.Substring(0, lastxpath + 1);

                if (evalString.Contains("text(") || evalString.Contains("number("))
                {
                    evalString = evalString.Substring(evalString.IndexOf(')') + 1);

                }

                evalString = replaceHTMLOpearators(evalString);

                if (evalString.Contains(" = "))//c# equal sign
                    evalString = evalString.Replace("=", "==");

            }

            xPathCollection.Add(XPath);

            while (xPathReader.ReadUntilMatch())
            {
                if (xPathReader.Match(0))
                {
                    if (evalString != string.Empty)
                    {
                        if (xPathReader.NodeType == XmlNodeType.Attribute)
                        {
                            value = xPathReader.GetAttribute(xPathReader.Name);
                        }
                        else
                        {
                            value = xPathReader.ReadString();
                        }

                        string literal = String.Empty;
                        if (evalString.Contains("'"))
                        {
                            literal = "\"";
                            evalString = evalString.Replace("'", "\"");
                        }

                        found = ScriptExpressionHelper.ValidateExpression(String.Format("{0}{1}{0}", literal, value), evalString);

                    }
                    else
                        found = true;


                    
                    break;
                }
            }

            readOnlySeekableStream.Seek(0, SeekOrigin.Begin);
            pInMsg.BodyPart.Data = readOnlySeekableStream;

            if (found)
            {
                pInMsg = null;
            }

           
            return pInMsg;
        }

        private string replaceHTMLOpearators(string evalString)
        {
            if (evalString.Contains("&lt;"))
               evalString = evalString.Replace("&lt;", "<");

            if (evalString.Contains("&gt;"))
                evalString = evalString.Replace("&gt;", ">");

            if (evalString.Contains("&eq;"))
                evalString = evalString.Replace("&eq;", "==");

            if (evalString.Contains("&ne;"))
                evalString = evalString.Replace("&ne;", "!=");

            if (evalString.Contains("&ge;"))
                evalString = evalString.Replace("&ge;", ">=");

            if (evalString.Contains("&le;"))
                evalString = evalString.Replace("&le;", "<=");
        

            return evalString;
        }

        //Load and Save are generic, the functions create properties based on the components "public" "read/write" properties.
        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            var props = this.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var prop in props)

            {

                if (prop.CanRead & prop.CanWrite)

                {

                    prop.SetValue(this, PropertyBagHelper.ReadPropertyBag(propertyBag, prop.Name, prop.GetValue(this)));

                }

            }


        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            var props = this.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var prop in props)

            {

                if (prop.CanRead & prop.CanWrite)

                {

                    PropertyBagHelper.WritePropertyBag(propertyBag, prop.Name, prop.GetValue(this));

                }

            }

        }
    }
}
