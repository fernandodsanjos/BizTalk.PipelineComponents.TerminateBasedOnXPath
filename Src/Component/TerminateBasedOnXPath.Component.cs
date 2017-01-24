using System;
using System.Collections;
using System.Linq;
using BizTalkComponents.Utils;

namespace Shared.PipelineComponents
{
    public partial class TerminateBasedOnXPath
    {
        public string Name { get { return "TerminateBasedOnXPath"; } }
        public string Version { get { return "1.0"; } }
        public string Description { get { return "Terminates message if Path cannot be found"; } }

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("E7F67FC1-FC6C-40B2-BD94-14AA7BD4F9D7");
        }

        public void InitNew()
        {

        }

        public IEnumerator Validate(object projectSystem)
        {
            return ValidationHelper.Validate(this, false).ToArray().GetEnumerator();
        }

        public bool Validate(out string errorMessage)
        {
            var errors = ValidationHelper.Validate(this, true).ToArray();

            if (errors.Any())
            {
                errorMessage = string.Join(",", errors);

                return false;
            }

            errorMessage = string.Empty;

            return true;
        }

        public IntPtr Icon { get { return IntPtr.Zero; } }
    }
}
