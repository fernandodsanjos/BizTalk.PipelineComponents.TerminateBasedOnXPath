# Terminate messsage based on XPath
BizTalk PipelineComponent to terminate a BizTalk message if a specific XPATH is found

Common scenario is a request-response where the response usually contains a number or a text to specify if the message was processed successfully or not.<br/>
Sometimes we are simply not interessted in the message if it is processed successfully.

The component uses BizTalks own XPathReader to a specified XML node, unfortunally a major restriction made to XPath in the XPathReader is to disallow testing for the values of elements or text nodes. The XPathReader does not support the following XPath expression:
`/books/book[contains(.,'Frederick Brooks')]`.<br/>
However, testing values of attributes, comments, or processing instructions is supported. The following XPath expression is supported by the XPathReader:
`/books/book[contains(@publisher,'WROX')]`.<br/>
I have added the functionality to specify testing on element value using a syntax bellow:

```xml
[text() = '0'] or [number() = 1]
```

A complete syntax would then look something like <br/>
<code>
/*[local-name()='Response']/*[local-name()='Result']/*[local-name()='ResultCode']<font color="red">[text() = '0']</font>
</code>

The element testing is always to be appended last to the XPath.

