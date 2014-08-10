using System.Text;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace Yahoo.Yui.Compressor.Tests.TestHelpers
{
    public static class BuildEngine
    {
        public static bool ContainsError(IBuildEngine engine, string error)
        {
            return CheckForError(engine, error, true);
        }

        public static bool DoesNotContainError(IBuildEngine engine, string error)
        {
            return CheckForError(engine, error, false);
        }

        private static bool CheckForError(IBuildEngine engine, string error, bool exists)
        {
            var buildEngine = engine as BuildEngineStub;
            if (buildEngine == null)
            {
                Assert.Fail("Not a BuildEngineStub, cannot test with this");
            }
            if (buildEngine.Errors != null && buildEngine.Errors.Count > 0)
            {
                foreach (var anError in buildEngine.Errors)
                {
                    if (anError.StartsWith(error))
                    {
                        return exists;
                    }
                }
            }

            if (!exists) 
            {
                return true;
            }

            var sb = new StringBuilder();
            sb.AppendLine(error + " not found.  Actual errors: ");
            foreach (var anError in buildEngine.Errors)
            {
                sb.AppendLine(anError);
            }
            Assert.Fail(sb.ToString());

            // ReSharper disable HeuristicUnreachableCode
            return !exists;
            // ReSharper restore HeuristicUnreachableCode
        }
    }
}
