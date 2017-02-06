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
       
        private const string XPathPropertyName = "XPath";
       

        [DisplayName("XPath")]
        [Description("The XPath to be evaluated")]
        [RequiredRuntime]
        public string XPath { get; set; }

       

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
           
            bool terminate = true;

            IBaseMessagePart bodyPart = pInMsg.BodyPart;

           

            Stream inboundStream = bodyPart.GetOriginalDataStream();
            VirtualStream virtualStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
            ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(inboundStream, virtualStream);

            XmlTextReader xmlTextReader = new XmlTextReader(readOnlySeekableStream);
            XPathCollection xPathCollection = new XPathCollection();
            XPathReader xPathReader = new XPathReader(xmlTextReader, xPathCollection);
            xPathCollection.Add(XPath);

            while (xPathReader.ReadUntilMatch())
            {
                if (xPathReader.Match(0))
                {
                    terminate = false;
                    inboundStream.Seek(0, SeekOrigin.Begin);
                    break;
                }
            }

           
            if( terminate == true)
            {
                pInMsg = null;
            }
           

            return pInMsg;
        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            XPath = PropertyBagHelper.ReadPropertyBag(propertyBag, XPathPropertyName, XPath);
         
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(propertyBag, XPathPropertyName, XPath);
        }
    }
}
