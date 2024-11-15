using System;
using System.Diagnostics;

namespace ScaleThreadProcess
{
    public class ProcessorTemplate : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region "Properties"
        #endregion

        #region Processors' class implemenations

        /// <summary>
        /// Prepare statements (grab text by using the OCR, or other methods based on the file types)
        /// </summary>
        /// <param name="callStatementId"></param>
        /// <param name="saveProcessedContent"></param>
        public (bool isError, string errorMessage) ProcessortClassName()

        {
            bool isError = false;
            string generalErrorMessage = null;

            Trace.WriteLine($"[{DateTime.Now}] > processing: {123}");
            Trace.Flush();

            return (isError, generalErrorMessage);
        }

        #endregion
    }
}
